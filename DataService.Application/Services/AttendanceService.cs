using CleanArchitecture.Core.Services;
using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
namespace DataService.Application.Services
{
    public class AttendanceService : GenericRepository<Attendance>, IAttendanceService
    {
        IServiceLocator _serviceLocator;
        public AttendanceService(AppDbContext context, IServiceLocator locator) : base(context)
        {
            _serviceLocator = locator;
        }

    }
}
