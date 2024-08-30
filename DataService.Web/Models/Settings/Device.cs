namespace DataService.Web.Models.Settings
{
    public class Device
    {
        public string IP { get; set; } = "192.168.0.201";

        public string Port { get; set; } = "4370";

        public string CommKey { get; set; } = "0";

        public List<Sheet> Sheets { get; set; } = [];

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(IP) && !string.IsNullOrEmpty(Port) && !string.IsNullOrEmpty(CommKey);
        }
    }
}
