using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class Serviser
    {
        public int Id { get; set; }
        public int IdServis { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Opis { get; set; }

        public virtual Servis IdServisNavigation { get; set; }

        public static explicit operator Serviser(char v)
        {
            throw new NotImplementedException();
        }
    }
}
