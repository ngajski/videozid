using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;
using videozid.ViewModels;

namespace videozid.ViewModels
{
    /// <summary>
    /// Razred za prikaz Uređaja i njemu pridruženih komponenti
    /// </summary>
    public class UredajDetailsViewModel
    {
        /// <summary>
        /// Uređaj
        /// </summary>
        public Uredaj Uredaj { get; set; }

        /// <summary>
        /// Lista Uređaja kojim je uređaj zamjena
        /// </summary>
        public IEnumerable<ZamjenskiUredaj> ZamjenaZa { get; set; }

        /// <summary>
        /// Lista Uređaja koji se uređaju zamjena
        /// </summary>
        public IEnumerable<ZamjenskiUredaj> Zamjena { get; set; }

        /// <summary>
        /// Lista servisa koji se servisiraju uređaj
        /// </summary>
        public IEnumerable<Servisira> Servisira { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public UredajDetailsViewModel(Uredaj u, IEnumerable<ZamjenskiUredaj> z1, IEnumerable<ZamjenskiUredaj> z2, IEnumerable<Servisira> s)
        {
            this.Uredaj = u;
            this.ZamjenaZa = z1;
            this.Zamjena = z2;
            this.Servisira = s;
        }

        /// <summary>
        /// Postupak 
        /// </summary>
        public UredajDenorm GetInfo()
        {
            var u = new UredajDenorm();

            u.Id = Uredaj.Id;
            u.NabavnaCijena = Uredaj.NabavnaCijena;
            u.AktualnaCijena = Uredaj.AktualnaCijena;
            u.DatumNabavke = Uredaj.DatumNabavke.ToString("dd.MM.yyyy.");
            u.Naziv = Uredaj.Naziv;

            if(Uredaj.IdStatusaNavigation != null)
                u.Status = Uredaj.IdStatusaNavigation.Naziv;

            if (Uredaj.IdNadredeneKomponenteNavigation != null)
                u.NadredenaKomponenta = Uredaj.IdNadredeneKomponenteNavigation.Naziv;

            if (Uredaj.IdZidaNavigation != null)
                u.Zid = Uredaj.IdZidaNavigation.Naziv;

            if (Zamjena.Count() > 0)
            {
                u.Zamjena = "";
                foreach (var ur in Zamjena)
                {
                    u.Zamjena += ur.IdUredajaNavigation.Naziv + ", ";
                }
            }

            if (ZamjenaZa.Count() > 0)
            {
                u.ZamjenaZa = "";
                foreach (var ur in ZamjenaZa)
                {
                    u.ZamjenaZa += ur.IdZamjenaZaNavigation.Naziv + ", ";
                }
            }

            if (Servisira.Count() > 0)
            {
                u.Servisi = "";
                foreach (var s in Servisira)
                {
                    u.Servisi += s.IdServisNavigation.Ime + ", ";
                }
            }


            return u;
        }

    }
}
