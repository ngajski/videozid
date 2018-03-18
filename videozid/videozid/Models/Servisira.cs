using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class Servisira
    {
        public Servisira()
        {
            //TipServisa = new HashSet<TipServisa>();
        }

        public int Id { get; set; }
        public int IdServis { get; set; }
        public int IdUredaj { get; set; }

        //public virtual ICollection<TipServisa> TipServisa { get; set; }
        public virtual Servis IdServisNavigation { get; set; }
        public virtual Uredaj IdUredajNavigation { get; set; }
    }
}
