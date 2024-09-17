using DataService.Core.Interfaces;
using DataService.Core.Contracts;
using DataService.Core.Messaging;
using DataService.Infrastructure.Entities;
using DataService.Core.Services;
using RabbitMQ.Client;

namespace DataService;

public class Worker : BackgroundService
{
    readonly ILogger<Worker> _logger;
    IServiceLocator _locator;
    RabbitMQConsumer _consumer;
    IModel _model;
    ISDKService _sdkService;

    public Worker(ILogger<Worker> logger, IServiceLocator locator)
    {
        _locator = locator;
        _logger = logger;
        _consumer = new RabbitMQConsumer(locator);
        _sdkService = locator.Get<ISDKService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int TenSeconds = 60*10 * 1000;
        while (!stoppingToken.IsCancellationRequested)
        {
            var sdks = _sdkService.GetCurrentSDKs();
            var runningCount = sdks.Where(item => item.GetConnectState());
            _logger.LogInformation($"Total {sdks.Count} in system. {runningCount.Count()} running: {string.Join(",", runningCount)}");
            await Task.Delay(TenSeconds, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _sdkService.Init();
        _consumer.StartListeningOnDeviceQueue();
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Turning off.");
        _model.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
