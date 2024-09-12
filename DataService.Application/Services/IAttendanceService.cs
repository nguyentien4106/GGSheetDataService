using CleanAchitecture.Application.Contracts.Persistence;
using DataService.Infrastructure.Entities;

namespace DataService.Application.Services
{
    public interface IAttendanceService : IGenericRepository<Attendance>
    {
    }
}
