﻿using DataService.Infrastructure.Entities;
using GoogleSheetsWrapper;

namespace DataService.Core.Interfaces;

public interface IQueueSender
{
    Task SendAttendanceDB(Attendance att);
    Task SendMessageToQueue(List<string> message, string queueName);
    Task SendAttendancesSheet(Dictionary<SheetAppender, List<string>> rowSheet);
}
