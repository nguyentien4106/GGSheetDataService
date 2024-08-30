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
            //open file stream
            using (StreamWriter file = File.CreateText(_filePath))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                //serialize object directly into file stream
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(_settingsObject, Newtonsoft.Json.Formatting.Indented);

                serializer.Serialize(file, _settingsObject);
            }

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

                _settingsObject[section]?.Replace(JToken.Parse(System.Text.Json.JsonSerializer.Serialize(devices)));
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
