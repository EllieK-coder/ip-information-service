using IPService.Application.HttpClients;
using IPService.Application.Interfaces;
using IPService.Application.Queries;
using IPService.Infrastructure.Persistence;
using IPService.Infrastructure.Persistence.Commands;
using IPService.Infrastructure.Persistence.Queries;
using Microsoft.EntityFrameworkCore;
using IPService.Gateway;

var builder = WebApplication.CreateBuilder(args);

//basic stuff for controllers + swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

//db connection, reads the connection string from appsettings
builder.Services.AddDbContext<IpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//mediator finds all the handlers in the Application project by itself
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetCountryByIpQuery).Assembly));

//tell the app which class to use for each interface
//scoped = one instance per request (same with DbContext they use)
builder.Services.AddScoped<IGetIpAddress, GetIpAddressPersistence>();
builder.Services.AddScoped<ISaveIpAddress, SaveIpAddressPersistence>();
builder.Services.AddScoped<IEnsureCountry, EnsureCountryPersistence>();

builder.Services.AddHttpClient<IIp2cHttpClient, Ip2cHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://ip2c.org");
});

var app = builder.Build();

//swagger only when running locally(dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();