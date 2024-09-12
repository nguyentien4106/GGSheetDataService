using CleanArchitecture.Core.Interfaces;
using DataService.Infrastructure.Entities;
using GoogleSheetsWrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Messaging;

/// <summary>
/// A simple implementation using the built-in Queue type and a single static instance.
/// </summary>
public class InMemoryQueueReceiver : IQueueReceiver
{
    public static Queue<List<string>> MessageQueue = new Queue<List<string>>();
    public static Queue<Dictionary<SheetAppender, List<string>>> AttendanceRowsSheet = new ();
    public static Queue<Attendance> AttendanceRowsDB = new();

    public async Task<Attendance> GetAttendanceRowsDB()
    {
        await Task.CompletedTask; // just so async is allowed

        if (AttendanceRowsDB.Count == 0) return null;

        return AttendanceRowsDB.Dequeue();
    }

    public async Task<List<string>> GetMessageFromQueue(string queueName)
    {
        await Task.CompletedTask; // just so async is allowed
        //Guard.Against.NullOrWhiteSpace(queueName, nameof(queueName));
        if (MessageQueue.Count == 0) return null;
        return MessageQueue.Dequeue();
    }

    public Task<Dictionary<SheetAppender, List<string>>> GetRowSheet()
    {
        throw new NotImplementedException();
    }

    public async Task<Dictionary<SheetAppender, List<string>>> GetAttendanceRowsSheet()
    {
        await Task.CompletedTask; // just so async is allowed
        //Guard.Against.NullOrWhiteSpace(queueName, nameof(queueName));
        if (AttendanceRowsSheet.Count == 0) return null;
        return AttendanceRowsSheet.Dequeue();
    }
}
