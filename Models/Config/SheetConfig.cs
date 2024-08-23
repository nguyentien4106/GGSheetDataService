using System;
using System.Collections.Generic;
using System.Text;
using DataWorkerService.Models.Config;

namespace DataWorkerService.Models
{
    public class SheetConfig
    {
        public string URL { get; set; }

        public string SheetName { get; set; }

        public string SheetId { get; set; }

        public List<string> DocumentIds { get; set; }

        public string ServiceAccountId { get; set; }

        public JSONCredential JSONCredential { get; set; }
    }
}
