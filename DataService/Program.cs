using DataService;
using DataService.PubSub;
using DataService.Settings;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
//var logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)   //read config from appsettings
//    .CreateLogger();
//builder.Services.AddSerilog(logger);
builder.Services.AddSerilog(config =>
{
    config.ReadFrom.Configuration(builder.Configuration);
    config.WriteTo.File(Path.Join(builder.Environment.ContentRootPath, "logs/service.log"));
});
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET DataService";
});


builder.Services.AddSingleton(new CommandLineArgs(args));

var devices = builder.Configuration.GetSection("Devices").Get<List<Device>>() ?? default!;
if (devices == null)
{
    throw new ArgumentNullException(nameof(DevicesAppSettings));
}

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

builder.Services.AddSingleton(new DevicesAppSettings(devices));
builder.Services.AddSingleton(credential);
builder.Services.AddSingleton(googleAccount);
builder.Services.AddSingleton(new Publisher("PublisherGGSheetData", 2000));
builder.Services.AddHostedService<Worker>();
builder.Services.AddSystemd();

var host = builder.Build();
host.Run();

