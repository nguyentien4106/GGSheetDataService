using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using DataService.Core.Entities;


namespace DataService.Infrastructure.Data
{
    public class DeviceRepository : EfRepository
    {
        public DeviceRepository(AppDbContext  context) : base(context)
        {
            
        }
    }
}
