
namespace DataService.Infrastructure.Entities
{
    public class Attendance : BaseEntity
    {
        public int UserId { get; set; }

        public int DeviceId { get; set; }

        public Device? Device { get; set; }

        public DateTime VerifyDate { get; set; }
        
        public int VerifyType { get;set; }

        public int VerifyState { get; set; }

        public int WorkCode { get; set; }

    }
}
