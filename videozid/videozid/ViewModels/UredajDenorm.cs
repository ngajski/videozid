using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels
{
    /// <summary>
    /// Razred za pripremu prikaza informacija o uređaju i njegov ispis u format PDF
    /// </summary>
    public class UredajDenorm
    {
        /// <summary>
        /// Id uređaja
        /// </summary>
        [Display(Name = "Identifikator uređaja")]
        public int Id { get; set; }

        /// <summary>
        /// Naziv uređaja
        /// </summary>
        [Display(Name = "Ime uređaja")]
        public string Naziv { get; set; }

        /// <summary>
        /// Nabavna cijena uređaja
        /// </summary>
        [Display(Name = "Nabavna cijena")]
        public double NabavnaCijena { get; set; }

        /// <summary>
        /// Aktualna cijena uređaja
        /// </summary>
        [Display(Name = "Aktualna cijena")]
        public double AktualnaCijena { get; set; }

        /// <summary>
        /// Datum nabavke uređaja
        /// </summary>
        [Display(Name = "Datum nabavke")]
        public string DatumNabavke { get; set; }

        /// <summary>
        /// Nadređena komponenta uređaja
        /// </summary>
        [Display(Name = "Nadređena komponenta")]
        public string NadredenaKomponenta { get; set; }

        /// <summary>
        /// Videozid kojem uređaj pripada
        /// </summary>
        [Display(Name = "Pripada videozidu")]
        public string Zid { get; set; }

        /// <summary>
        /// Status uređaja
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Lista uređaja kojim je uređaj zamjenski
        /// </summary>
        [Display(Name = "Zamjena za uređaj")]
        public string ZamjenaZa { get; set; }

        /// <summary>
        /// Lista uređaja koji su zamjenski uređaju
        /// </summary>
        [Display(Name = "Zamjenski uređaji")]
        public string Zamjena { get; set; }

        /// <summary>
        /// Lista servisa uređaja
        /// </summary>
        [Display(Name = "Pripdani servisi")]
        public string Servisi { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public UredajDenorm()
        {
            NadredenaKomponenta = "/";
            Zid = "/";
            Status = "/";
            ZamjenaZa = "/";
            Zamjena = "/";
            Servisi = "/";
        }
    }
}
