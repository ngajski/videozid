using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    /// <summary>
    /// Prezentacijski pogled za detaljni prikaz korisnika
    /// </summary>
    public class KorisnikDetailsViewModel
    {
        /// <summary>
        /// Instanca korisnika
        /// </summary>
        public Korisnik korisnik { get; set; }

        /// <summary>
        /// Popis sadržaja gdje je korisnik autor istih
        /// </summary>
        public IEnumerable<Sadrzaj> Sadrzaji { get; set; }

        /// <summary>
        /// Sadržaji koje je korisnik odobrio za prikaz na videozid-u
        /// </summary>
        public IEnumerable<Sadrzaj> Odobrio { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="korisnik">Primjerak korisnika</param>
        /// <param name="sadrzaji">Autor sadržaja</param>
        /// <param name="odobrio">Odobreni sadržaji</param>
        public KorisnikDetailsViewModel(Korisnik korisnik, IEnumerable<Sadrzaj> sadrzaji, IEnumerable<Sadrzaj> odobrio)
        {
            this.Sadrzaji = sadrzaji;
            this.korisnik = korisnik;
            this.Odobrio = odobrio;
        }
    }
}
