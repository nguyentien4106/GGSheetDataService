using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Infrastructure.Entities
{
    public class Employee : BaseEntity
    {
        [MaxLength(9)]
        [Display(Name = "PIN")]
        [Required]
        public string Pin { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public int Privilege { get; set; }
        
        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = string.Empty;

        public List<Device> Devices { get; set; } = [];
    }
}
