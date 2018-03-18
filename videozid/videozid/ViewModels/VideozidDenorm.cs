using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels
{
    /// <summary>
    /// Razred za prikaz videozida spremnog za ispis u PDF formatu
    /// </summary>
    public class VideozidDenorm
    {
        /// <summary>
        /// Naziv Videozida
        /// </summary>
        [Display(Name = "Naziv videozida")]
        public string Naziv { get; set; }

        /// <summary>
        /// LOkacija Videozida
        /// </summary>
        [Display(Name = "Lokacija")]
        public string Lokacija { get; set; }

        /// <summary>
        /// Sirina Videozida
        /// </summary>
        [Display(Name = "Širina zida (ekrani)")]
        public int Sirina { get; set; }

        /// <summary>
        /// Visina Videozida
        /// </summary>
        [Display(Name = "Visina zida (ekrani)")]
        public int Visina { get; set; }

        /// <summary>
        /// Ekrani Videozida
        /// </summary>
        [Display(Name = "Ekrani videozida")]
        public string Ekrani { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public VideozidDenorm()
        {
            Ekrani = "/";
        }
    }
}
