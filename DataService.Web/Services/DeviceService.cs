using DataService.Web.Models;
using DataService.Web.Models.Settings;

namespace DataService.Web.Services
{
    public class DeviceService : IDeviceService
    {
        public DeviceService()
        {

        }

        public Result Add(Device device)
        {
            return Result.Success();
        }

        public Result Delete(Device device)
        {
            throw new NotImplementedException();
        }

        public Result Update(Device device)
        {
            throw new NotImplementedException();
        }
    }
}
