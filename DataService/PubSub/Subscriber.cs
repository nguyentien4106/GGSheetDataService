using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.PubSub
{
    public class Subscriber(string _subscriberName)
    {
        public string Name { get; private set; } = _subscriberName;

        // This function subscribe to the events of the publisher
        public void Subscribe(Publisher p)
        {
            // register OnNotificationReceived with publisher event
            p.OnPublish += OnNotificationReceived;  // multicast delegate 

        }

        // This function unsubscribe from the events of the publisher
        public void Unsubscribe(Publisher p)
        {

            // unregister OnNotificationReceived from publisher
            p.OnPublish -= OnNotificationReceived;  // multicast delegate 
        }

        // It get executed when the event published by the Publisher
        protected virtual void OnNotificationReceived(Publisher p, NotificationEvent e)
        {

            Console.WriteLine("Hey " + Name + ", " + e.Message + " - " + p.Name + " at " + e.ActionAt);
        }
    }
}
