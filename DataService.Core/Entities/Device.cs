﻿using CleanArchitecture.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Entities
{
    public class Device : BaseEntity
    {
        public string Ip { get; set; } = string.Empty;

        public string Port { get; set; } = string.Empty;

        public string CommKey { get; set; } = string.Empty;

        public List<Sheet> Sheets { get; set; } = [];
    }
}
