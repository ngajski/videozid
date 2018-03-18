using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class TipServisa
    {
        public int Id { get; set; }
        public int IdServis { get; set; }
        public string Tip { get; set; }

        public virtual Servis IdServisNavigation { get; set; }
    }
}
