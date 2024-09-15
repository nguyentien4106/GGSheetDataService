using DataService.Infrastructure.Entities;

namespace DataService.Web.Models.Devices
{
    public class DeviceViewModel
    {
        public DeviceViewModel(Device device, bool status)
        {
            Device = device;
            Status = status;    
        }

        public Device Device { get; set; }
        public bool Status { get; set; }
    }
}
