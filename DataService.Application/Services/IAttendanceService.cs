using CleanAchitecture.Application.Contracts.Persistence;
using DataService.Core.Entities;

namespace DataService.Application.Services
{
    public interface IAttendanceService : IGenericRepository<Attendance>
    {
    }
}
