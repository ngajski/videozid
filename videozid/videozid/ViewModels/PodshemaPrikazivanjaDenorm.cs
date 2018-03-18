using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    public class PodshemaPrikazivanjaDenorm
    {
        public int Id { get; set; }

        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Sheme { get; set; }
        public string Od { get; set; }
        public string Do { get; set; }

        public PodshemaPrikazivanjaDenorm()
        {
            this.Naziv = "-";
            this.Opis = "-";
            this.Sheme = "-";
            
        }

        public void SetNaziv(string s)
        {
            this.Naziv = s;
        }

        public void SetOpis(string s)
        {
            this.Opis = s;
        }

        public void SetSheme(string s)
        {
            this.Sheme = s;
        }
    }
}
