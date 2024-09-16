using Newtonsoft.Json;

namespace DataService.Infrastructure.Entities
{
    public class Device : BaseEntity
    {
        public string Ip { get; set; } = string.Empty;

        public string Port { get; set; } = string.Empty;

        public string CommKey { get; set; } = string.Empty;

        public bool IsConnected { get; set; } = false;

        public List<Sheet> Sheets { get; set; } = [];

        public List<Attendance> Attendances { get; set; } = [];

        public bool IsSameValue(Device other) => JsonConvert.SerializeObject(this) == JsonConvert.SerializeObject(other);
    }
}
