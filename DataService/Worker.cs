using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Services;
using CleanArchitecture.Infrastructure.Data;
using DataService.Core.Entities;
using DataService.Core.Helper;
using DataService.Settings;
using DataWorkerService.Helper;
using DataWorkerService.Models.Config;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;

namespace DataService;

public class Worker : BackgroundService
{
    readonly ILogger<Worker> _logger;
    List<SDKHelper> _sdks = [];
    AppDbContext _context;

    IRepository _repository;
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
        const int ThirtyMinutes = 1 * 30 * 60 * 1000;
        while (!stoppingToken.IsCancellationRequested)
        {
            var sheets = await _receiver.GetAttendanceRowsSheet();
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            if (sheets != null)
            {
                foreach (var entry in sheets)
                {
                    DataHelper.PublishDataToSheet([entry.Key], entry.Value, _sender);
                    _logger.LogInformation($"Re-sending data to sheets {string.Join("-", entry.Value)}");

                    await Task.Delay(500, stoppingToken);

                }
            }

            var attendance = await _receiver.GetAttendanceRowsDB();
            if(attendance != null)
            {
                DataHelper.PublishDataToDB(_repository, attendance, _sender);
                _logger.LogInformation($"Re-sending data to DB {attendance.UserId} - {attendance.VerifyDate}");
            }

            foreach (var sdk in _sdks)
            {
                sdk.TestRealTimeEvent();
            }
            await Task.Delay(ThirtyMinutes, stoppingToken);
            
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var devices = _context.Devices.Include(i => i.Sheets).ToList();
        
        foreach(var device in devices)
        {
            var sdk = new SDKHelper(_locator, device, true);
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
