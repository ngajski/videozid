using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    /// <summary>
    /// Prezentacijski pogled za detaljni prikaz FerWeb računa
    /// </summary>
    public class FerWebAccDetailsViewModel
    {
        /// <summary>
        /// FerWeb račun
        /// </summary>
        public FerWebAcc racun { get; set; }

        /// <summary>
        /// Vlasnik FerWeb računa
        /// </summary>
        public Korisnik korisnik{ get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="racun">FerWeb račun</param>
        /// <param name="korisnik">Korisnik koji je vlasnik tog FerWeb računa</param>
        public FerWebAccDetailsViewModel(FerWebAcc racun, Korisnik korisnik)
        {
            this.racun = racun;
            this.korisnik = korisnik;
        }

    }
}