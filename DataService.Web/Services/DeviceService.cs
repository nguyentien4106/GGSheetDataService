using DataService.Web.Models;
using DataService.Web.Models.Settings;
using System.Text.Json;

namespace DataService.Web.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IAppSettingsService _appSettingsService;

        public DeviceService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public Result Add(Device device)
        {
            return _appSettingsService.AddToArray("Devices", JsonSerializer.Serialize(device));
        }

        public Result Delete(Device device)
        {
            return _appSettingsService.RemoveToArray("Devices", JsonSerializer.Serialize(device), device.IP);

        }

        public List<Device> GetDevices()
        {
            return _appSettingsService.GetDevices();
        }

        public Result Update(Device device)
        {
            throw new NotImplementedException();
        }
    }
}
