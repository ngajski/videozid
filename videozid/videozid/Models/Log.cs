using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.Models
{
    public class Log
    {
        public DateTime Logged { get; set; }
        public int Id { get; set; }
        public string Application { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
        public string Logger { get; set; }

        public Log()
        {

        }

        internal static Log FromString(string text)
        {
            string[] arr = text.Split('|');
            Log entry = new Log();
            entry.Logged = DateTime.ParseExact(arr[0], "yyyy-MM-dd HH:mm:ss.ffff", System.Globalization.CultureInfo.InvariantCulture);
            entry.Id = int.Parse(arr[1]);
            entry.Application = arr[2];
            entry.Level = arr[3];
            entry.Message = arr[4];
            if (arr.Length > 5) entry.Callsite = arr[5].Substring(5); //url: 
            //if (arr.Length > 5) entry.Callsite = entry.Action = arr[6].Substring(8); //action: 
            return entry;
        }
    }
}
