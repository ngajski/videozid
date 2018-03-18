using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class StatusUredaja
    {
        public StatusUredaja()
        {
            Uredaj = new HashSet<Uredaj>();
        }

        public int Id { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Uredaj> Uredaj { get; set; }
    }
}
