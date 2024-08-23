using DataService.Models;

namespace DataService.PubSub
{
    public class NotificationEvent : EventData
    {
        public string Message { get; private set; }

        public NotificationEvent(DateTime _dateTime, string _message)
        {
            ActionAt = _dateTime;
            Message = _message;
        }
    }
}
