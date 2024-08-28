using DataService.PubSub;
using DataService.Settings;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;
using DataWorkerService.Models.Sheet;
using GoogleSheetsWrapper;
using Newtonsoft.Json;

namespace DataService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private SDKHelper _sdk;
    private SettingModel _model;
    private DevicesAppSettings _deviceSettings;
    private Publisher _publisher;
    private List<SheetHelper<Record>> _sheetsHelper = [];
    private JSONCredential _jsonCredential;
    private GoogleApiAccount _account;

    public Worker(ILogger<Worker> logger, DevicesAppSettings devices, Publisher p, JSONCredential credential, GoogleApiAccount account)
    {
        _logger = logger;
        _sdk = new SDKHelper(account, credential);
        _deviceSettings = devices;
        _publisher = p;
        _jsonCredential = credential;
        _account = account;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            a();
            await Task.Delay(5000, stoppingToken);
        }
    }

    public void a()
    {

        // Append new rows to the spreadsheet
        foreach (var sheet in _sheetsHelper)
        {
            var appender = new SheetAppender(sheet);

            // Appends weakly typed rows to the spreadsheeet
            appender.AppendRow(
                [
                    "Nguyen Van Tien", "created this row", "every 5s", DateTime.Now.ToLongDateString()
                ]);
        }
    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var device in _deviceSettings.Devices)
        {
            var result = _sdk.sta_ConnectTCP(device, device);
            _logger.LogInformation($"Connecting to {device.IP}");

            foreach(var sheet in device.Sheets)
            {
                var sheetHelper = new SheetHelper<Record>(sheet.DocumentId, _account.ServiceAccountId, sheet.SheetName);
                sheetHelper.Init(_jsonCredential.ToString());
                _sheetsHelper.Add(sheetHelper);
            }

            if (result.IsSuccess)
            {
                _logger.LogInformation("Connected Sucessfully");
                _logger.LogInformation($"Subscribing to publisher");
                device.Subscribe(_publisher);
            }
            else
            {
                _logger.LogError("Failed to connect, because {0}", result.Message);
            }
        }

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}
