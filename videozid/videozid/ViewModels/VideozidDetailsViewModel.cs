using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    /// <summary>
    /// Prezentacijski model podataka koji sadrži osnovne informacije o vidoezidu kao i njegovim ekranima 
    /// </summary>
    public class VideozidDetailsViewModel
    {
        /// <summary>
        /// Videozid
        /// </summary>
        public Videozid Videozid { get; set; }

        /// <summary>
        /// Lista ekrana pridrućenih videozidu.
        /// </summary>
        public IEnumerable<EkranZida> Ekrani { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public VideozidDetailsViewModel(Videozid v, IEnumerable<EkranZida> e)
        {
            this.Videozid = v;
            this.Ekrani = e;
        }

        /// <summary>
        /// Postupak za pripremu objekta za ispis u PDF formatu
        /// </summary>
        public VideozidDenorm GetInfo()
        {
            var v = new VideozidDenorm();

            v.Naziv = Videozid.Naziv;
            v.Lokacija = Videozid.Lokacija;
            v.Visina = Videozid.Visina;
            v.Sirina = Videozid.Sirina;

            if(Ekrani.Count() > 0)
            {
                v.Ekrani = "";
                foreach(var e in Ekrani)
                {
                    v.Ekrani += e.IdUredajaNavigation.Naziv + ", ";
                }
            }

            return v;
        }
        
    }
}
