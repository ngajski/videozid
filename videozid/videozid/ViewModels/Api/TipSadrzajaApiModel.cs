using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{
    public class TipSadrzajaApiModel
    {
        /// <summary>
        /// ID tipa sadržaja
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Ime tipa sadržaja
        /// </summary>
        [Required(ErrorMessage = "Ime tipa sadržaja je obavezno!")]
        public string Ime { get; set; }
    }
}
