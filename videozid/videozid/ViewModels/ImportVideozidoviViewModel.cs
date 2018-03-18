using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    /// <summary>
    /// Razred za uvoz videozidova s Excela
    /// </summary>
    public class ImportVideozidoviViewModel
    {
        /// <summary>
        /// Status uvoza 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Naziv videozida
        /// </summary>
        public string Naziv { get; set; }

        /// <summary>
        /// Lokacija videozida
        /// </summary>
        public string Lokacija { get; set; }

        /// <summary>
        /// Širina videozida
        /// </summary>
        public string Sirina { get; set; }

        /// <summary>
        /// Visina videozida
        /// </summary>
        public string Visina { get; set; }
    }
}
