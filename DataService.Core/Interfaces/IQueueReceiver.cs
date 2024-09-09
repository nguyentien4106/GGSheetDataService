using DataService.Core.Entities;
using GoogleSheetsWrapper;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Interfaces;

public interface IQueueReceiver
{
    Task<List<string>> GetMessageFromQueue(string queueName);
    Task<Dictionary<SheetAppender, List<string>>> GetAttendanceRowsSheet();
    Task<Attendance> GetAttendanceRowsDB();
}
