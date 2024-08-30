using DataService.Settings;
using DataWorkerService.Helper;
using DataWorkerService.Models.Config;

namespace DataService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private GoogleApiAccount _account;
    private JSONCredential _jsonCredential;
    private DevicesAppSettings _deviceSettings;

    private List<SDKHelper> _sdks = [];

    public Worker(ILogger<Worker> logger, DevicesAppSettings devices, JSONCredential credential, GoogleApiAccount account)
    {
        _logger = logger;
        _deviceSettings = devices;
        _jsonCredential = credential;
        _account = account;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int Hour = 1 * 60 * 60 * 1000;
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(Hour, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var device in _deviceSettings.Devices)
        {
            var sdk = new SDKHelper(_account, _jsonCredential, _logger);
            _sdks.Add(sdk);
            sdk.sta_ConnectTCP(device);
        }

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}
