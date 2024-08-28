using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Web.Helper
{
    public class RealTimeEventHelper
    {
        //If your fingerprint(or your card) passes the verification,this event will be triggered,only for color device
        public static void axCZKEM1_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {
            var time = Year + "-" + Month + "-" + Day + " " + Hour + ":" + Minute + ":" + Second;
            var info = $"Verify OK.UserID={EnrollNumber} isInvalid={IsInValid} state={AttState} verifystyle={VerifyMethod} time={time}";
            //gRealEventListBox.Items.Add("Verify OK.UserID=" + EnrollNumber + " isInvalid=" + IsInValid.ToString() + " state=" + AttState.ToString() + " verifystyle=" + VerifyMethod.ToString() + " time=" + time);

            throw new NotImplementedException();
        }

        //If your fingerprint(or your card) passes the verification,this event will be triggered,only for black%white device
        public static void axCZKEM1_OnAttTransaction(int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second)
        {
            string time = Year + "-" + Month + "-" + Day + " " + Hour + ":" + Minute + ":" + Second;
            //gRealEventListBox.Items.Add("Verify OK.UserID=" + EnrollNumber.ToString() + " isInvalid=" + IsInValid.ToString() + " state=" + AttState.ToString() + " verifystyle=" + VerifyMethod.ToString() + " time=" + time);

            throw new NotImplementedException();
        }
    }
}
