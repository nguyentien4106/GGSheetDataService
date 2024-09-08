using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Services;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Http;
using CleanArchitecture.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure;

public static class ServiceCollectionSetupExtensions
{
    public static void AddPostgresDB(this IServiceCollection services, IConfiguration configuration) =>
        services.AddEntityFrameworkNpgsql().AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

    public static void AddRepositories(this IServiceCollection services) =>
        services.AddScoped<IRepository, EfRepository>();

    public static void AddMessageQueues(this IServiceCollection services)
    {
        services.AddSingleton<IQueueReceiver, InMemoryQueueReceiver>();
        services.AddSingleton<IQueueSender, InMemoryQueueSender>();
    }

    public static void AddOtherServices(this IServiceCollection services)
    {
        services.AddSingleton<IServiceLocator, ServiceScopeFactoryLocator>();
    }

    public static void AddUrlCheckingServices(this IServiceCollection services)
    {
        services.AddTransient<IHttpService, HttpService>();
    }
}
