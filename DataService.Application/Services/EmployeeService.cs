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
    public class EmployeeService(AppDbContext context, ILogger<GenericRepository<Employee>> logger) : GenericRepository<Employee>(context, logger), IEmployeeService
    {
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
            _logger.LogInformation("Requested to Add Employee successfully! Please wait...");

            return Result.Success("Requested to Add Employee successfully! Please wait...");
        }

    }
}
