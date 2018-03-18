using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Prezentacija
    /// </summary>
    public class PrezentacijaApiModel
    {
        /// <summary>
        /// ID prezentacije
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// X koordinata
        /// </summary>
        [Range(0, 3, ErrorMessage = "Samo pozitivne vrijednosti od 0 do 3!")]
        [Required(ErrorMessage = "Niste unijeli koordinatu!")]
        public int XKoord { get; set; }

        /// <summary>
        /// Y koordinata
        /// </summary>
        [Range(0, 3, ErrorMessage = "Samo pozitivne vrijednosti od 0 do 3!")]
        [Required(ErrorMessage = "Niste unijeli koordinatu!")]
        public int YKoord { get; set; }

        /// <summary>
        /// Širina
        /// </summary>
        [Range(1, 4, ErrorMessage = "Samo pozitivne vrijednosti od 1 do 4!")]
        [Required(ErrorMessage = "Niste unijeli širinu!")]
        public int Sirina { get; set; }

        /// <summary>
        /// Visina
        /// </summary>
        [Range(1, 4, ErrorMessage = "Samo pozitivne vrijednosti od 1 do 4!")]
        [Required(ErrorMessage = "Niste unijeli visinu!")]
        public int Visina { get; set; }

        /// <summary>
        /// ID sadržaja
        /// </summary>
        public int IdSadrzaja { get; set; }

        /// <summary>
        /// Ime sadržaja
        /// </summary>
        public string Sadrzaj{ get; set; }

        /// <summary>
        /// ID kategorije
        /// </summary>
        public int IdKategorije { get; set; }

        /// <summary>
        /// Ime kategorije
        /// </summary>
        public string Kategorija { get; set; }

        /// <summary>
        /// Koristi prezentaciju
        /// </summary>
        public IEnumerable<MasterDetailHelper> KoristenOd { get; set; }
    }
}
