using DataService.Core.Contracts;
using DataService.Core.Interfaces;
using DataService.Core.Models.Enum;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Services
{
    public class SDKService : ISDKService
    {
        public List<SDKHelper> _sdks = new();
        ILogger<SDKService> _logger;
        IGenericRepository<Device> _repository;
        IGenericRepository<Notification> _notifications;
        IServiceLocator _locator;
        AppDbContext _context;
        public SDKService(IServiceLocator locator)
        {
            _logger = locator.Get<ILogger<SDKService>>();
            _repository = locator.Get<IGenericRepository<Device>>();
            _notifications = locator.Get<IGenericRepository<Notification>>();
            _context = locator.Get<AppDbContext>();
            _locator = locator;
        }

        public void Init(bool connect = false)
        {
            var devices = _repository.Get(includeProperties: "Sheets");
            foreach (var device in devices)
            {
                _sdks.Add(new SDKHelper(_locator, device));
            }

            if (connect)
            {
                ConnectAll();
            }
        }

        public List<SDKHelper> GetCurrentSDKs() => _sdks;

        public Result Add(Device device, bool connect = false)
        {
            var sdk = new SDKHelper(_locator, device);
            _sdks.Add(sdk);
            _repository.Insert(device);

            _notifications.Insert(new Notification
            {
                Action = (short)NotificationAction.ServiceAdded,
                Message = $"Device ${device.Ip} added successfully!",
                Type = (short)NotificationType.Device,
                Success = true
            });
            if (connect)
            {
                var result = sdk.ConnectTCP();
                if (result.IsSuccess)
                {
                    _context.Devices.Where(item => item.Ip == device.Ip).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, true));
                    return Result.Success();
                }
                else
                {
                    return Result.Fail(503, "Can not connect");
                }
            }

            return Result.Success();
        }

        public Result Remove(Device device)
        {
            var item = _sdks.FirstOrDefault(item => item.DeviceIP == device.Ip);
            if (item == null)
            {
                return Result.Fail(404, "Device not found");
            }

            item.Disconnect();
            _sdks.Remove(item);
            foreach (var sheet in device.Sheets)
            {
                _context.Sheets.Entry(sheet).State = EntityState.Deleted;
            }

            _context.Devices.Entry(device).State = EntityState.Deleted;
            _context.SaveChanges();
            _notifications.Insert(new Notification
            {
                Action = (short)NotificationAction.ServiceRemoved,
                Message = $"Device {device.Ip} removed successfully!",
                Type = (short)NotificationType.Device,
                Success = true
            });
            return Result.Success();
        }

        public void DisconnectAll()
        {
            foreach(var sdk in _sdks)
            {
                _logger.LogInformation($"Disconnecting Device's IP: {sdk.GetDevice().Ip}");
                sdk.Disconnect();
            }
        }

        public void ConnectAll()
        {
            foreach (var sdk in _sdks)
            {
                sdk.ConnectTCP();
            }
        }

        public Result AddEmployee(Employee emp)
        {
            foreach(var sdk in _sdks)
            {
                if (sdk.IsConnected)
                {
                    var result = sdk.AddEmployee(emp);
                    if(result.IsSuccess )
                    {
                        _logger.LogInformation(result.Message);
                    }
                    else
                    {
                        _logger.LogError(result.Message);
                    }
                }
            }

            return Result.Success();
        }

        public Result RemoveEmployee(Employee employy)
        {
            throw new NotImplementedException();
        }

        public Result UpdateEmployee(Employee employy)
        {
            throw new NotImplementedException();
        }
    }
}
