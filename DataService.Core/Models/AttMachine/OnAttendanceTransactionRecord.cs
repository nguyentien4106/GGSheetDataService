using DataService.Infrastructure.Entities;

namespace DataService.Core.Models.AttMachine
{
    public class OnAttendanceTransactionRecord
    {
        public int DeviceId { get; set; }

        public string UserId { get; set; }

        public int WorkCode { get; set; }

        public int IsInvalid { get; set; }

        public int AttState { get; set; }

        public int VerifyMethod { get; set; }

        public DateTime DateTimeRecord { get; set; }

        public Attendance ToAttendance()
        {
            return new Attendance()
            {
                EmployeeId = int.Parse(UserId),
                DeviceId = DeviceId,
                WorkCode = WorkCode,
                VerifyDate = DateTimeRecord,
                VerifyState = AttState,
                VerifyType = VerifyMethod,
            };
        }

        public List<string> ToRow()
        {
            return new List<string>()
            {
                UserId,
                DeviceId.ToString(),
                DateTimeRecord.ToString("f"),
                AttMachine.VerifyMethod.GetVerifyMethod(VerifyMethod),
                AttMachine.AttState.GetAttState(AttState)
            };
        }
    }
}
