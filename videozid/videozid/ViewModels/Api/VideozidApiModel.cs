using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Videozid
    /// </summary>
    public class VideozidApiModel
    {
        /// <summary>
        /// ID videozida
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Naziv videozida
        /// </summary>
        [Required(ErrorMessage = "Naziv videozida je obvezno polje")]
        public string Naziv { get; set; }

        /// <summary>
        /// Lokacija videozida
        /// </summary>
        [Required(ErrorMessage = "Lokacija videozida je obvezno polje")]
        public string Lokacija { get; set; }

        /// <summary>
        /// Širina videozida
        /// </summary>
        [Range(0, 100)]
        [Required(ErrorMessage = "Širina videozida mora biti između 0 i 100")]
        public int Sirina { get; set; }

        /// <summary>
        /// Visina videozida
        /// </summary>
        [Range(0, 100)]
        [Required(ErrorMessage = "Visina videozida mora biti između 0 i 100")]
        public int Visina { get; set; }

        /// <summary>
        /// Ekrani videozida
        /// </summary>
        public IEnumerable<MasterDetailHelper> Ekrani { get; set; }
    }
}
