using DataService.Core.Entities;
using GoogleSheetsWrapper;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Interfaces;

public interface IQueueSender
{
    Task SendAttendance(Attendance att);
    Task SendMessageToQueue(List<string> message, string queueName);
    Task SendMessageToQueue(Dictionary<SheetAppender, List<string>> rowSheet, string queueName);
}
