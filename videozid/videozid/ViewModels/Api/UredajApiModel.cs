using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Uređaj
    /// </summary>
    public class UredajApiModel
    {
        /// <summary>
        /// ID uređaja
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Naziv uređaja
        /// </summary>
        [Required(ErrorMessage = "Naziv uređaja je obvezno polje")]
        public string Naziv { get; set; }

        /// <summary>
        /// Nabavna cijena uređaja
        /// </summary>
        [Required(ErrorMessage = "Nabavna Cijena uređaja je obvezno polje")]
        public double NabavnaCijena { get; set; }

        /// <summary>
        /// Aktualna cijena uređaja
        /// </summary>
        [Required(ErrorMessage = "Aktualna Cijena uređaja je obvezno polje")]
        public double AktualnaCijena { get; set; }

        /// <summary>
        /// Datum nabavke uređaja
        /// </summary>
        public string DatumNabavke { get; set; }

        /// <summary>
        /// ID naređenog uređaja
        /// </summary>
        public int? IdNadredeneKomponente { get; set; }

        /// <summary>
        /// Naziv naređenog uređaja
        /// </summary>
        public string NadredenaKomponenta { get; set; }

        /// <summary>
        /// ID videozida kojem uređaj pripada
        /// </summary>
        public int? IdZida { get; set; }

        /// <summary>
        /// Naziv videozida
        /// </summary>
        public string Zid { get; set; }

        /// <summary>
        /// Status uređaja
        /// </summary>
        [Required(ErrorMessage = "Status uređaja je obvezno polje")]
        public int IdStatusa { get; set; }

        /// <summary>
        /// Naziv statusa
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Zamjenski uređaji
        /// </summary>
        public IEnumerable<MasterDetailHelper> Zamjenski {get; set; }

        /// <summary>
        /// Uređaji za koje je uređaj zamjenski
        /// </summary>
        public IEnumerable<MasterDetailHelper> ZamjenaZa { get; set; }

        /// <summary>
        /// Uređaji za koji su podređeni uređaju
        /// </summary>
        public IEnumerable<MasterDetailHelper> PodredeneKomponente { get; set; }

        /// <summary>
        /// Servisi koji su zaduženi za uređaj
        /// </summary>
        public IEnumerable<MasterDetailHelper> Servisi { get; set; }
    }
}
