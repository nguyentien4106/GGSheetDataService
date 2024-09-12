using DataService.Core.Contracts;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;

namespace DataService.Application.Services
{
    public interface IDeviceService : IGenericRepository<Device>
    {
    }
}
