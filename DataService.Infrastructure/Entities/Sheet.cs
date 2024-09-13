using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Infrastructure.Entities
{
    public class Sheet : BaseEntity
    {
        public int DeviceId { get; set; }

        public Device? Device { get; set; }

        public string SheetName { get; set; } = string.Empty;

        public string DocumentId { get; set; }
    }
}
