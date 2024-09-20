using DataService.Core.Services;
using DataService.Core.Messaging;
using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DeviceEntity = DataService.Infrastructure.Entities.Device;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataService.Core.Settings;


namespace DataService.Application.Services
{
    public class DeviceService : GenericRepository<DeviceEntity>, IDeviceService
    {
        RabbitMQProducer _producer;

        public DeviceService(AppDbContext context, ILogger<GenericRepository<DeviceEntity>> logger, RabbitMQParams rabbit) : base(context, logger)
        {
            _producer = new(rabbit);
        }

        public override async Task<Result> Insert(DeviceEntity device)
        {
            var devices = await base.GetAsync();

            var existedIp = devices.FirstOrDefault(item => item.Ip == device.Ip);
            if (existedIp != null) 
            {
                return Result.Fail(400, "Device's Ip is existed in the system. Please add another device!");
            }

            var result = SDKHelper.Ping(device);

            if (result.IsSuccess)
            {
                _producer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
                {
                    ActionType = ActionType.Added,
                    Data = device
                }.ToString());
            }

            return result;
        }

        public override async Task<Result> Update(Device entityToUpdate)
        {
            _producer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
            {
                ActionType = ActionType.Modified,
                Data = entityToUpdate
            }.ToString());

            return Result.Success("Requested to Update! Please wait....");
        }

        public override async Task<Result> Delete(int id)
        {
            var device = await GetById(id, "Sheets");

            _producer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
            {
                ActionType = ActionType.Deleted,
                Data = device
            }.ToString());

            return Result.Success("Requested to Delete! Please wait....");
        }

        public async Task<Result> Connect(int deviceid)
        {
            var device = await GetById(deviceid);

            _producer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
            {
                ActionType = ActionType.Connect,
                Data = device
            }.ToString());

            return Result.Success("Requested to Connect! Please wait....");
        }
        public async Task<Result> Disconnect(int deviceid)
        {
            var device = await GetById(deviceid);
            _producer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
            {
                ActionType = ActionType.Disconnect,
                Data = device
            }.ToString());

            return Result.Success("Requested to Disconnect! Please wait....");
        }
    }
}
