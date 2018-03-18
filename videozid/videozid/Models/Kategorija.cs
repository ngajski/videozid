using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{
    public partial class Kategorija
    {
        public Kategorija()
        {
            Prezentacija = new HashSet<Prezentacija>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Niste unijeli vrstu!")]
        public string Vrsta { get; set; }

        public virtual ICollection<Prezentacija> Prezentacija { get; set; }
    }
}
