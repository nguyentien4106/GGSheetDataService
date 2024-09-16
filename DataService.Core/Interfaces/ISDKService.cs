﻿using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Interfaces
{
    public interface ISDKService
    {
        void Init();

        Result Add(Device device, bool connect = false);

        Result Remove(SDKHelper device);

        Result Remove(DataService.Infrastructure.Entities.Device device);

        List<SDKHelper> GetCurrentSDKs();

        void DisconnectAll();
    }
}
