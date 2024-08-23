using DataService;
using DataService.Ob;
using DataService.Settings;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton(new CommandLineArgs(args));
var devices = builder.Configuration.GetSection("DevicesSettings").Get<List<Device>>() ?? default!;
if (devices == null)
{
    throw new ArgumentNullException(nameof(DevicesAppSettings));
}

builder.Services.AddSingleton(new DevicesAppSettings(devices));
builder.Services.AddSingleton(new DevicesAppSettings(devices));
builder.Services.AddSingleton(new Publisher("PublisherGGSheetData", 2000));

var host = builder.Build();
host.Run();
