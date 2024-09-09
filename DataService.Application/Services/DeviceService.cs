using CleanArchitecture.Infrastructure.Data;
using DataService.Application.Repositories;
using DataService.Core.Entities;
using DataWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Application.Services
{
    public class DeviceService : GenericRepository<Device>, IDeviceService
    {
        public DeviceService(AppDbContext context) : base(context)
        {
            
        }

        public override async Task<Result> Insert(Device device)
        {
            var devices = await base.Get();

            var existedIp = devices.FirstOrDefault(item => item.Ip == device.Ip);
            if (existedIp != null) 
            {
                return Result.Fail(400, "Device's IP is existed in the system. Please add another device!");
            }

            return await base.Insert(device);
        }
    }
}
