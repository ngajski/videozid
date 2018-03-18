using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    /// <summary>
    /// Prezentacijski pogled za detaljni prikaz DHMZ računa
    /// </summary>
    public class DhmzAccDetailsViewModel
    {
        /// <summary>
        /// Dhmz račun
        /// </summary>
        public DhmzAcc racun { get; set; }

        /// <summary>
        /// Vlasnik Dhmz računa
        /// </summary>
        public Korisnik korisnik { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="racun">Dhmz račun</param>
        /// <param name="korisnik">Korisnik koji je vlasnik tog Dhmz računa</param>
        public DhmzAccDetailsViewModel (DhmzAcc racun, Korisnik korisnik)
        {
            this.racun = racun;
            this.korisnik = korisnik;
        }
    }
}