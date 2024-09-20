using DataService.Core.Contracts;
using DataService.Core.Interfaces;
using DataService.Core.Models.AttMachine;
using DataService.Core.Services;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataService.Settings;
using DataWorkerService.Helper;
using DataWorkerService.Models.Config;
using DataWorkerService.Models.Sheet;
using DataWorkerService.Models;
using GoogleSheetsWrapper;
using Microsoft.Extensions.Logging;
using BiometricDevices.NET.Concrete.ZKUFace800;
using Microsoft.EntityFrameworkCore;
using BiometricDevices.NET.Enums;


namespace DataService.Core.Helper
{
    public class SDKHelperNew
    {
        private readonly ZKUFace800Device _sdk = new();
        private readonly GoogleApiAccount _account;
        private readonly JSONCredential _credential;
        private readonly ILogger<SDKHelper> _logger;
        private readonly IGenericRepository<Attendance> _repository;
        private readonly IGenericRepository<Notification> _notifications;
        private readonly IQueueSender _queueSender;
        private readonly AppDbContext _context;

        private readonly Device _device;
        private readonly List<SheetAppender> _appenders = new();
        private List<ZKUFace800BiometricUser> _employees = new();

        private bool _disposed;
        private int iMachineNumber = 1;

        public SDKHelperNew(IServiceLocator locator, Device device)
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

        public bool IsConnected => _sdk.State.HasFlag(DeviceState.Connected);

        public DeviceState DeviceState => _sdk.State;

        public Device GetDevice() => _device;

        public List<ZKUFace800BiometricUser> Employees => _employees;

        public static Result Ping(Device device)
        {
            return Result.Success();
        }


        public int GetMachineNumber() => iMachineNumber;

        public void SetMachineNumber(int Number)
        {
            iMachineNumber = Number;
        }

        private Dictionary<ConnectionParameterKey, string> GetParams(Device device)
        {
            return new()
            {
                { ConnectionParameterKey.IP, device.Ip },
                { ConnectionParameterKey.Port, device.Port },
                { ConnectionParameterKey.Password, device.CommKey },

            };
        }


        public async Task<Result> ConnectTCP()
        {
            if (_device == null)
            {
                _logger.LogError("Device is null");
                return Result.Fail(-1, "Invalid device settings");
            }

            if (DeviceState.Connected.HasFlag(_sdk.State))
            {
                _logger.LogWarning("Device already connected");
                return Result.Fail(500, "Already connected");
            }

            _logger.LogInformation($"Device {DeviceIP}: Connecting...");

            var connected = await _sdk.ConnectAsync(ConnectionType.TCP, GetParams(_device));
            if (connected)
            {
                _employees.Add((ZKUFace800BiometricUser)await GetEmployees());
                InitializeSheetsHelper();
                _logger.LogInformation($"Device {DeviceIP}: Connected successfully!");
                await _sdk.InitializeAsync(iMachineNumber);
                _sdk.OnAttendanceRecorded += HandleOnAttendanceRecorded;

                return Result.Success();

            }
            
            _logger.LogError($"Device {DeviceIP}: Connected failed.");

            await _notifications.Insert(new Notification
            {
                Message = $"Device {DeviceIP}: Connected failed!",
                Success = false
            });
            return Result.Fail(500, "Connection failed");
        }
        private void HandleOnAttendanceRecorded(object sender, ZKUFace800AttendanceRecord attendanceRecord)
        {
            // Handle attendance record
            var user = _employees.FirstOrDefault(item => item.Id == attendanceRecord.UserId);
            var attRecord = new OnAttendanceTransactionRecord
            {
                UserId = attendanceRecord.UserId,
                AttState = (short)attendanceRecord.AttendanceState,
                VerifyMethod = (short)attendanceRecord.VerificationMethod,
                DateTimeRecord = attendanceRecord.Timestamp,
                IsInvalid = attendanceRecord.IsValid ? 0 : 1,
                WorkCode = 0,
                DeviceId = _device.Id
            };

            if (user == null)
            {
                _logger.LogWarning($"Employee was not found with EnrollNumber={attendanceRecord.UserId}");
                _logger.LogInformation($"Discard to push data into GGSheet");
                return;
            }

            var employee = new Employee
            {
                CardNumber = user.CardNumber,
                Name = user.FullName,
                Password = user.Password,
                Pin = user.Id,
                Privilege = (short)user.Privilege,
            };

            DataHelper.PublishData(_appenders, _repository, attRecord, employee, _queueSender);
        }

