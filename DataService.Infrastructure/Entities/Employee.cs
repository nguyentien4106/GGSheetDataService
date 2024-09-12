using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Infrastructure.Entities
{
    public class Employee : BaseEntity
    {
        public string Pin { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int Privilege { get; set; }

        public string CardNumber { get; set; } = string.Empty;

    }
}
