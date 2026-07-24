using Autofac;
using Autofac.Extensions.DependencyInjection;
using IPService.Application;
using IPService.Application.Caching;
using IPService.Application.HttpClients;
using IPService.Configuration;
using IPService.Gateway;
using IPService.Infrastructure;
using IPService.Infrastructure.Caching;
using IPService.Infrastructure.Persistence;
using IPService.Jobs;
using IPService.Middleware;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using NLog.Extensions.Logging;



var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(cb =>
{
    cb.RegisterModule<ApplicationModule>();
    cb.RegisterModule<InfrastructureModule>();
});

//basic stuff for controllers + swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

//db connection, reads the connection string from appsettings
builder.Services.AddDbContext<IpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);



builder.Services.AddHttpClient<IIp2cHttpClient, Ip2cHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://ip2c.org");
})
.AddResilienceHandler("ip2c-pipeline", pipeline =>
{
    pipeline.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        Delay = TimeSpan.FromSeconds(2),
        UseJitter = true,
        OnRetry = args =>
        {
            Console.WriteLine($"[Polly] Retry {args.AttemptNumber} after {args.RetryDelay.TotalSeconds}s");
            return default;
        }
    });

    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        MinimumThroughput = 5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        BreakDuration = TimeSpan.FromSeconds(30),
        OnOpened = args =>
        {
            Console.WriteLine("[Polly] Circuit OPENED - failing fast for 30s");
            return default;
        },
        OnClosed = args =>
        {
            Console.WriteLine("[Polly] Circuit CLOSED - back to normal");
            return default;
        }
    });

    pipeline.AddTimeout(TimeSpan.FromSeconds(30));
});


    
    

//caching
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, MemoryCacheStore>();

//Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//logging
builder.Logging.ClearProviders();         
builder.Logging.AddNLog();

//ip refresh background job (Task 2) — Quartz, mirroring WorldRank DataFetchJob
builder.Services.Configure<IpRefreshOptions>(
    builder.Configuration.GetSection(IpRefreshOptions.SectionName));

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey(nameof(IpRefreshJob));

    q.AddJob<IpRefreshJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(t => t
        .ForJob(jobKey)
        .WithIdentity($"{nameof(IpRefreshJob)}-trigger")
        .StartNow()                              // fire immediately on startup
        .WithSimpleSchedule(s => s
            .WithIntervalInHours(1)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

var app = builder.Build();

//swagger only when running locally(dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();