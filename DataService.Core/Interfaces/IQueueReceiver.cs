using DataService.Infrastructure.Entities;
using GoogleSheetsWrapper;
using System.Threading.Tasks;

namespace DataService.Core.Interfaces;

public interface IQueueReceiver
{
    Task<List<string>> GetMessageFromQueue(string queueName);
    Task<Dictionary<SheetAppender, List<string>>> GetAttendanceRowsSheet();
    Task<Attendance> GetAttendanceRowsDB();
}
