using CleanArchitecture.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Entities
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
