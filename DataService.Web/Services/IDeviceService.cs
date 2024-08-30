using DataService.Web.Models;
using DataService.Web.Models.Settings;

namespace DataService.Web.Services
{
    public interface IDeviceService
    {
        Result Add(Device device);

        Result Delete(string ip);

        Result Update(Device device);

        List<Device> GetDevices();
    }
}
