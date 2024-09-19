using CleanAchitecture.Application;
using DataService.Core;
using DataService.Infrastructure.Data;
using Google;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddPostgresDB(builder.Configuration);
builder.Services.AddOtherServices(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddMessageQueues();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    var context = services.GetRequiredService<AppDbContext>();
//    var logger = services.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation("Logger");
//    if (context.Database.GetPendingMigrations().Any())
//    {
//        logger.LogInformation("context.Database.GetPendingMigrations().Any()");
//        context.Database.Migrate();
//        logger.LogInformation("context.Database.GetPendingMigrations().Any() Migrate() ");

//    }
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
