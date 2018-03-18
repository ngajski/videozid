using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{
    /// <summary>
    /// Račun za prijavu na FerWeb
    /// </summary>
    public partial class FerWebAcc
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FerWebAcc()
        {
            Korisnik = new HashSet<Korisnik>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Lozinka računa
        /// </summary>
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Minimalno 8 znakova, maksimum 20")]
        [Required(ErrorMessage = "Niste unijeli lozinku!")]
        public string Lozinka { get; set; }

        /// <summary>
        /// Korisničko ime računa
        /// </summary>
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Minimalno 3 znaka, maksimum 16")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Dozvoljeni su brojevi,slova i znak '_'")]
        [Required(ErrorMessage = "Niste unijeli korisničko ime!")]
        [Remote("racunPostoji", "FerWebAcc", AdditionalFields = "Id", ErrorMessage = "Korisničko ime već postoji")]
        public string KorisnickoIme { get; set; }

        /// <summary>
        /// Broj dozvole za pristup serveru
        /// </summary>
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Dozvoljeni samo brojevi")]
        public int? DozvolaServer { get; set; }

        /// <summary>
        /// Skup korisnika koji posjeduje račun
        /// </summary>
        public virtual ICollection<Korisnik> Korisnik { get; set; }
    }
}
