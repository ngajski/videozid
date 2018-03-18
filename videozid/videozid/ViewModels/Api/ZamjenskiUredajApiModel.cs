using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Zamjenski Uređaj
    /// </summary>
    public class ZamjenskiUredajApiModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// ID Uređaja u ulozi zamjenskog
        /// </summary>
        [Required(ErrorMessage = "ID zamjenskog uređaja je obvezno polje")]
        public int IdUredaja { get; set; }

        /// <summary>
        /// Ime Uređaja u ulozi zamjenskog
        /// </summary>
        public string Uredaj { get; set; }

        /// <summary>
        /// ID Uređaja koji ima zamjenu
        /// </summary>
        [Required(ErrorMessage = "ID uređaja kojeg se zamjenjuje je obvezno polje")]
        public int IdZamjenaZa { get; set; }

        /// <summary>
        /// IMe Uređaja koji ima zamjenu
        /// </summary>
        public string Zamjena { get; set; }
    }

}
