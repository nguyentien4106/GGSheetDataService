using CleanArchitecture.Core.Services;
using DataService.Core.Repositories;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace DataService.Application.Services
{
    public class DeviceService : GenericRepository<Device>, IDeviceService
    {
        IServiceLocator _serviceLocator;
        SDKHelper _sdk;
        public DeviceService(AppDbContext context, IServiceLocator locator, ILogger<GenericRepository<Device>> logger) : base(context, logger)
        {
            _serviceLocator = locator;
        }

        public override async Task<Result> Insert(Device device)
        {
            var devices = await base.GetAsync();

            var existedIp = devices.FirstOrDefault(item => item.Ip == device.Ip);
            if (existedIp != null) 
            {
                return Result.Fail(400, "Device's IP is existed in the system. Please add another device!");
            }

            using var sdk = new SDKHelper(_serviceLocator, device);
            if (sdk.GetConnectState())
            {
                var result = await base.Insert(device);
                sdk.sta_DisConnect();
                return result;
            }
            

            return Result.Fail(503, "Can not connect to device! Please check and correct the information !");
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
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(500, ex.Message);
            }
        }

        public override async Task<Result> Delete(int id)
        {
            var entity = await GetById(id, "Sheets");

            foreach(var sheet in entity.Sheets)
            {
                _context.Sheets.Entry(sheet).State = EntityState.Deleted;
            }

            return await base.Delete(entity);
        }

        public async Task<Result> Connect(int deviceid)
        {
            var device = await GetById(deviceid);

            using var sdk = new SDKHelper(_serviceLocator, device);
            var connected = sdk.GetConnectState();
            if (connected)
            {
                device.IsConnected = true;
                return await Update(device);
            }

            return Result.Fail(500, "Can not connect");
        }
        public async Task<Result> Disconnect(int deviceid)
        {
            var device = await GetById(deviceid);
            device.IsConnected = false;

            return await Update(device);
        }
    }
}
