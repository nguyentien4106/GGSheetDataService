using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataService.Application.Services;
using DataService.Infrastructure.Entities;
using DataService.Core.Contracts;
using DataService.Core.Repositories;

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
            services.AddScoped<IEmployeeService, EmployeeService>();
            return services;
        }
    }
}
