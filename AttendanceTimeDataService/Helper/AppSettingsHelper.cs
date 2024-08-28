using Newtonsoft.Json;
using System.Configuration;

namespace DataService.Web.Helper
{
    public static class AppSettingsHelper
    {
        public static void AddOrUpdateAppSetting<T>(string key, T value, string filePath = "")
        {
            try
            {
                filePath = string.IsNullOrEmpty(filePath) ? Path.Combine(AppContext.BaseDirectory, "appsettings.json") : filePath;
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];

                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no sectionpath just set the value
                }

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
