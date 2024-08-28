using DataService.Web.Models.Settings;

namespace DataService.Web.ViewModel.Home
{
    public class IndexViewModel
    {
        public Device NewDevice { get; set; } = new();

        public List<Device> Devices { get; set; }
    }
}
