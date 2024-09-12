using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using System.Linq.Expressions;

namespace DataService.Application.Services
{
    public class EmployeeService : GenericRepository<Employee>, IEmployeeService
    {
        private IDeviceService _deviceSerivce;
        public EmployeeService(AppDbContext context, IDeviceService deviceService) : base(context)
        {
            _deviceSerivce = deviceService;
        }


        public override Task<IEnumerable<Employee>> GetAsync(Expression<Func<Employee, bool>> filter = null, Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null, string includeProperties = "")
        {
            
            return base.GetAsync(filter, orderBy, includeProperties);
        }

    }
}
