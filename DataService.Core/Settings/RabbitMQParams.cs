﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Settings
{
    public class RabbitMQParams
    {
        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DeviceQueue { get; set; }

        public string EmployeeQueue { get; set; }
    }
}
