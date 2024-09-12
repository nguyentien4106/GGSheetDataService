using CleanAchitecture.Application.Contracts.Persistence;
using DataService.Infrastructure.Entities;

namespace DataService.Application.Services
{
    public interface IDeviceService : IGenericRepository<Device>
    {
    }
}
