using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;

namespace DataService.Application.Services
{
    public class EmployeeService : GenericRepository<Employee>, IEmployeeService
    {
        public EmployeeService(AppDbContext context) : base(context)
        {
        }


    }
}
