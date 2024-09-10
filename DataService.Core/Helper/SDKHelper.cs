using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Services;
using DataService.Core.Entities;
using DataService.Core.Helper;
using DataService.Core.Models.AttMachine;
using DataService.Models.AttMachine;
using DataService.Settings;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;
using DataWorkerService.Models.Sheet;
using GoogleSheetsWrapper;
using Microsoft.Extensions.Logging;

namespace DataWorkerService.Helper
{
    public class SDKHelper
    {
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private GoogleApiAccount _account;
        private ILogger<SDKHelper> _logger;
        private JSONCredential _credential;
        private IRepository _repository;

        private static bool bIsConnected = false;
        private static int iMachineNumber = 1;
        private static int idwErrorCode = 0;
        private static int iDeviceTpye = 1;
        private Device _device;
        private List<Employee> _employees = [];
        private List<SheetHelper> _sheetsHelper = [];
        private List<SheetAppender> _appenders = [];
        IQueueSender _queueSender;
        bool _isTest = false;
        public SDKHelper(IServiceLocator locator, Device device, bool isTest = false)
        {
            _account = locator.Get<GoogleApiAccount>();
            _credential = locator.Get<JSONCredential>();
            _logger = locator.Get<ILogger<SDKHelper>>();
            _repository = locator.Get<IRepository>();
            _queueSender = locator.Get<IQueueSender>();
            _device = device;
            _isTest = isTest;
            sta_ConnectTCP();
            
        }

        public bool GetConnectState()
        {
            return bIsConnected;
        }

        public void SetConnectState(bool state)
        {
            _logger.LogInformation($"Connected state turn into {state}");
            bIsConnected = state;
        }

        public int GetMachineNumber()
        {
            return iMachineNumber;
        }

        public void SetMachineNumber(int Number)
        {
            iMachineNumber = Number;
        }

        public void TestRealTimeEvent()
        {
            axCZKEM1_OnAttTransactionEx("10001", 0, 1, 1, 2024, 12, 12, 12, 12, 12, 0);
        }

        public Result sta_ConnectTCP()
        {

            if (_isTest)
            {
                SetConnectState(true);
                _logger.LogInformation($"Connected successfully to device ${_device.Ip}!");
                sta_RegRealTime();
                return Result.Success();
            }

            if (_device == null)
            {
                _logger.LogError($"Device model given was null");

                return Result.Fail(-1, "Do not recognize device settings");// ip or port is null

            }

            axCZKEM1.SetCommPassword(Convert.ToInt32(_device.CommKey));

            if (bIsConnected)
            {
                axCZKEM1.Disconnect();
                SetConnectState(false);
                _logger.LogError($"Device was connected then disconnected");

                return Result.Fail(-2, "Disconnected"); //disconnect
            }
            
            _logger.LogInformation($"Connecting to device {_device.Ip}");
            if (axCZKEM1.Connect_Net(_device.Ip, Convert.ToInt32(_device.Port)))
            {
                SetConnectState(true);
                _logger.LogInformation($"Connected successfully to device ${_device.Ip}!");
                sta_RegRealTime();
                return Result.Success();
            }
            else
            {
                SetConnectState(false);
                axCZKEM1.GetLastError(ref idwErrorCode);
                _logger.LogError($"Refer to documentation for more details with errorCode={idwErrorCode}");

                return Result.Fail(idwErrorCode, $"Refer to documentation for more details with errorCode={idwErrorCode}");
            }
        }

        public void sta_DisConnect()
        {
            if (GetConnectState())
            {
                axCZKEM1.Disconnect();
                _logger.LogInformation($"Disconnected device {_device.Ip} successfully");
                return;
            }
            _logger.LogError($"Disconnected device {_device.Ip} failed !!!");
        }

