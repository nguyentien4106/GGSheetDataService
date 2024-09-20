using DataService.Core.Interfaces;
using DataService.Core.Services;
using DataService.Core.Contracts;
using DataService.Core.Helper;
using DataService.Core.Models.AttMachine;
using DataService.Infrastructure.Entities;
using DataService.Settings;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;
using DataWorkerService.Models.Sheet;
using GoogleSheetsWrapper;
using Microsoft.Extensions.Logging;
using DataService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataWorkerService.Helper
{
    public class SDKHelper : IDisposable
    {
        private readonly zkemkeeper.CZKEMClass _zkem = new();
        private readonly GoogleApiAccount _account;
        private readonly JSONCredential _credential;
        private readonly ILogger<SDKHelper> _logger;
        private readonly IGenericRepository<Attendance> _repository;
        private readonly IGenericRepository<Notification> _notifications;
        private readonly IQueueSender _queueSender;
        private readonly AppDbContext _context;

        private readonly Device _device;
        private readonly List<SheetAppender> _appenders = new();
        private List<Employee> _employees = new();

        private bool _isConnected;
        private bool _disposed;
        private int iMachineNumber = 1;

        public SDKHelper(IServiceLocator locator, Device device)
        {
            _account = locator.Get<GoogleApiAccount>();
            _credential = locator.Get<JSONCredential>();
            _logger = locator.Get<ILogger<SDKHelper>>();
            _repository = locator.Get<IGenericRepository<Attendance>>();
            _queueSender = locator.Get<IQueueSender>();
            _notifications = locator.Get<IGenericRepository<Notification>>();
            _context = locator.Get<AppDbContext>();

            _device = device ?? throw new ArgumentNullException(nameof(device));
        }

        public string DeviceIP => _device.Ip;

        public bool IsConnected => _device.IsConnected;

        public List<Employee> Employees => _employees;

        public static Result Ping(Device device)
        {
            var ping = new zkemkeeper.CZKEMClass();
            return Result.Success();
            if (ping.Connect_Net(device.Ip, Int32.Parse(device.Port)))
            {
                ping.Disconnect();
                return Result.Success();
            }

            return Result.Fail(503, "Connection Time out");
        }

        public Device GetDevice() => _device;

        public void SetConnectState(bool state)
        {
            _isConnected = state;
            _logger.LogInformation($"Connected state turn into {state}");
        }

        public int GetMachineNumber() => iMachineNumber;

        public void SetMachineNumber(int Number)
        {
            iMachineNumber = Number;
        }

        public Result ConnectTCP()
        {
            if (_device == null)
            {
                _logger.LogError("Device is null");
                SetConnectState(false);
                return Result.Fail(-1, "Invalid device settings");
            }

            if (_isConnected)
            {
                _logger.LogWarning("Device already connected");
                return Result.Success("Already connected");
            }

            _logger.LogInformation($"Device {DeviceIP}: Connecting...");
            _zkem.SetCommPassword(int.Parse(_device.CommKey));

            if (_zkem.Connect_Net(_device.Ip, int.Parse(_device.Port)))
            {
                _logger.LogInformation($"Device {DeviceIP}: Connected successfully!");
                SetConnectState(true);
                RegisterRealTimeEvents();

                _notifications.Insert(new Notification {
                    Message = $"Device {DeviceIP}: Connected successfully!",
                    Success = true
                });

                _context.Devices.Where(item => item.Ip == DeviceIP).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, true));

                return Result.Success();
            }

            int errorCode = 1;
            _zkem.GetLastError(ref errorCode);
            _logger.LogError($"Device {DeviceIP}: Connected failed with error code {errorCode}");
           
            _notifications.Insert(new Notification
            {
                Message = $"Device {DeviceIP}: Connected failed!",
                Success = false
            });
            return Result.Fail(errorCode, "Connection failed");
        }

        public void Disconnect(bool isRemove = false)
        {
            if (_isConnected)
            {
                _zkem.Disconnect();
                SetConnectState(false);
                _logger.LogInformation($"Device {DeviceIP}: Disconnected sucessfully");

                if (isRemove)
                {
                    _context.Devices.Where(item => item.Ip == DeviceIP).ExecuteDelete();
                    _notifications.Insert(new Notification
                    {
                        Message = $"Device {DeviceIP}: Disconnected then Removed successfully",
                        Success = true
                    });
                }
                else
                {
                    _context.Devices.Where(item => item.Ip == DeviceIP).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, false));
                    _notifications.Insert(new Notification
                    {
                        Message = $"Device {DeviceIP}: Disconnected sucessfully",
                        Success = true
                    });
                }
                

            }
            else
            {
                _notifications.Insert(new Notification
                {
                    Message = $"Device {DeviceIP}: Cann't Disconnect due to unconnected.",
                    Success = false
                });
                _logger.LogWarning($"Device {DeviceIP}: Cann't Disconnect due to unconnected.");
                _context.Devices.Where(item => item.Ip == DeviceIP).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, false));
            }

        }

        public Result RegisterRealTimeEvents()
        {
            if (!_isConnected)
            {
                _logger.LogError("Failed to register real-time events. Device is not connected.");
                return Result.Fail(-1024, "Device not connected");
            }


            if (_zkem.RegEvent(GetMachineNumber(), 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            {
                _employees = GetEmployees();
                InitializeSheetsHelper();
                _zkem.OnAttTransactionEx += HandleAttendanceTransaction;
                _zkem.OnDisConnected += () => _logger.LogInformation("Device disconnected");
                _zkem.OnConnected += () => _logger.LogInformation("Device connected");

                return Result.Success();
            }
            int errorCode = 1;
            _zkem.GetLastError(ref errorCode);
            return Result.Fail(errorCode, "Failed to register events");

        }

        private void InitializeSheetsHelper()
        {
            foreach (var sheet in _device.Sheets)
            {
                try
                {
                    var sheetHelper = new SheetHelper<Record>(sheet.DocumentId, _account.ServiceAccountId, sheet.SheetName);
                    sheetHelper.Init(_credential.ToString());
                    _appenders.Add(new SheetAppender(sheetHelper));
                    _logger.LogInformation($"Device {DeviceIP}: Initialized sheet {sheet.SheetName} (ID: {sheet.DocumentId}) successfully.");
                    _notifications.Insert(new Notification
                    {
                        Message = $"Device {DeviceIP}: Initialized sheet {sheet.SheetName} (ID: {sheet.DocumentId}) successfully.",
                        Success = true,
                    });
                }
                catch (Exception ex)
                {
                    _notifications.Insert(new Notification
                    {
                        Message = $"Device {DeviceIP}: Initialized sheet {sheet.SheetName} (ID: {sheet.DocumentId}) failed because ${ex.Message}",
                        Success = true,
                    });
                    _logger.LogError($"Device {DeviceIP}: Initialized sheet {sheet.SheetName} (ID: {sheet.DocumentId}) failed.");
                }
            }
        }

        private void HandleAttendanceTransaction(string EnrollNumber, int IsInValid, int attState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {
            var date = new DateTime(Year, Month, Day, Hour, Minute, Second);
            var employee = _employees.FirstOrDefault(item => item.Pin == EnrollNumber);
            var attRecord = new OnAttendanceTransactionRecord
            {
                UserId = EnrollNumber,
                AttState = attState,
                VerifyMethod = VerifyMethod,
                DateTimeRecord = date,
                IsInvalid = IsInValid,
                WorkCode = WorkCode,
                DeviceId = _device.Id
            };

            if (employee == null)
            {
                _logger.LogWarning($"Employee was not found with EnrollNumber={EnrollNumber}");
                _logger.LogInformation($"Discard to push data into GGSheet");
                return;
            }

            DataHelper.PublishData(_appenders, _repository, attRecord, employee, _queueSender);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger.LogInformation("Disconnecting device...");
                    _zkem.Disconnect();
                }
                _disposed = true;
            }
        }

        public Result AddEmployee(Employee employee)
        {
            if (!_isConnected)
            {
                _logger.LogError($"Device {DeviceIP}: Device was not connected then can not insert employee.");
                return Result.Fail(500);
            }

            int idwErrorCode = 0;

            _zkem.EnableDevice(iMachineNumber, false);
            _zkem.SetStrCardNumber(employee.CardNumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
            if (_zkem.SSR_SetUserInfo(iMachineNumber, employee.Pin.Trim(), employee.Name.Trim(), employee.Password.Trim(), employee.Privilege, true))//upload the user's information(card number included)
            {
                _context.Employees.Add(employee);
                _context.DeviceEmployees.Add(new DeviceEmployee
                {
                    DeviceId = _device.Id,
                    EmployeeId = employee.Id,
                });
                _notifications.Insert(new Notification
                {
                    Message = $"Device {DeviceIP}: Employee added successfully!",
                    Success = true
                });
                _logger.LogInformation($"Device {DeviceIP}: Employee added successfully!");
            }
            else
            {
                _zkem.GetLastError(ref idwErrorCode);

                _notifications.Insert(new Notification
                {
                    Message = $"Device {DeviceIP}: Employee added failed! ErrorCode = {idwErrorCode}",
                    Success = false
                });
                _logger.LogError($"Device {DeviceIP}: Employee added failed!");
                return Result.Fail(idwErrorCode, $"Device {DeviceIP}: Employee added failed!");

            }
            _zkem.RefreshData(iMachineNumber);//the data in the device should be refreshed
            _zkem.EnableDevice(iMachineNumber, true);

            return Result.Success();

        }

        private List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();

            if (!_isConnected)
            {
                return employees;
            }

            _zkem.EnableDevice(iMachineNumber, false);
            try
            {
                _zkem.ReadAllUserID(iMachineNumber);
                while (_zkem.SSR_GetAllUserInfo(iMachineNumber, out var empNo, out var name, out var pwd, out var pri, out var enable))
                {
                    var cardNum = _zkem.GetStrCardNumber(out var cn) ? cn : string.Empty;
                    employees.Add(new Employee
                    {
                        Pin = empNo,
                        Name = name.TrimEnd('\0'),
                        Privilege = pri,
                        Password = pwd,
                        CardNumber = cardNum
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving employees: {ex.Message}");
            }
            finally
            {
                _zkem.EnableDevice(iMachineNumber, true);
            }

            return employees;
        }

    }

}
