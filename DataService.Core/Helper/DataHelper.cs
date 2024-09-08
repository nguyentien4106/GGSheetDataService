using CleanArchitecture.Core.Interfaces;
using DataService.Core.Entities;
using DataService.Core.Models.AttMachine;
using DataService.Models.AttMachine;
using DataWorkerService.Models;
using GoogleSheetsWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataService.Core.Helper
{
    public class DataHelper
    {
        public static Result PublishData(IEnumerable<SheetAppender> appenders, IRepository repository, OnAttendanceTransactionRecord record, Employee employee)
        {
            PublishDataToSheet(appenders, employee, null, record);
            PublishDataToDB(repository, record, null);

            return Result.Success();
        }

        public static Result PublishDataToSheet(IEnumerable<SheetAppender> appenders, Employee employee, IQueueSender queueSender, OnAttendanceTransactionRecord record)
        {
            var row = new List<string> {
                    record.UserId,
                    employee.Name,
                    employee.CardNumber,
                    UserPrivilege.GetUserPrivilegeName(employee.Privilege),
                    record.IsInvalid == 0 ? "Success" : "Failed",
                    AttState.GetAttState(record.AttState),
                    record.WorkCode.ToString(),
                    record.DateTimeRecord.ToString("HH:mm:ss MM/dd/yyyy"),
                    record.VerifyMethod.ToString(),
                };
            foreach (var appender in appenders)
            {
                try
                {
                    appender.AppendRow(row);
                }
                catch (Exception e)
                {
                    queueSender.SendMessageToQueue(new Dictionary<SheetAppender, List<string>>
                    {
                        { appender, row }
                    }, "MissRows");
                    return Result.Fail(400, e.Message);
                }
            }

            return Result.Success();

        }

        public static Result PublishDataToDB(IRepository repository, OnAttendanceTransactionRecord record, IQueueSender sender)
        {
            try
            {
                repository.Add<Attendance>(record.ToAttendance());
                return Result.Success();
            }
            catch (Exception ex)
            {
                sender.SendAttendance(record.ToAttendance());
                return Result.Fail(00, $"Cannot send attendace to database because of {ex.Message}");
            }
        }
    }
}
