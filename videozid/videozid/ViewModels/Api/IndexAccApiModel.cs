using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Prezentacijski pogled za prikaz FerWeb i DHMZ računa s njihovim vlasnikom
    /// </summary>
    public class IndexAccApiModel
    {

        /// <summary>
        /// ID računa
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Korisničko ime FerWeb, DHMZ računa
        /// </summary>
        public string KorisnickoIme { get; set; }

        /// <summary>
        /// Lozinka FerWeb, DHMZ računa
        /// </summary>
        public string Lozinka { get; set; }

        /// <summary>
        /// Broj dozvole za pristup serveru
        /// </summary>
        public int? DozvolaServer { get; set; }

        /// <summary>
        /// Korisnicko ime korisnika koji posjeduje taj FerWeb, DHMZ račun
        /// </summary>
        public string Vlasnik { get; set; }

    }
}
