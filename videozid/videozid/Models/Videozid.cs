using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{

    /// <summary>
    /// Podatkovni model videozida
    /// </summary>
    public partial class Videozid
    {
        /// <summary>
        /// KOnstruktor
        /// </summary>
        public Videozid()
        {
            EkranZida = new HashSet<EkranZida>();
            Prikazuje = new HashSet<Prikazuje>();
            Uredaj = new HashSet<Uredaj>();
        }

        /// <summary>
        /// ID videozida
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Naziv videozida
        /// </summary>
        [Required]
        public string Naziv { get; set; }

        /// <summary>
        /// Lokacija videozida
        /// </summary>
        [Required]
        public string Lokacija { get; set; }

        /// <summary>
        /// Sirina vidoezida
        /// </summary>
        [Range(0, 100)]
        public int Sirina { get; set; }

        /// <summary>
        /// Visina videozida
        /// </summary>
        [Range(0, 100)]
        public int Visina { get; set; }

        /// <summary>
        /// Lista ekrana videozida
        /// </summary>
        public virtual ICollection<EkranZida> EkranZida { get; set; }

        /// <summary>
        /// Lista shemi prikazivanja koje videozid prikazuje
        /// </summary>
        public virtual ICollection<Prikazuje> Prikazuje { get; set; }

        /// <summary>
        /// Lista uređaja koji su sastavni dio videozida
        /// </summary>
        public virtual ICollection<Uredaj> Uredaj { get; set; }
    }
}
