using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    /// <summary>
    /// Prezentacijski pogled za uvoz FerWeb ili Dhmz računa, sadrži status ispravnosti uvezenih podataka
    /// </summary>
    public class ImportExcelViewModel
    {

            /// <summary>
            /// Status koji obilježava da li je račun unesen ili ne
            /// </summary>
            public string Status { get; set; }

            /// <summary>
            /// Korisničko ime 
            /// </summary>
            public string KorisnickoIme { get; set; }

            /// <summary>
            /// Lozinka 
            /// </summary>
            public string Lozinka { get; set; }

            /// <summary>
            /// Broj dozvole za pristup serveru
            /// </summary>
            public int? DozvolaServer { get; set; }

            /// <summary>
            /// Korisnicko ime korisnika koji posjeduje račun
            /// </summary>
            public string Vlasnik { get; set; }

        
    }
}
