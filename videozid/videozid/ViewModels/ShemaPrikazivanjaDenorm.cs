using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    public class ShemaPrikazivanjaDenorm
    {

        public int Id { get; set; } 
 
        public string Naziv { get; set; }
        public string Opis { get; set; }

        public ShemaPrikazivanjaDenorm ()
        {
            this.Naziv = "-";
            this.Opis = "-";
        }

        public void SetNaziv(string s)
        {
            this.Naziv = s;
        }

        public void SetOpis(string s)
        {
            this.Opis = s;
        }
    }
}
