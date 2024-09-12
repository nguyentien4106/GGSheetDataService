using DataService.Core.Models.AttMachine;
using DataService.Infrastructure.Entities;

namespace DataService.Web.Models.Att
{
    public class AttendanceViewModel
    {
        public AttendanceViewModel(Attendance attendance)
        {
            VerifyType = VerifyMethod.GetVerifyMethod(attendance.VerifyType);
            VerifyState = AttState.GetAttState(attendance.VerifyState);
            Id = attendance.Id;
            VerifyDate = attendance.VerifyDate;
            DeviceId = attendance.DeviceId;
            UserId = attendance.UserId;
        }


        public int Id { get; set; }
        public int UserId { get; set; }

        public int DeviceId { get; set; }

        public DateTime VerifyDate { get; set; }

        public string VerifyType { get; set; }

        public string VerifyState { get;set; }

        public int WorkCode { get; set; }
    }
}
