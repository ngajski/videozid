using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace videozid.ViewModels.Api
{
    /// <summary>
    /// Sadržaj
    /// </summary>
    public class SadrzajApiModel
    {
        /// <summary>
        /// ID sadržaja
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ime sadržaja
        /// </summary>
        [Required(ErrorMessage = "Ime sadržaja je obvezno polje")]
        public string Ime { get; set; }

        /// <summary>
        /// Opis sadržaja
        /// </summary>
        [Required(ErrorMessage = "Opis sadržaja je obvezno polje")]
        public string Opis { get; set; }

        /// <summary>
        /// URL sadržaja
        /// </summary>
        [Required(ErrorMessage = "URL sadržaja je obvezno polje")]
        public string Url { get; set; }

        /// <summary>
        /// Odobren
        /// </summary>
        public bool? JeOdobren { get; set; }

        /// <summary>
        /// ID autora
        /// </summary>
        public int IdAutora { get; set; }

        /// <summary>
        /// Ime autora
        /// </summary>
        public string Autor { get; set; }

        /// <summary>
        /// ID korisnika koji je odobrio sadržaj
        /// </summary>
        public int IdOdobrenOd { get; set; }

        /// <summary>
        /// Ime korisnika koji je odobrio sadržaj
        /// </summary>
        public string OdobrenOd { get; set; }

        /// <summary>
        /// ID tipa sadržaja
        /// </summary>
        public int IdTipa { get; set; }

        /// <summary>
        /// Ime tipa
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// Prezentacija
        /// </summary>
        public IEnumerable<MasterDetailHelper> Prezentacija { get; set; }
    }
}
