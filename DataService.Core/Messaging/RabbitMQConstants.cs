using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Messaging
{
    public static class RabbitMQConstants
    {
        public static string ActionQueue = "DataService.ActionQueue";

        public static string HostName = "localhost";

        public static int Port = 5672;

        public static string EventQueue = "DataService.EventQueue";

        public static string DeviceEventQueue = "DataService.DeviceEventQueue";

        public static string EmployeeEventQueue = "DataService.EmployeeEventQueue";
    }
}
