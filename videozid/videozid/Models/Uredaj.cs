using System;
using System.Collections.Generic;

namespace videozid.Models
{
    /// <summary>
    /// Podatkovni model uređaja
    /// </summary>
    public partial class Uredaj
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public Uredaj()
        {
            EkranZida = new HashSet<EkranZida>();
            Servisira = new HashSet<Servisira>();
            ZamjenskiUredajIdUredajaNavigation = new HashSet<ZamjenskiUredaj>();
            ZamjenskiUredajIdZamjenaZaNavigation = new HashSet<ZamjenskiUredaj>();
        }

        /// <summary>
        /// ID uređaja
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Naziv uređaja
        /// </summary>
        public string Naziv { get; set; }

        /// <summary>
        /// Nabavna cijena uređaja
        /// </summary>
        public double NabavnaCijena { get; set; }

        /// <summary>
        /// Aktualna cijena uređaja
        /// </summary>
        public double AktualnaCijena { get; set; }

        /// <summary>
        /// Datum nabavke uređaja
        /// </summary>
        public DateTime DatumNabavke { get; set; }

        /// <summary>
        /// ID uređaja koji je nadređen ovom uređaju
        /// </summary>
        public int? IdNadredeneKomponente { get; set; }

        /// <summary>
        /// ID videozida kojemu ovaj uređaj pripada
        /// </summary>
        public int? IdZida { get; set; }

        /// <summary>
        /// ID statusa kojeg je ovaj uređaj 
        /// </summary>
        public int IdStatusa { get; set; }

        /// <summary>
        /// Ekran zida
        /// </summary>
        public virtual ICollection<EkranZida> EkranZida { get; set; }

        /// <summary>
        /// Lista servisera koji ovaj uređaj servisiraju
        /// </summary>
        public virtual ICollection<Servisira> Servisira { get; set; }

        /// <summary>
        /// ZamjenskiUredajIdUredajaNavigation
        /// </summary>
        public virtual ICollection<ZamjenskiUredaj> ZamjenskiUredajIdUredajaNavigation { get; set; }

        /// <summary>
        /// ZamjenskiUredajIdZamjenaZaNavigation
        /// </summary>
        public virtual ICollection<ZamjenskiUredaj> ZamjenskiUredajIdZamjenaZaNavigation { get; set; }

        /// <summary>
        /// IdNadredeneKomponenteNavigation        
        /// /// </summary>
        public virtual Uredaj IdNadredeneKomponenteNavigation { get; set; }

        /// <summary>
        /// InverseIdNadredeneKomponenteNavigation
        /// </summary>
        public virtual ICollection<Uredaj> InverseIdNadredeneKomponenteNavigation { get; set; }

        /// <summary>
        /// IdStatusaNavigation
        /// </summary>
        public virtual StatusUredaja IdStatusaNavigation { get; set; }

        /// <summary>
        /// IdZidaNavigation
        /// </summary>
        public virtual Videozid IdZidaNavigation { get; set; }
    }
}
