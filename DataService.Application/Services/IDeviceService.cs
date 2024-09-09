using CleanAchitecture.Application.Contracts.Persistence;
using DataService.Core.Entities;

namespace DataService.Application.Services
{
    public interface IDeviceService : IGenericRepository<Device>
    {
    }
}
