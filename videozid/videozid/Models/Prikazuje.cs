using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class Prikazuje
    {
        public int Id { get; set; }
        public int IdZida { get; set; }
        public int IdSheme { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }

        public virtual ShemaPrikazivanja IdShemeNavigation { get; set; }
        public virtual Videozid IdZidaNavigation { get; set; }
    }
}
