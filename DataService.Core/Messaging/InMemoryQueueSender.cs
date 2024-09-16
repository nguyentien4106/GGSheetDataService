using DataService.Core.Interfaces;
using DataService.Infrastructure.Entities;
using GoogleSheetsWrapper;
using System.Threading.Tasks;

namespace DataService.Core.Messaging;

/// <summary>
/// A simple implementation using the built-in Queue type
/// </summary>
public class InMemoryQueueSender : IQueueSender
{
    public async Task SendAttendanceDB(Attendance att)
    {
        await Task.CompletedTask; // just so async is allowed
        InMemoryQueueReceiver.AttendanceRowsDB.Enqueue(att);
    }

    public async Task SendMessageToQueue(List<string> message, string queueName)
      {
        await Task.CompletedTask; // just so async is allowed
        InMemoryQueueReceiver.MessageQueue.Enqueue(message);
      }

    public async Task SendAttendancesSheet(Dictionary<SheetAppender, List<string>> rowSheet)
    {
        await Task.CompletedTask; // just so async is allowed
        InMemoryQueueReceiver.AttendanceRowsSheet.Enqueue(rowSheet);
    }
}