        public Result sta_RegRealTime()
        {
            if (_isTest)
            {
                _employees = sta_getEmployees();
                InitSheetsHelper();
                this.axCZKEM1.OnAttTransactionEx += axCZKEM1_OnAttTransactionEx;
                return Result.Success();
            }


            if (!GetConnectState())
            {
                _logger.LogError("Register Real-Time Event fail because of unconnected device!");
                return Result.Fail(-1024);
            }

            
            if (axCZKEM1.RegEvent(GetMachineNumber(), 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            {
                //common interface
                //this.axCZKEM1.OnFinger += new zkemkeeper._IZKEMEvents_OnFingerEventHandler(axCZKEM1_OnFinger);
                //this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                //this.axCZKEM1.OnFingerFeature += new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnFingerFeature);
                //this.axCZKEM1.OnDeleteTemplate += new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnDeleteTemplate);
                //this.axCZKEM1.OnNewUser += new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
                //this.axCZKEM1.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
                //this.axCZKEM1.OnAlarm += new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
                //this.axCZKEM1.OnDoor += new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnDoor);

                //only for color device
                _employees = sta_getEmployees();
                InitSheetsHelper();
                this.axCZKEM1.OnAttTransactionEx += axCZKEM1_OnAttTransactionEx;
                //this.axCZKEM1.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);

                //only for black&white device
                //this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction);
                //this.axCZKEM1.OnWriteCard += new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
                //this.axCZKEM1.OnEmptyCard += new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
                //this.axCZKEM1.OnKeyPress += new zkemkeeper._IZKEMEvents_OnKeyPressEventHandler(axCZKEM1_OnKeyPress);
                //this.axCZKEM1.OnEnrollFinger += new zkemkeeper._IZKEMEvents_OnEnrollFingerEventHandler(axCZKEM1_OnEnrollFinger);


                return Result.Success();
            }
            axCZKEM1.GetLastError(ref idwErrorCode);

            if (idwErrorCode != 0)
            {
                return Result.Fail(idwErrorCode, "*RegEvent failed,ErrorCode: " + idwErrorCode.ToString());
            }


            return Result.Success("*No data");
        }

        private void InitSheetsHelper( )
        {
            try
            {
                foreach (var sheet in _device.Sheets)
                {
                    var sheetHelper = new SheetHelper<Record>(sheet.DocumentId, _account.ServiceAccountId, sheet.SheetName);
                    sheetHelper.Init(_credential.ToString());
                    var appender = new SheetAppender(sheetHelper);
                    _appenders.Add(appender);
                    _logger.LogInformation($"Init sheet sheetName={sheet.SheetName} documentId={sheet.DocumentId} Successfully!");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public List<Employee> sta_getEmployees()
        {
            if (!GetConnectState())
            {
                return [];
            }

            List<Employee> employees = [];

            string empnoStr = string.Empty;
            string name = string.Empty;
            string pwd = string.Empty;
            int pri = 0;
            bool enable = true;
            var cardNum = string.Empty;

            axCZKEM1.EnableDevice(iMachineNumber, false);
            try
            {
                axCZKEM1.ReadAllUserID(iMachineNumber);

                while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out empnoStr, out name, out pwd, out pri, out enable))
                {
                    cardNum = "";
                    if (axCZKEM1.GetStrCardNumber(out cardNum))
                    {
                        if (string.IsNullOrEmpty(cardNum))
                            cardNum = "";
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        int index = name.IndexOf("\0");
                        if (index > 0)
                        {
                            name = name.Substring(0, index);
                        }
                    }

                    Employee emp = new();
                    emp.Pin = empnoStr;
                    emp.Name = name;
                    emp.Privilege = pri;
                    emp.Password = pwd;
                    emp.CardNumber = cardNum;

                    employees.Add(emp);
                }
                _logger.LogInformation($"Loaded User Information Successfully with Count={employees.Count}");
            }
            catch(Exception ex) 
            {
                _logger.LogInformation(ex.Message);
            }
            finally
            {
                axCZKEM1.EnableDevice(iMachineNumber, true);
            }
            return employees;
        }

        private void axCZKEM1_OnAttTransactionEx(string EnrollNumber, int IsInValid, int attState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
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
                WorkCode = WorkCode
            };

            if (_isTest)
            {
                DataHelper.PublishData(_appenders, _repository, attRecord, new Employee(), _queueSender);
                return;
            }

            if (employee == null)
            {
                _logger.LogWarning($"Employee was not found with EnrollNumber={EnrollNumber}");
                _logger.LogInformation($"Discard to push data into GGSheet");
                return;
            }

            DataHelper.PublishData(_appenders, _repository, attRecord, employee, _queueSender);
        }

        private static void axCZKEM1_OnAttTransaction(int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second)
        {
            string time = Year + "-" + Month + "-" + Day + " " + Hour + ":" + Minute + ":" + Second;
            //gRealEventListBox.Items.Add("Verify OK.UserID=" + EnrollNumber.ToString() + " isInvalid=" + IsInValid.ToString() + " state=" + AttState.ToString() + " verifystyle=" + VerifyMethod.ToString() + " time=" + time);

            throw new NotImplementedException();
        }

    }
}
