using DataService;
using DataService.Core;
using Serilog;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddPostgresDB(builder.Configuration);
builder.Services.AddMessageQueues();
builder.Services.AddOtherServices(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddSerilog(config =>
{
    config.ReadFrom.Configuration(builder.Configuration);
    config.WriteTo.File(Path.Join(builder.Environment.ContentRootPath, "logs/.log"), rollingInterval: RollingInterval.Day);
    config.WriteTo.Console();
});
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "DataServiceSheetDB";
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddSystemd();

var host = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

host.Run();

