using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Messaging;
using CleanArchitecture.Core.Services;
using DataService.Core.Contracts;
using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    }

    public static void AddMessageQueues(this IServiceCollection services)
    {
        services.AddSingleton<IQueueReceiver, InMemoryQueueReceiver>();
        services.AddSingleton<IQueueSender, InMemoryQueueSender>();
    }

    public static void AddOtherServices(this IServiceCollection services)
    {
        services.AddSingleton<IServiceLocator, ServiceScopeFactoryLocator>();
    }

}
