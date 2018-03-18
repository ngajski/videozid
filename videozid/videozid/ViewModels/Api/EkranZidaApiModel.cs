using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Ekran videozida
    /// </summary>
    public class EkranZidaApiModel
    {
        /// <summary>
        /// ID ekrana videozida
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// ID uređaja
        /// </summary>
        [Required(ErrorMessage = "ID uređaja je obvezno polje")]
        public int IdUredaja { get; set; }

        /// <summary>
        /// Ime Uređaja
        /// </summary>
        public string Uredaj { get; set; }

        /// <summary>
        /// ID Videozida
        /// </summary>
        [Required(ErrorMessage = "ID videozida je obvezno polje")]
        public int IdZida { get; set; }

        /// <summary>
        /// Ime Zida
        /// </summary>
        public string Zid { get; set; }

        /// <summary>
        /// X-koordinata
        /// </summary>
        [Range(0, 100)]
        [Required(ErrorMessage = "X-koordinata mora biti u rasponu između 0 i 100")]
        public int XKoord { get; set; }

        /// <summary>
        /// Y-koordinata
        /// </summary>
        [Range(0, 100)]
        [Required(ErrorMessage = "Y-koordinata mora biti u rasponu između 0 i 100")]
        public int YKoord { get; set; }
    }
}
