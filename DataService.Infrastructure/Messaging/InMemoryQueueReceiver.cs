using CleanArchitecture.Core.Interfaces;
using DataService.Core.Entities;
using GoogleSheetsWrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Messaging;

/// <summary>
/// A simple implementation using the built-in Queue type and a single static instance.
/// </summary>
public class InMemoryQueueReceiver : IQueueReceiver
{
    public static Queue<List<string>> MessageQueue = new Queue<List<string>>();
    public static Queue<Dictionary<SheetAppender, List<string>>> SheetRowQueues = new ();
    public static Queue<Attendance> Attendances = new();

    public async Task<Attendance> GetAttendances()
    {
        await Task.CompletedTask; // just so async is allowed

        if (Attendances.Count == 0) return null;

        return Attendances.Dequeue();
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

    public async Task<Dictionary<SheetAppender, List<string>>> GetRowSheets()
    {
        await Task.CompletedTask; // just so async is allowed
        //Guard.Against.NullOrWhiteSpace(queueName, nameof(queueName));
        if (SheetRowQueues.Count == 0) return null;
        return SheetRowQueues.Dequeue();
    }
}
