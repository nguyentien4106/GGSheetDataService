using CleanAchitecture.Application;
using CleanAchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Core.Services;
using CleanArchitecture.Infrastructure;
using DataService.Core.Entities;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddPostgresDB(builder.Configuration);
builder.Services.AddScoped<IServiceLocator, ServiceScopeFactoryLocator>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
