using DataService.Web.Models.Settings;

namespace DataService.Web.Services
{
    public interface IAppSettingsService
    {
        List<Device> GetCurrentDevicesSettings();
    }
}
