using DataService.Core.Messaging;
using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using DocumentFormat.OpenXml.Vml.Office;
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

        public override async Task<Result> Insert(Employee employee)
        {
            if (employee.Pin.StartsWith("0"))
            {
                return Result.Fail(501, "PIN can not start with 0");
            }

            if (string.IsNullOrWhiteSpace(employee.Pin))
            {
                return Result.Fail(502, "PIN is null or whitespace only.");
            }

            if (!employee.Pin.All(char.IsDigit))
            {
                _logger.LogError("*User ID error! User ID only support digital");
                return Result.Fail(501, "*User ID error! User ID only support digital");

            }

            RabbitMQProducer.SendMessage(RabbitMQConstants.EmployeeEventQueue, new RabbitMQEvent<Employee>()
            {
                Data = employee,
                ActionType = ActionType.Added
            }.ToString());

            return Result.Success("Requested to Add Employee successfully! Please wait...");
        }

    }
}
