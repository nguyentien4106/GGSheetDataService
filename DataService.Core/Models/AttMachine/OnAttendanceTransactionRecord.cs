using DataService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Models.AttMachine
{
    public class OnAttendanceTransactionRecord
    {
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
                UserId = int.Parse(UserId),
                WorkCode = WorkCode,
                VerifyDate = DateTimeRecord,
                VerifyState = AttState,
                VerifyType = VerifyMethod
            };
        }
    }
}
