using DataService.Web.Models;
using DataService.Web.Models.Settings;

namespace DataService.Web.Services
{
    public interface IDeviceService
    {
        Result Add(Device device);

        Result Delete(Device device);

        Result Update(Device device);

    }
}
