using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class PodshemaPrikazivanja
    {
        public PodshemaPrikazivanja()
        {
            Koristi = new HashSet<Koristi>();
            Sadrzi = new HashSet<Sadrzi>();
        }

        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }

        public virtual ICollection<Koristi> Koristi { get; set; }
        public virtual ICollection<Sadrzi> Sadrzi { get; set; }
    }
}
