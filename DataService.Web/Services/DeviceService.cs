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

        public Result Delete(string ip)
        {
            var device = GetDevices().FirstOrDefault(item => item.IP == ip);

            return _appSettingsService.RemoveToArray("Devices", JsonSerializer.Serialize(device), ip);

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
