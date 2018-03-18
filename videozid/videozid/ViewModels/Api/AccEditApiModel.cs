using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Prezentacijski model za ažuriranje FerWeb i DHMZ računa
    /// </summary>
    public class AccEditApiModel
    {

        /// <summary>
        /// Lozinka 
        /// </summary>
        public string Lozinka { get; set; }

        /// <summary>
        /// Broj dozvole za pristup serveru FerWeba, DHMZ-a
        /// </summary>
        public int? DozvolaServer { get; set; }

    }
}
