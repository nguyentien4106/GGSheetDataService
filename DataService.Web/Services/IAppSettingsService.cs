using DataService.Web.Models;
using DataService.Web.Models.Settings;

namespace DataService.Web.Services
{
    public interface IAppSettingsService
    {
        List<Device> GetDevices();

        bool Update(string section, string value);

        bool UpdateList<T>(string section, T value);

        Result AddToArray(string section, string value);

        Result RemoveToArray(string seciton, string value, string ip);
    }
}
