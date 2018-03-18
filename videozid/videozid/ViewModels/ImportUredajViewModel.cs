using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    /// <summary>
    /// Prezentacijski pogled za uvoz uredaja kroz excel, sadrži status ispravnosti uvezenih podataka
    /// </summary>
    public class ImportUredajViewModel
    {
        /// <summary>
        /// Status ispravnosti uvezenih podataka
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Naziv uređaja
        /// </summary>
        public string Naziv { get; set; }

        /// <summary>
        /// Nabavna cijena uređaja
        /// </summary>
        public string NabavnaCijena { get; set; }

        /// <summary>
        /// Aktualna cijena uređaja
        /// </summary>
        public string AktualnaCijena { get; set; }

        /// <summary>
        /// Datum nabavke uređaja
        /// </summary>
        public string DatumNabavke { get; set; }

        /// <summary>
        /// Status uređaja (aktivan ili zamjenski)
        /// </summary>
        public string StatusUredaja { get; set; }

        /// <summary>
        /// Naziv nadređene komponenete uređaja
        /// </summary>
        public string NadredenaKomponenta { get; set; }

        /// <summary>
        /// Naziv videozida kojem je uređaj pridružen
        /// </summary>
        public string Videozid { get; set; }
    }
}
