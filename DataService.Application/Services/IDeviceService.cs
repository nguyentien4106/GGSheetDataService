using DataService.Core.Contracts;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using DataWorkerService.Models;

namespace DataService.Application.Services
{
    public interface IDeviceService : IGenericRepository<Device>
    {
        Task<Result> Connect(int deviceid);
        Task<Result> Disconnect(int deviceid);
    }
}
