using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{
    public partial class Sadrzaj
    {
        public Sadrzaj()
        {
            Prezentacija = new HashSet<Prezentacija>();
        }

        public int Id { get; set; }
        public int IdAutora { get; set; }
        public int IdOdobrenOd { get; set; }
        public int IdTipa { get; set; }

        [Required(ErrorMessage = "Niste unijeli ime!")]
        public string Ime { get; set; }
        [Required(ErrorMessage = "Niste unijeli opis!")]
        public string Opis { get; set; }
        [Required(ErrorMessage = "Niste unijeli URL!")]
        public string Url { get; set; }
        public bool? JeOdobren { get; set; }

        public virtual ICollection<Prezentacija> Prezentacija { get; set; }
        public virtual Korisnik IdAutoraNavigation { get; set; }
        public virtual Korisnik IdOdobrenOdNavigation { get; set; }
        public virtual TipSadrzaja IdTipaNavigation { get; set; }
       
    }
}
