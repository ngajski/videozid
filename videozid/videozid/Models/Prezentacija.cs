using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{
    public partial class Prezentacija
    {
        public Prezentacija()
        {
            Koristi = new HashSet<Koristi>();
        }

        public int Id { get; set; }
        public int IdSadrzaja { get; set; }
        public int IdKategorije { get; set; }

        [Range(0, 3, ErrorMessage = "Samo pozitivne vrijednosti od 0 do 3!")]
        [Required(ErrorMessage = "Niste unijeli koordinatu!")]
        public int XKoord { get; set; }
        [Range(0, 3, ErrorMessage = "Samo pozitivne vrijednosti od 0 do 3!")]
        [Required(ErrorMessage = "Niste unijeli koordinatu!")]
        public int YKoord { get; set; }
        [Range(1, 4, ErrorMessage = "Samo pozitivne vrijednosti od 1 do 4!")]
        [Required(ErrorMessage = "Niste unijeli sirinu!")]
        public int Sirina { get; set; }
        [Range(1, 4, ErrorMessage = "Samo pozitivne vrijednosti od 1 do 4!")]
        [Required(ErrorMessage = "Niste unijeli visinu!")]
        public int Visina { get; set; }

        public virtual ICollection<Koristi> Koristi { get; set; }
        public virtual Kategorija IdKategorijeNavigation { get; set; }
        public virtual Sadrzaj IdSadrzajaNavigation { get; set; }
    }
}
