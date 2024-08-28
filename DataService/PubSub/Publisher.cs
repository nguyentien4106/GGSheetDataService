using DataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.PubSub
{
    public class Publisher(string _publisherName, int _notificationInterval)
    {
        public string Name { get; set; } = _publisherName;

        public int NotificationInterval { get; set; } = _notificationInterval;

        public delegate void Notify(Publisher p, NotificationEvent e);

        public event Notify OnPublish;

        public void Publish()
        {
            while (true)
            {

                // fire event after certain interval
                Thread.Sleep(NotificationInterval);

                if (OnPublish != null)
                {
                    NotificationEvent data = new NotificationEvent(DateTime.Now, "New Notification Arrived from");
                    OnPublish(this, data);
                }
                Thread.Yield();
            }
        }

        public void Publish(EventData data)
        {
            while (true)
            {

                // fire event after certain interval
                Thread.Sleep(NotificationInterval);

                if (OnPublish != null)
                {
                    OnPublish(this, new NotificationEvent(DateTime.Now, "hello"));
                }
                Thread.Yield();
            }
        }
    }
}
