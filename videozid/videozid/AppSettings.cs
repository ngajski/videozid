using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid
{
    public class AppSettings
    {
        //Ja sam generirao klasu
        public int PageSize { get; set; } = 5;
        public int PageOffset { get; set; } = 10;
        public string ConnectionString { get; set; }
    }
}