        public async Task Disconnect(bool isRemove = false)
        {
            if (DeviceState.Connected.HasFlag(_sdk.State))
            {
                var result = await _sdk.DisconnectAsync();
                if (result)
                {
                    _logger.LogInformation($"Device {DeviceIP}: Disconnected sucessfully");
                }
                else
                {
                    _logger.LogError($"Device {DeviceIP}: Disconnected failed.");

                }

                if (isRemove)
                {
                    _context.Devices.Where(item => item.Ip == DeviceIP).ExecuteDelete();
                    await _notifications.Insert(new Notification
                    {
                        Message = $"Device {DeviceIP}: Disconnected then Removed successfully",
                        Success = true
                    });
                }
                else
                {
                    _context.Devices.Where(item => item.Ip == DeviceIP).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, false));
                    await _notifications.Insert(new Notification
                    {
                        Message = $"Device {DeviceIP}: Disconnected sucessfully",
                        Success = true
                    });
                }
            }
            else
            {
                await _notifications.Insert(new Notification
                {
                    Message = $"Device {DeviceIP}: Cann't Disconnect due to unconnected.",
                    Success = false
                });
                _logger.LogWarning($"Device {DeviceIP}: Cann't Disconnect due to unconnected.");
                _context.Devices.Where(item => item.Ip == DeviceIP).ExecuteUpdate(setter => setter.SetProperty(i => i.IsConnected, false));
            }

        }

        //public Result RegisterRealTimeEvents()
        //{
        //    if (!_isConnected)
        //    {
        //        _logger.LogError("Failed to register real-time events. Device is not connected.");
        //        return Result.Fail(-1024, "Device not connected");
        //    }


        //    if (_zkem.RegEvent(GetMachineNumber(), 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
        //    {
        //        _employees = GetEmployees();
        //        InitializeSheetsHelper();
        //        _zkem.OnAttTransactionEx += HandleAttendanceTransaction;
        //        _zkem.OnDisConnected += () => _logger.LogInformation("Device disconnected");
        //        _zkem.OnConnected += () => _logger.LogInformation("Device connected");

        //        return Result.Success();
        //    }
        //    int errorCode = 1;
        //    _zkem.GetLastError(ref errorCode);
        //    return Result.Fail(errorCode, "Failed to register events");

        //}

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

        public async void Dispose()
        {
            var a = Dispose(true);
            GC.SuppressFinalize(this);
            await a;
        }

        protected virtual async Task Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger.LogInformation("Disconnecting device...");
                    await _sdk.DisconnectAsync();
                }
                _disposed = true;
            }
        }

        private ZKUFace800BiometricUser CreateUser(Employee employee)
        {
            return new ZKUFace800BiometricUser
            {
                IsActive = true,
                CardNumber = employee.CardNumber,
                FullName = employee.Name,
                Password = employee.Password,
                Privilege = UserPrivilege.CommonUser,
                Id = employee.Pin
            };
        }

        public async Task<Result> AddEmployeeAsync(Employee employee)
        {
            if (!DeviceState.Connected.HasFlag(_sdk.State))
            {
                _logger.LogError($"Device {DeviceIP}: Device was not connected then can not insert employee.");
                return Result.Fail(500);
            }
            var result = await _sdk.CreateUserAsync(CreateUser(employee));

            if (result)
            {
                _context.Employees.Add(employee);
                _context.DeviceEmployees.Add(new DeviceEmployee
                {
                    DeviceId = _device.Id,
                    EmployeeId = employee.Id,
                });
                await _notifications.Insert(new Notification
                {
                    Message = $"Device {DeviceIP}: Employee added successfully!",
                    Success = true
                });
                _logger.LogInformation($"Device {DeviceIP}: Employee added successfully!");

                return Result.Success();
            }

            await _notifications.Insert(new Notification
            {
                Message = $"Device {DeviceIP}: Employee added failed",
                Success = false
            });
            _logger.LogError($"Device {DeviceIP}: Employee added failed!");
            return Result.Fail(500, $"Device {DeviceIP}: Employee added failed!");

        }

        private async Task<IEnumerable<ZKUFace800BiometricUser>> GetEmployees()
        {
            return await _sdk.GetAllUsersAsync(); ;
        }
    }
}
