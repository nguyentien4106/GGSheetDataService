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

            _logger.LogInformation("Worker service is listening for messages...");

            return channel;
        }

        public void HandleDeviceEventQueue(object? sender, BasicDeliverEventArgs args)
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

            sdk.sta_DisConnect();
        }

        private void HandleConnectDevice(Device device)
        {
            _logger.LogInformation("HandleConnectDevice");

            var sdk = _sdkService.GetCurrentSDKs().FirstOrDefault(item => item.DeviceIP == device.Ip);

            if(sdk == null) 
            {
                _logger.LogError($"Didn't find any SDK initilized with Device's IP = {device.Ip}");
                return;
            }

            var result = sdk.sta_ConnectTCP();
            if (result.IsSuccess)
            {
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
            foreach (var sheet in device.Sheets)
            {
                _context.Sheets.Entry(sheet).State = EntityState.Deleted;
            }

            device.Attendances.Clear();
            _deviceRepository.Delete(device);
        }

        private void HandleAddNewDevice(Device device)
        {
            _sdkService.Add(device);
            _deviceRepository.Insert(device);
        }

    }
}
