using DataService.Core.Contracts;
using DataService.Core.Interfaces;
using DataService.Infrastructure.Entities;
using DataWorkerService.Helper;
using DataWorkerService.Models;
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
        IServiceLocator _locator;

        public SDKService(IServiceLocator locator)
        {
            _logger = locator.Get<ILogger<SDKService>>();
            _repository = locator.Get<IGenericRepository<Device>>();
            _locator = locator;
        }

        public Result Add(Device device, bool connect = false)
        {
            var sdk = new SDKHelper(_locator, device);
            if (connect)
            {
                sdk.sta_ConnectTCP();
            }

            _sdks.Add(sdk);
            return Result.Success();
        }

        public List<SDKHelper> GetCurrentSDKs()
        {
            return _sdks;
        }

        public Result Remove(SDKHelper device)
        {
            return Result.Success();
        }

        public void DisconnectAll()
        {
            foreach(var sdk in _sdks)
            {
                _logger.LogInformation($"Disconnecting Device's IP: {sdk.GetDevice().Ip}");
                sdk.sta_DisConnect();
            }
        }

        public void Init()
        {
            var devices = _repository.Get(includeProperties: "Sheets");
            foreach(var device in devices)
            {
                _sdks.Add(new SDKHelper(_locator, device));
            }
        }

        public Result Remove(Device device)
        {
            foreach(var sdk in _sdks)
            {
                if(sdk.DeviceIP == device.Ip)
                {
                    sdk.sta_DisConnect();
                    return Result.Success();
                }
            }

            return Result.Fail(404, "Device not found");
        }
    }
}
