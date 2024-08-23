using DataService.Ob;
using DataService.Settings;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using Newtonsoft.Json;

namespace DataService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private SDKHelper _sdk;
    private SettingModel _model;
    private DevicesAppSettings _deviceSettings;
    private Publisher _publisher;

    public Worker(ILogger<Worker> logger, DevicesAppSettings devices, Publisher p)
    {
        _logger = logger;
        _sdk = new SDKHelper();
        _deviceSettings = devices;
        _publisher = p;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
            _publisher.Publish();
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        foreach(var device in _deviceSettings.Devices)
        {
            var result = _sdk.sta_ConnectTCP(device);
            _logger.LogInformation($"Connecting to {device.IP}");

            if (result.IsSuccess)
            {
                _logger.LogInformation("Connected Sucessfully");
                _logger.LogInformation($"Subscribing to publisher");
                device.Subscribe(_publisher);
            }
        }

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _cmd = null;
        _sdk = null;
        _model = null;
        return base.StopAsync(cancellationToken);
    }
}
