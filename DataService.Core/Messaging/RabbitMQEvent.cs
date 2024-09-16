using DataService.Infrastructure.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Messaging
{
    public class RabbitMQEvent<T> where T : BaseEntity
    {
        public ActionType ActionType {get;set; }

        public T Data { get; set;}

        public string ToString() => JsonConvert.SerializeObject(this);
    }
}
