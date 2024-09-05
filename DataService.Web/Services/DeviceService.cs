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
            var devices = GetDevices();
            var existed = devices.Find(item => item.IP == device.IP);
            if(existed != null)
            {
                return Result.Fail(-400, "Duplicated IP");
            }
            device.ID = devices.Last().ID + 1;
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
