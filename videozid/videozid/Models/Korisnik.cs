using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{
    /// <summary>
    /// Korisnik aplikacije VideoZid
    /// </summary>
    public partial class Korisnik
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public Korisnik()
        {
            Administrator = new HashSet<Administrator>();
            SadrzajIdAutoraNavigation = new HashSet<Sadrzaj>();
            SadrzajIdOdobrenOdNavigation = new HashSet<Sadrzaj>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ime
        /// </summary>
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Minimalno 3 znaka, maksimum 16")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Uneseni su nedozvoljeni znakovi")]
        [Required(ErrorMessage = "Niste unijeli ime!")]
        public string Ime { get; set; }

        /// <summary>
        /// Prezime
        /// </summary>
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Minimalno 3 znaka, maksimum 16")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Uneseni su nedozvoljeni znakovi")]
        [Required(ErrorMessage = "Niste unijeli ime!")]
        public string Prezime { get; set; }

        /// <summary>
        /// Adresa elektroničke pošte korisnika
        /// </summary>
        [Required(ErrorMessage = "Niste unijeli email adresu!")]
        [EmailAddress(ErrorMessage = "Neispravna adresa")]
        //[Remote("emailPostoji", "Korisnik", AdditionalFields = "Id", ErrorMessage = "Već postoji račun za navedenu email adresu.")]
        public string Email { get; set; }

        /// <summary>
        /// Lozinka
        /// </summary>
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Minimalno 8 znakova, maksimum 20")]
        [Required(ErrorMessage = "Niste unijeli lozinku!")]
        public string Lozinka { get; set; }

        /// <summary>
        /// Korisničko ime 
        /// </summary>
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Minimalno 3 znaka, maksimum 16")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Dozvoljeni su brojevi,slova i znak '_'")]
        [Required(ErrorMessage = "Niste unijeli korisničko ime!")]
        //[Remote("korisnikPostoji", "Korisnik", AdditionalFields = "Id", ErrorMessage ="Korisničko ime već postoji")]
        public string KorisnickoIme { get; set; }


        /// <summary>
        /// Strani ključ na Dhmz račun, id računa
        /// </summary>
        public int? DhmzId { get; set; }

        /// <summary>
        /// Strani ključ na FerWeb račun, id računa
        /// </summary>
        public int? FerId { get; set; }

        /// <summary>
        /// Skup korisnika koji su administratori
        /// </summary>
        public virtual ICollection<Administrator> Administrator { get; set; }

        /// <summary>
        /// Skup sadržaja kojima je korisnik autor
        /// </summary>
        public virtual ICollection<Sadrzaj> SadrzajIdAutoraNavigation { get; set; }

        /// <summary>
        /// Skup sadržaja koje je korisnik odobrio
        /// </summary>
        public virtual ICollection<Sadrzaj> SadrzajIdOdobrenOdNavigation { get; set; }

        /// <summary>
        /// Dhmz račun korisnika
        /// </summary>
        public virtual DhmzAcc Dhmz { get; set; }

        /// <summary>
        /// FerWeb račun korisnika
        /// </summary>
        public virtual FerWebAcc Fer { get; set; }
    }
}
