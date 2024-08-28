using DataService.Web.Models.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DataService.Web.Helper
{
    public static class ObjectConverter
    {
        public static string SerializeObject<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new(toSerialize?.GetType());

            using StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        public static T DeserializeToJObject<T>(this string json, string value)
        {
            var a = JObject.Parse(json);
            
            return a.GetValue(value).ToObject<T>();
        }
    }
}
