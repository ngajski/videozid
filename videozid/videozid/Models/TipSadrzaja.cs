using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{
    public partial class TipSadrzaja
    {
        public TipSadrzaja()
        {
            Sadrzaj = new HashSet<Sadrzaj>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Niste unijeli ime!")]
        public string Ime { get; set; }

        public virtual ICollection<Sadrzaj> Sadrzaj { get; set; }
    }
}
