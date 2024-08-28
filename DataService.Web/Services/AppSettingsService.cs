using DataService.Web.Helper;
using DataService.Web.Models.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace DataService.Web.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private IConfiguration _configuration;
        private string _appSettingFilePath;

        public AppSettingsService(string pathFile)
        {
            if (string.IsNullOrEmpty(pathFile))
            {
                throw new ArgumentNullException(nameof(pathFile));
            }

            _appSettingFilePath = pathFile;
        }

        public List<Device> GetCurrentDevicesSettings()
        {
            return LoadJson().DeserializeToJObject<List<Device>>("Devices");
        }

        private string LoadJson()
        {
            using StreamReader r = new(_appSettingFilePath);
            return r.ReadToEnd();
        }
    }
}
