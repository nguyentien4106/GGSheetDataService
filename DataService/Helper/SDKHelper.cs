using System;
using System.Collections.Generic;
using System.Text;
using DataService.Settings;
using DataWorkerService.Models;
using DataWorkerService.Models.Config;
using DataWorkerService.Models.Sheet;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using GoogleSheetsWrapper;
using zkemkeeper;

namespace DataWorkerService.Helper
{
    public class SDKHelper(GoogleApiAccount account, JSONCredential credential)
    {
        public CZKEMClass axCZKEM1 = new CZKEMClass();
        private GoogleApiAccount _account = account;
        private JSONCredential _credential = credential;
        private static bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private static int iMachineNumber = 1;
        private static int idwErrorCode = 0;
        private static int iDeviceTpye = 1;
        private List<Device> _devices = [];

        #region ConnectDevice

        public bool GetConnectState()
        {
            return bIsConnected;
        }

        public void SetConnectState(bool state)
        {
            bIsConnected = state;
            //connected = state;
        }

        public int GetMachineNumber()
        {
            return iMachineNumber;
        }

        public void SetMachineNumber(int Number)
        {
            iMachineNumber = Number;
        }

        public int sta_ConnectTCP(string ip, string port, string commKey, Device device = null)
        {
            if (ip == "" || port == "" || commKey == "")
            {
                return -1;// ip or port is null
            }

            if (device == null)
            {
                return -1;
            }
            _devices.Add(device);

            if (Convert.ToInt32(port) <= 0 || Convert.ToInt32(port) > 65535)
            {
                return -1;
            }

            if (Convert.ToInt32(commKey) < 0 || Convert.ToInt32(commKey) > 999999)
            {
                return -1;
            }

            int idwErrorCode = 0;

            axCZKEM1.SetCommPassword(Convert.ToInt32(commKey));

            if (bIsConnected)
            {
                axCZKEM1.Disconnect();
                //sta_UnRegRealTime();
                SetConnectState(false);
                //connected = false;
                return -2; //disconnect
            }

            if (axCZKEM1.Connect_Net(ip, Convert.ToInt32(port)))
            {
                SetConnectState(true);
                //sta_RegRealTime(lblOutputInfo);

                //get Biotype

                return 1;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                return idwErrorCode;
            }
        }

        public Result sta_ConnectTCP(Device model, Device device)
        {
            if (!model.IsValid())
            {
                return Result.Fail(-1, "ConnectTCPModel is not valid");// ip or port is null
            }
            if (device == null)
            {
                return Result.Fail(-1, "settings is not valid");// ip or port is null

            }
            _devices.Add(device);

            if (Convert.ToInt32(model.Port) <= 0 || Convert.ToInt32(model.Port) > 65535)
            {
                return Result.Fail(-1, "ConnectTCPModel is not valid");// ip or port is null

            }

            if (Convert.ToInt32(model.CommKey) < 0 || Convert.ToInt32(model.CommKey) > 999999)
            {
                return Result.Fail(-1, "ConnectTCPModel is not valid");// ip or port is null

            }

            int idwErrorCode = 0;

            axCZKEM1.SetCommPassword(Convert.ToInt32(model.CommKey));

            if (bIsConnected)
            {
                axCZKEM1.Disconnect();
                SetConnectState(false);

                return Result.Fail(-2, "Disconnected"); //disconnect
            }

            if (axCZKEM1.Connect_Net(model.IP, Convert.ToInt32(model.Port)))
            {
                SetConnectState(true);

                return Result.Success();
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                return Result.Fail(idwErrorCode, "Refer to documentation for more details");

            }
        }

        public void sta_DisConnect()
        {
            if (GetConnectState())
            {
                axCZKEM1.Disconnect();
            }
        }

        #endregion

        public Result sta_RegRealTime()
        {
            if (GetConnectState() == false)
            {
                return Result.Fail(-1024);
            }

            int ret = 0;

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
                this.axCZKEM1.OnAttTransactionEx += axCZKEM1_OnAttTransactionEx;
                //this.axCZKEM1.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);

                //only for black&white device
                this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(RealTimeEventHelper.axCZKEM1_OnAttTransaction);
                //this.axCZKEM1.OnWriteCard += new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
                //this.axCZKEM1.OnEmptyCard += new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
                //this.axCZKEM1.OnKeyPress += new zkemkeeper._IZKEMEvents_OnKeyPressEventHandler(axCZKEM1_OnKeyPress);
                //this.axCZKEM1.OnEnrollFinger += new zkemkeeper._IZKEMEvents_OnEnrollFingerEventHandler(axCZKEM1_OnEnrollFinger);


                return Result.Success();
            }
            axCZKEM1.GetLastError(ref idwErrorCode);

            if (idwErrorCode != 0)
            {
                //lblOutputInfo.Items.Add("*RegEvent failed,ErrorCode: " + idwErrorCode.ToString());
                return Result.Fail(idwErrorCode, "*RegEvent failed,ErrorCode: " + idwErrorCode.ToString());
            }


            return Result.Success("*No data");
        }

        private void axCZKEM1_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {

            foreach (var device in _devices)
            {

                foreach (var sheet in device.Sheets)
                {
                    var sheetHelper = new SheetHelper<Record>(
                            // https://docs.google.com/spreadsheets/d/<SPREADSHEET_ID_IS_HERE>/edit#gid=0
                            sheet.DocumentId,
                            // The email for the service account you created
                            _account.ServiceAccountId,
                            // the name of the tab you want to access, leave blank if you want the default first tab
                            sheet.SheetName);

                    sheetHelper.Init(_credential.ToString());
                    var appender = new SheetAppender(sheetHelper);
                    appender.AppendRow(
                    [
                        EnrollNumber,
                        $"{IsInValid}",
                        $"{AttState}",
                        $"{WorkCode}",
                        $"{new DateTime(Year, Month, Day, Hour, Minute, Second).ToShortDateString()}",
                    ]);
                }

            }

        }
    }
}
