using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{

    /// <summary>
    /// Kategorija
    /// </summary>
    public class KategorijaApiModel
    {
        /// <summary>
        /// ID kategorije
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Vrsta kategorije
        /// </summary>
        [Required(ErrorMessage = "Vrsta je obavezna.")]
        public string Vrsta { get; set; }
    }
}
