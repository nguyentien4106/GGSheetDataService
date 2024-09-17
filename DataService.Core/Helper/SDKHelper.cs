using DataService.Core.Interfaces;
using DataService.Core.Services;
using DataService.Core.Contracts;
using DataService.Core.Helper;
using DataService.Core.Models.AttMachine;
using DataService.Infrastructure.Entities;
using DataService.Models.AttMachine;
using DataService.Settings;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;
using DataWorkerService.Models.Sheet;
using GoogleSheetsWrapper;
using Microsoft.Extensions.Logging;
using BiometricDevices.NET.Abstract;
using BiometricDevices.NET.Concrete.ZKUFace800;
using BiometricDevices.NET.Enums;
using zkemkeeper;

namespace DataWorkerService.Helper
{
    public class SDKHelper : IDisposable
    {
        private readonly CZKEMClass _zkem = new();
        private readonly GoogleApiAccount _account;
        private readonly JSONCredential _credential;
        private readonly ILogger<SDKHelper> _logger;
        private readonly IGenericRepository<Attendance> _repository;
        private readonly IQueueSender _queueSender;

        private readonly Device _device;
        private readonly List<Employee> _employees = new();
        private readonly List<SheetAppender> _appenders = new();

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
            _device = device ?? throw new ArgumentNullException(nameof(device));
        }

        public string DeviceIP => _device.Ip;

        public bool IsConnected => _device.IsConnected;

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

        public bool GetConnectState() => _isConnected;

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

        //public void TestRealTimeEvent()
        //{
        //    var random = new Random();
        //    axCZKEM1_OnAttTransactionEx("-1", random.Next(0, 1), random.Next(0, 5), random.Next(0, 14), 2024, 12, 12, 12, 12, 12, 0);
        //}

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
                //Disconnect();
                return Result.Success("Already connected");
            }

            _logger.LogInformation($"Connecting to {_device.Ip}");
            _zkem.SetCommPassword(int.Parse(_device.CommKey));

            if (_zkem.Connect_Net(_device.Ip, int.Parse(_device.Port)))
            {
                SetConnectState(true);
                _logger.LogInformation($"Connected to {_device.Ip}");
                RegisterRealTimeEvents();
                return Result.Success();
            }
            int errorCode = 1;
            _zkem.GetLastError(ref errorCode);
            _logger.LogError($"Connection failed with error code {errorCode}");
            return Result.Fail(errorCode, "Connection failed");
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _logger.LogInformation($"Disconnecting {_device.Ip}");
                _zkem.Disconnect();
                SetConnectState(false);
            }
            else
            {
                _logger.LogWarning("Device already disconnected");
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
                _employees.AddRange(GetEmployees());
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
                    _logger.LogInformation($"Initialized sheet {sheet.SheetName} (ID: {sheet.DocumentId})");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error initializing sheet: {ex.Message}");
                }
            }
        }

        public List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();

            if (!_isConnected)
            {
                return employees;
            }

            _zkem.EnableDevice(1, false);
            try
            {
                _zkem.ReadAllUserID(1);
                while (_zkem.SSR_GetAllUserInfo(1, out var empNo, out var name, out var pwd, out var pri, out var enable))
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
                _zkem.EnableDevice(1, true);
            }

            return employees;
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
                _logger.LogError($"Device {DeviceIP} was not connected");
                return Result.Fail(500);
            }

            //int iPrivilege = cbPrivilege.SelectedIndex;

            bool bFlag = false;
        
            int iPIN2Width = 0;
            int iIsABCPinEnable = 0;
            int iT9FunOn = 0;
            string strTemp = "";
            _zkem.GetSysOption(GetMachineNumber(), "~PIN2Width", out strTemp);
            iPIN2Width = Convert.ToInt32(strTemp);
            _zkem.GetSysOption(GetMachineNumber(), "~IsABCPinEnable", out strTemp);
            iIsABCPinEnable = Convert.ToInt32(strTemp);
            _zkem.GetSysOption(GetMachineNumber(), "~T9FunOn", out strTemp);
            iT9FunOn = Convert.ToInt32(strTemp);


            if (employee.Pin.Length > iPIN2Width)
            {
                _logger.LogError("*User ID error! The max length is " + iPIN2Width.ToString());
                return Result.Fail(501, "*User ID error! The max length is " + iPIN2Width.ToString());
            }

            if (iIsABCPinEnable == 0 || iT9FunOn == 0)
            {
                if (employee.Pin.StartsWith("0"))
                {
                    _logger.LogError("PIN can not start with 0");

                    return Result.Fail(501, "PIN can not start with 0");
                }

                if (!employee.Pin.All(char.IsDigit))
                {
                    _logger.LogError("*User ID error! User ID only support digital");
                    return Result.Fail(501, "*User ID error! User ID only support digital");

                }
               
            }

            int idwErrorCode = 0;

            _zkem.EnableDevice(iMachineNumber, false);
            _zkem.SetStrCardNumber(employee.CardNumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
            if (_zkem.SSR_SetUserInfo(iMachineNumber, employee.Pin.Trim(), employee.Name.Trim(), employee.Password.Trim(), employee.Privilege, true))//upload the user's information(card number included)
            {
                _logger.LogInformation("Set user information successfully");
            }
            else
            {
                _zkem.GetLastError(ref idwErrorCode);
                _logger.LogError("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
                return Result.Fail(idwErrorCode, "*Operation failed,ErrorCode=" + idwErrorCode.ToString());

            }
            _zkem.RefreshData(iMachineNumber);//the data in the device should be refreshed
            _zkem.EnableDevice(iMachineNumber, true);

            return Result.Success();

        }
    }

}
