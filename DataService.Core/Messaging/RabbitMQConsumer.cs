using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using DataService.Infrastructure.Entities;
using Newtonsoft.Json;
using DataService.Core.Interfaces;
using DataWorkerService.Helper;
using DataService.Core.Services;
using DataService.Core.Contracts;
using DataService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataService.Core.Messaging
{
    public class RabbitMQConsumer
    {
        ILogger<RabbitMQConsumer> _logger;
        ISDKService _sdkService;
        IServiceLocator _locator;
        IGenericRepository<Device> _deviceRepository;
        AppDbContext _context;

        public RabbitMQConsumer(IServiceLocator locator)
        {
            _locator = locator;
            _logger = locator.Get<ILogger<RabbitMQConsumer>>();
            _sdkService = locator.Get<ISDKService>();
            _deviceRepository = locator.Get<IGenericRepository<Device>>();
            _context = locator.Get<AppDbContext>();
        }

        public IModel StartListening(string queue)
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQConstants.HostName, Port = RabbitMQConstants.Port };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queue,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += RabbitMQHandler.GetHandler(queue);

            channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

            _logger.LogInformation("Worker service is listening for messages...");

            return channel;
        }

        public IModel StartListeningOnDeviceQueue()
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQConstants.HostName, Port = RabbitMQConstants.Port };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: RabbitMQConstants.DeviceEventQueue,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += HandleDeviceEventQueue;

            channel.BasicConsume(queue: RabbitMQConstants.DeviceEventQueue, autoAck: true, consumer: consumer);

            return channel;
        }

        public IModel StartListeningOnEmployeesQueue()
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQConstants.HostName, Port = RabbitMQConstants.Port };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: RabbitMQConstants.EmployeeEventQueue,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += HandleEmployeeEventQueue;

            channel.BasicConsume(queue: RabbitMQConstants.EmployeeEventQueue, autoAck: true, consumer: consumer);

            return channel;
        }

        private void HandleEmployeeEventQueue(object? sender, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventObj = JsonConvert.DeserializeObject<RabbitMQEvent<Employee>>(message);
            _logger.LogInformation("Received message: {0}", message);
            if (eventObj == null) return;

            switch (eventObj.ActionType)
            {
                case ActionType.Added:
                    break;
                case ActionType.Deleted:
                    break;
                case ActionType.Modified:
                    break;
                case ActionType.Connect:
                    break;
                case ActionType.Disconnect:
                    break;
                default:
                    break;
            }
        }

        private void HandleDeviceEventQueue(object? sender, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventObj = JsonConvert.DeserializeObject<RabbitMQEvent<Device>>(message);
            _logger.LogInformation("Received message: {0}", message);
            if (eventObj == null) return;

            switch (eventObj.ActionType)
            {
                case ActionType.Added:
                    HandleAddNewDevice(eventObj.Data);
                    break;
                case ActionType.Deleted:
                    HandleDeleteDevice(eventObj.Data);
                    break;
                case ActionType.Modified:
                    HandleModifiedDevice(eventObj.Data);
                    break;
                case ActionType.Connect:
                    HandleConnectDevice(eventObj.Data);
                    break;
                case ActionType.Disconnect:
                    HandleDisconnectDevice(eventObj.Data);
                    break;
                default:
                    break;
            }
        }

        private void HandleModifiedDevice(Device device)
        {
            _logger.LogInformation("HandleModifiedDevice");

            var sdk = _sdkService.GetCurrentSDKs().FirstOrDefault(item => item.DeviceIP == device.Ip);
            if (sdk == null)
            {
                _logger.LogError($"Didn't find any SDK initilized with Device's IP = {device.Ip}");
                return;
            }

            var sheets = _context.Sheets.Where(item => item.DeviceId == device.Id);
            foreach (var sheet in sheets)
            {
                _context.Entry(sheet).State = EntityState.Deleted;
            }

            foreach(var sheet in device.Sheets)
            {
                sheet.Id = 0;
                sheet.DeviceId = device.Id;
                _context.Sheets.Entry(sheet).State = EntityState.Added;
            }
            
            _context.SaveChanges();

            _sdkService.Remove(device);
            _sdkService.Add(device, true);
        }

        private void HandleDisconnectDevice(Device device)
        {
            _logger.LogInformation("HandleDisconnectDevice");
            var sdk = _sdkService.GetCurrentSDKs().FirstOrDefault(item => item.DeviceIP == device.Ip);

            if (sdk == null)
            {
                _logger.LogError($"Didn't find any SDK initilized with Device's IP = {device.Ip}");
                return;
            }

            sdk.Disconnect();
            _context.Devices.Where(item => item.Ip == device.Ip).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, false));
        }

        private void HandleConnectDevice(Device device)
        {
            _logger.LogInformation("HandleConnectDevice");

            var sdk = _sdkService.GetCurrentSDKs().FirstOrDefault(item => item.DeviceIP == device.Ip);

            if(sdk == null) 
            {
                _logger.LogError($"Didn't find any SDK initilized with Device's IP = {device.Ip}, Started to initial SDK with this IP.");
                _sdkService.Add(device, true);
                return;
            }

            var result = sdk.ConnectTCP();
            if (result.IsSuccess)
            {
                _context.Devices.Where(item => item.Ip == device.Ip).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, true));
                _logger.LogInformation($"Connected Successfully! Device's IP = {device.Ip}");
            }
            else
            {
                _logger.LogError($"Can not connect to device because of {result.Message}");
            }
        }

        private void HandleDeleteDevice(Device device)
        {
            _sdkService.Remove(device);
        }

        private void HandleAddNewDevice(Device device)
        {
            _sdkService.Add(device);
        }

    }
}
