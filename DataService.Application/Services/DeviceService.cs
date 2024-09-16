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


namespace DataService.Application.Services
{
    public class DeviceService : GenericRepository<DeviceEntity>, IDeviceService
    {
        public DeviceService(AppDbContext context, ILogger<GenericRepository<DeviceEntity>> logger) : base(context, logger)
        {
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
                RabbitMQProducer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
                {
                    ActionType = ActionType.Added,
                    Data = device
                }.ToString());
            }

            return result;
        }

        public override async Task<Result> Update(Device entityToUpdate)
        {
            try
            {
                var sheetsInDb = await _context.Sheets.AsNoTracking().Where(item => item.DeviceId == entityToUpdate.Id).ToListAsync();
                dbSet.Attach(entityToUpdate);
                _context.Entry(entityToUpdate).State = EntityState.Modified;

                // added
                var added = entityToUpdate.Sheets.Where(item => item.Id == 0);
                foreach(var item in added)
                {
                    _context.Sheets.Entry(item).State = EntityState.Added;
                }

                // determine
                var items = entityToUpdate.Sheets.Where(item => item.Id != 0);
                foreach (var sheet in sheetsInDb)
                {
                    var item = items.FirstOrDefault(i => i.Id == sheet.Id);
                    if (item != null)
                    {
                        _context.Sheets.Entry(item).State = EntityState.Modified;
                    }

                    else
                    {
                        _context.Sheets.Entry(sheet).State = EntityState.Deleted;

                    }
                }
                await _context.SaveChangesAsync();

                RabbitMQProducer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
                {
                    ActionType = ActionType.Deleted,
                    Data = entityToUpdate
                }.ToString());

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(500, ex.Message);
            }
        }

        public override async Task<Result> Delete(int id)
        {
            var device = await GetById(id, "Sheets,Attendances");

            RabbitMQProducer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
            {
                ActionType = ActionType.Deleted,
                Data = device
            }.ToString());

            return Result.Success("Requested to Delete! Please wait....");
        }

        public async Task<Result> Connect(int deviceid)
        {
            var device = await GetById(deviceid);

            RabbitMQProducer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
            {
                ActionType = ActionType.Connect,
                Data = device
            }.ToString());

            return Result.Success("Requested to Connect! Please wait....");
        }
        public async Task<Result> Disconnect(int deviceid)
        {
            var device = await GetById(deviceid);
            RabbitMQProducer.SendMessage(RabbitMQConstants.DeviceEventQueue, new RabbitMQEvent<DeviceEntity>
            {
                ActionType = ActionType.Disconnect,
                Data = device
            }.ToString());

            return Result.Success("Requested to Disconnect! Please wait....");
        }
    }
}
