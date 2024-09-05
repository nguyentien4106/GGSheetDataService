using DataService.Web.Models;
using DataService.Web.Models.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Collections.Specialized.BitVector32;

namespace DataService.Web.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private string _settings;
        private readonly string _filePath;
        private JObject _settingsObject;

        public AppSettingsService(IConfiguration configuration)
        {
            _filePath = configuration.GetSection("FileSettingPath").Value ?? default!;
            if (string.IsNullOrEmpty(_filePath))
            {
                throw new ArgumentNullException("FileSettingPath");
            }

            _settings = LoadJson();
            _settingsObject = JsonConvert.DeserializeObject(_settings) as JObject;
        }

        public List<Device> GetDevices()
        {
            return _settingsObject.SelectToken("Devices")?.ToObject<List<Device>>() ?? new();
        }

        public bool Update(string section, string value)
        {
            var token = _settingsObject.SelectToken(section);
            token?.Replace(value);
            UpdateJson();
            return true;
        }

        public bool UpdateList<T>(string section, T value)
        {
            var tokens = _settingsObject.SelectTokens(section);
            return true;
        }

        private string LoadJson( )
        {
            using StreamReader r = new(_filePath);
            return r.ReadToEnd();
        }

        private void UpdateJson()
        {
            File.WriteAllText(_filePath, _settingsObject.ToString());
        }

        public Result AddToArray(string section, string value)
        {
            try
            {
                var devices = JArray.FromObject(_settingsObject[section]);
                devices.Add(JToken.Parse(value));

                _settingsObject[section]?.Replace(devices.ToObject<JToken>());
                UpdateJson();
            }
            catch (Exception ex)
            {
                return Result.Fail(-1, message: ex.Message);
            }

            return Result.Success();
        }

        public Result RemoveToArray(string section, string value, string ip)
        {
            try
            {
                var devices = JArray.FromObject(_settingsObject[section]).Where(item => (string)item["IP"] != ip).ToList();
                JArray.FromObject(devices);
                _settingsObject[section]?.Replace(JArray.FromObject(devices));
                UpdateJson();
            }
            catch (Exception ex)
            {
                return Result.Fail(-1, message: ex.Message);
            }

            return Result.Success();
        }
    }
}
