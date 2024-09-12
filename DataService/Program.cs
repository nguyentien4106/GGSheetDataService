using DataService;
using DataService.Core;
using DataService.Settings;
using DataWorkerService.Models.Config;
using Serilog;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddPostgresDB(builder.Configuration);
builder.Services.AddMessageQueues();
builder.Services.AddOtherServices();
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

var credential = builder.Configuration.GetSection("JSONCredential").Get<JSONCredential>() ?? default!;
if(credential == null)
{
    throw new ArgumentNullException(nameof(JSONCredential));
}

var googleAccount = builder.Configuration.GetSection("GoogleAccount").Get<GoogleApiAccount>() ?? default!;
if (googleAccount == null)
{
    throw new ArgumentNullException(nameof(GoogleApiAccount));
}

builder.Services.AddSingleton(credential);
builder.Services.AddSingleton(googleAccount);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSystemd();

var host = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

host.Run();

