using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Services;
using CleanArchitecture.Infrastructure.Data;
using DataService.Core.Entities;
using DataService.Core.Helper;
using DataService.Settings;
using DataWorkerService.Helper;
using DataWorkerService.Models.Config;
using Microsoft.EntityFrameworkCore;

namespace DataService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private GoogleApiAccount _account;
    private JSONCredential _jsonCredential;
    //private DevicesAppSettings _deviceSettings;

    private List<SDKHelper> _sdks = [];
    private AppDbContext _context;
    private IRepository _repository;
    IServiceLocator _locator;
    IQueueReceiver _receiver;
    IQueueSender _sender;

    public Worker(ILogger<Worker> logger, IServiceLocator locator, IRepository repository, IQueueReceiver receiver, IQueueSender sender)
    {
        _locator = locator;
        _logger = logger;
        _context = locator.Get<AppDbContext>();
        _repository = repository;
        _receiver = receiver;
        _sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int Hour = 1 * 60 * 60 * 1000;
        while (!stoppingToken.IsCancellationRequested)
        {
            var dictionary = await _receiver.GetRowSheets();
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            if (dictionary != null)
            {
                foreach (var entry in dictionary)
                {
                    //DataHelper.PublishDataToSheet(entry.Key, entry.Value, _sender, _repository);
                    _logger.LogInformation($"Re-sending data {string.Join("-", entry.Value)}");

                    await Task.Delay(1000, stoppingToken);

                }
            }
            await Task.Delay(5000, stoppingToken);

        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var devices = _context.Devices.Include(i => i.Sheets).ToList();
        
        foreach(var device in devices)
        {
            var sdk = new SDKHelper(_locator, device);
            _sdks.Add(sdk);
        }

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Turning off.");
        foreach(var sdk in _sdks)
        {
            sdk.sta_DisConnect();
        }
        return base.StopAsync(cancellationToken);
    }
}
