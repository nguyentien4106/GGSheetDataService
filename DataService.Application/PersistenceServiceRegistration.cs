using CleanAchitecture.Application.Contracts.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataService.Core.Entities;
using DataService.Application.Repositories;
using DataService.Application.Services;

namespace CleanAchitecture.Application
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IGenericRepository<Device>, GenericRepository<Device>>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            return services;
        }
    }
}
