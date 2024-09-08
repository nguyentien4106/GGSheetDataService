using CleanArchitecture.Core.Interfaces;
using DataService.Core.Entities;
using GoogleSheetsWrapper;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Messaging;

/// <summary>
/// A simple implementation using the built-in Queue type
/// </summary>
public class InMemoryQueueSender : IQueueSender
{
    public async Task SendAttendance(Attendance att)
    {
        await Task.CompletedTask; // just so async is allowed
        InMemoryQueueReceiver.Attendances.Enqueue(att);
    }

    public async Task SendMessageToQueue(List<string> message, string queueName)
      {
        await Task.CompletedTask; // just so async is allowed
        InMemoryQueueReceiver.MessageQueue.Enqueue(message);
      }

    public async Task SendMessageToQueue(Dictionary<SheetAppender, List<string>> rowSheet, string queueName)
    {
        await Task.CompletedTask; // just so async is allowed
        InMemoryQueueReceiver.SheetRowQueues.Enqueue(rowSheet);
    }
}
