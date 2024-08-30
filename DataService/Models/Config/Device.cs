namespace DataWorkerService.Models
{
    public class Device 
    {
        public Device()
        {
            
        }

        public Device(string ip) 
        {
            IP = ip;
        }

        public required string IP { get; set; }

        public required string Port { get; set; }

        public required string CommKey { get; set; }

        public List<SheetConfig> Sheets { get; set; } = [];

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(IP) && !string.IsNullOrEmpty(Port) && !string.IsNullOrEmpty(CommKey);
        }
    }
}
