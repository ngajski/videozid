using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    public class PrezentacijaDenorm
    {

        public int Id { get; set; } 
        public int XKoord { get; set; }
        public int YKoord { get; set; }
        public int Sirina { get; set; }
        public int Visina { get; set; }

        public string Sadrzaja { get; set; }
        public string Kategorije { get; set; }

        public PrezentacijaDenorm ()
        {
            this.Sadrzaja = "-";
            this.Kategorije = "-";
        }

        public void SetSadrzaj(string s)
        {
            this.Sadrzaja = s;
        }

        public void SetKategorija(string s)
        {
            this.Kategorije = s;
        }
    }
}
