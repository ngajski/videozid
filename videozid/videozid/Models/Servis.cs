using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace videozid.Models
{
    public partial class Servis
    {
        public Servis()
        {
            Serviser = new HashSet<Serviser>();
            Servisira = new HashSet<Servisira>();
            TipServisa = new HashSet<TipServisa>();
        }

        public int Id { get; set; }
        public string Ime { get; set; }
        public string ZiroRacun { get; set; }
        public string Opis { get; set; }

        public virtual ICollection<Serviser> Serviser { get; set; }
        public virtual ICollection<Servisira> Servisira { get; set; }
        public virtual ICollection<TipServisa> TipServisa { get; set; }

        [NotMapped]
        //Position in the result
        public int Position { get; set; }

        //public static implicit operator Servis(Servis v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
