
using System.ComponentModel.DataAnnotations;

namespace DataService.Infrastructure.Entities
{
    public class Attendance : BaseEntity
    {
        [Display(Name = "User")]
        [Required(ErrorMessage = "{0} is required!")]
        public int EmployeeId { get; set; }


        [Display(Name = "Device")]
        [Required(ErrorMessage = "{0} is required!")]

        public int DeviceId { get; set; }

        [Display(Name = "Verify Date")]
        [Required(ErrorMessage = "{0} is required!")]

        public DateTime VerifyDate { get; set; }

        [Display(Name = "Verify Type")]
        [Required(ErrorMessage = "{0} is required!")]

        public int VerifyType { get;set; }

        [Display(Name = "Verify State")]
        [Required(ErrorMessage = "{0} is required!")]

        public int VerifyState { get; set; }

        public int WorkCode { get; set; }

    }
}
