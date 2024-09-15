using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Services;
using DataService.Core.Contracts;
using DataService.Core.Helper;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;

namespace DataService;

public class Worker : BackgroundService
{
    readonly ILogger<Worker> _logger;
    List<SDKHelper> _sdks = [];

    IGenericRepository<Attendance> _attendanceRepos;
    IGenericRepository<Device> _deviceRepos;
    IServiceLocator _locator;
    IQueueReceiver _receiver;
    IQueueSender _sender;

    public Worker(ILogger<Worker> logger, IServiceLocator locator)
    {
        _locator = locator;
        _logger = logger;
        _deviceRepos = locator.Get<IGenericRepository<Device>>();
        _attendanceRepos = locator.Get<IGenericRepository<Attendance>>();
        _receiver = locator.Get<IQueueReceiver>();
        _sender = locator.Get<IQueueSender>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int ThirtyMinutes = 1 * 30 * 60 * 1000;
        const int TenSeconds = 10 * 1000;
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
                DataHelper.PublishDataToDB(_attendanceRepos, attendance, _sender);
                _logger.LogInformation($"Re-sending data to DB {attendance.UserId} - {attendance.VerifyDate}");
            }

            var devices = await _deviceRepos.GetAsync(includeProperties: "Sheets");
            foreach (var device in devices)
            {

            }
            //foreach (var sdk in _sdks)
            //{
            //    sdk.TestRealTimeEvent();
            //}
            await Task.Delay(TenSeconds, stoppingToken);
            
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var devices = _deviceRepos.Get(includeProperties: "Sheets");
        
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
