﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Prezentacijski pogled za uvoz korisnika kroz excel, objedinjuje tablice administrator i korisnik, te sadrži status ispravnosti uvezenih podataka
    /// </summary>
    public class ImportExcelKorisnikViewModel
    {
        /// <summary>
        /// Status koji obilježava da li je korisnik unesen ili ne
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Ime korisnika
        /// </summary>
        public string Ime { get; set; }

        /// <summary>
        /// Prezime korisnika
        /// </summary>
        public string Prezime { get; set; }

        /// <summary>
        /// Email adresa 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Lozinka 
        /// </summary>
        public string Lozinka { get; set; }

        /// <summary>
        /// Korisničko ime 
        /// </summary>
        public string KorisnickoIme { get; set; }

        /// <summary>
        /// Zastavica koja obilježava da li je korisnik administrator
        /// </summary>
        public bool Admin { get; set; }

    }
}
