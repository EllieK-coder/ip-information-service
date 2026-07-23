using Autofac;
using Autofac.Extensions.DependencyInjection;
using IPService.Application;
using IPService.Infrastructure;

using IPService.Application.HttpClients;


using IPService.Application.Caching;
using IPService.Infrastructure.Caching;
using IPService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using IPService.Gateway;

using Quartz;
using IPService.Configuration;
using IPService.Jobs;



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
});

//caching
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, MemoryCacheStore>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();