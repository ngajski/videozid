using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class ShemaPrikazivanja
    {
        public ShemaPrikazivanja()
        {
            Prikazuje = new HashSet<Prikazuje>();
            Sadrzi = new HashSet<Sadrzi>();
        }

        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }

        public virtual ICollection<Prikazuje> Prikazuje { get; set; }
        public virtual ICollection<Sadrzi> Sadrzi { get; set; }
    }
}
