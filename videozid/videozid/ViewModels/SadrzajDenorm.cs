using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    public class SadrzajDenorm
    {
        public int Id { get; set; }

        public string Autora { get; set; }
        public string OdobrenOd { get; set; }
        public string Tip { get; set; }
        public string Ime { get; set; }
        public string Opis { get; set; }
        public string Url { get; set; }

        public SadrzajDenorm()
        {
            this.Autora = "-";
            this.OdobrenOd = "-";
            this.Tip = "-";
            
        }

        public void SetAutor(string s)
        {
            this.Autora = s;
        }

        public void SetOdobrenOd(string s)
        {
            this.OdobrenOd = s;
        }

        public void SetTip(string s)
        {
            this.Tip = s;
        }
    }
}
