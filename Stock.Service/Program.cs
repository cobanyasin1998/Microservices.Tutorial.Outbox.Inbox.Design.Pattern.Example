using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Settings;
using Stock.Service.Consumers;
using Stock.Service.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<StockDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer")));


builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEvent, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

var host = builder.Build();
host.Run();
