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
        [MaxLength(8)]
        [Required]
        public string Pin { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public int Privilege { get; set; }
        
        [Required]
        public string CardNumber { get; set; } = string.Empty;
    }
}
