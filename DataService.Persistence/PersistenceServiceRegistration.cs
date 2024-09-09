using DataService.Persistence.Repositories;
using CleanAchitecture.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataService.Core.Entities;

namespace CleanAchitecture.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IGenericRepository<Device>, GenericRepository<Device>>();
            return services;
        }
    }
}
