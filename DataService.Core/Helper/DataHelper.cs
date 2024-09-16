using DataService.Core.Interfaces;
using DataService.Core.Contracts;
using DataService.Core.Models.AttMachine;
using DataService.Infrastructure.Entities;
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
        public static Result PublishData(IEnumerable<SheetAppender> appenders, IGenericRepository<Attendance> repository, OnAttendanceTransactionRecord record, Employee employee, IQueueSender sender)
        {
            PublishDataToSheet(appenders, record.ToRow(), sender);
            PublishDataToDB(repository, record.ToAttendance(), sender);
            
            return Result.Success();
        }

        public static Result PublishDataToSheet(IEnumerable<SheetAppender> appenders, List<string> row, IQueueSender queueSender)
        {
            foreach (var appender in appenders)
            {
                try
                {
                    appender.AppendRow(row);
                }
                catch (Exception e)
                {
                    queueSender.SendAttendancesSheet(new Dictionary<SheetAppender, List<string>>
                    {
                        { appender, row }
                    });
                    return Result.Fail(400, e.Message);
                }
            }

            return Result.Success();

        }

        public static Result PublishDataToDB(IGenericRepository<Attendance> repository, Attendance attendance, IQueueSender sender)
        {
            try
            {
                repository.Insert(attendance);
                return Result.Success();
            }
            catch (Exception ex)
            {
                sender.SendAttendanceDB(attendance);
                return Result.Fail(00, $"Cannot send attendace to database because of {ex.Message}");
            }
        }
    }
}
