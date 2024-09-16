using DataService.Core.Interfaces;
using DataService.Infrastructure.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Messaging
{
    public class RabbitMQHandler
    {
        ISDKService _sdkService;
        public RabbitMQHandler(ISDKService sdkService)
        {
            _sdkService = sdkService;
        }
        public static EventHandler<BasicDeliverEventArgs> GetHandler(string key)
        {
            Console.WriteLine($"Queue Name: {key}");
            var handlers = new Dictionary<string, EventHandler<BasicDeliverEventArgs>>()
            {
                { RabbitMQConstants.EventQueue, HandleEventQueue },
                { RabbitMQConstants.ActionQueue, HandleActionQueue },
            };
            Console.WriteLine($"Queue Name: {handlers[key]}");

            return handlers.ContainsKey(key) ? handlers[key] : handlers[""];
        }

        private static void HandleEventQueue(object? sender, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventObj = JsonConvert.DeserializeObject<RabbitMQEvent<Device>>(message);
            Console.WriteLine(message);
        }

        private static void HandleActionQueue(object? sender, BasicDeliverEventArgs args)
        {

        }

        public void HandleDeviceEventQueue(object? sender, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventObj = JsonConvert.DeserializeObject<RabbitMQEvent<Device>>(message);

            if (eventObj == null) return;

            switch (eventObj.ActionType)
            {
                case ActionType.Added:
                    HandleAddNewDevice(eventObj.Data);
                    break;
                case ActionType.Deleted:
                    break;
                case ActionType.Modified:
                    break;
                default:
                    break;
            }
        }

        private void HandleAddNewDevice(Device device)
        {
            _sdkService.Remove(device);
        }
    }
}
