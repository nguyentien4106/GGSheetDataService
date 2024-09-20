using DataService.Core.Interfaces;
using DataService.Core.Messaging;
using DataService.Core.Services;
using DataService.Core.Contracts;
using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataService.Settings;
using DataWorkerService.Models.Config;
using DataService.Core.Settings;

namespace DataService.Core;

public static class ServiceCollectionSetupExtensions
{
    public static void AddPostgresDB(this IServiceCollection services, IConfiguration configuration) =>
        services.AddEntityFrameworkNpgsql().AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IGenericRepository<Attendance>, GenericRepository<Attendance>>();
        services.AddTransient<IGenericRepository<Device>, GenericRepository<Device>>();
        services.AddTransient<IGenericRepository<Employee>, GenericRepository<Employee>>();
        services.AddTransient<IGenericRepository<Notification>, GenericRepository<Notification>>();

    }

    public static void AddMessageQueues(this IServiceCollection services)
    {
        services.AddSingleton<IQueueReceiver, InMemoryQueueReceiver>();
        services.AddSingleton<IQueueSender, InMemoryQueueSender>();
    }

    public static void AddOtherServices(this IServiceCollection services, IConfiguration configuration)
    {
        var credential = configuration.GetSection("JSONCredential").Get<JSONCredential>() ?? default!;
        if (credential == null)
        {
            throw new ArgumentNullException(nameof(JSONCredential));
        }

        var googleAccount = configuration.GetSection("GoogleAccount").Get<GoogleApiAccount>() ?? default!;
        if (googleAccount == null)
        {
            throw new ArgumentNullException(nameof(GoogleApiAccount));
        }

        var rabbitmq = configuration.GetSection("RabbitMQ").Get<RabbitMQParams>() ?? default!;
        if (rabbitmq == null)
        {
            throw new ArgumentNullException(nameof(RabbitMQParams));
        }

        services.AddSingleton(credential);
        services.AddSingleton(googleAccount);
        services.AddSingleton(rabbitmq);

        services.AddSingleton<IServiceLocator, ServiceScopeFactoryLocator>();
        services.AddSingleton<ISDKService, SDKService>();
    }

}
