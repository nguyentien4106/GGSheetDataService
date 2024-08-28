using DataService;
using DataService.PubSub;
using DataService.Settings;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET DataService";
});

builder.Services.AddSingleton(new CommandLineArgs(args));

var devices = builder.Configuration.GetSection("DevicesSettings").Get<List<Device>>() ?? default!;
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

