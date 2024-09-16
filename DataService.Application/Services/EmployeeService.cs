using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DataService.Application.Services
{
    public class EmployeeService : GenericRepository<Employee>, IEmployeeService
    {
        public EmployeeService(AppDbContext context, ILogger<GenericRepository<Employee>> logger) : base(context, logger)
        {
        }


        public override Task<IEnumerable<Employee>> GetAsync(Expression<Func<Employee, bool>> filter = null, Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null, string includeProperties = "")
        {
            
            return base.GetAsync(filter, orderBy, includeProperties);
        }

    }
}
