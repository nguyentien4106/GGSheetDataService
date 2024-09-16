using DataService.Core.Services;
using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using Microsoft.Extensions.Logging;
namespace DataService.Application.Services
{
    public class AttendanceService : GenericRepository<Attendance>, IAttendanceService
    {
        IServiceLocator _serviceLocator;
        public AttendanceService(AppDbContext context, IServiceLocator locator, ILogger<GenericRepository<Attendance>> logger) : base(context, logger)
        {
            _serviceLocator = locator;
        }

    }
}
