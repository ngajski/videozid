using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    public class ServisDenorm
    {
        public int Id { get; set; }
        public string ImeServisa { get; set; }
        public string OpisServisa { get; set; }
        public string Serviseri { get; set; }
        public string Uredaji { get; set; }
        public string Tip { get; set; }

        public ServisDenorm ()
        {
            this.ImeServisa = "-";
            this.OpisServisa = "-";
            this.Serviseri = "-";
            this.Uredaji = "-";
            this.Tip = "-";
        }

        public void SetServiseri(ICollection<Serviser> s)
        {
            if (s.Count == 0)
            {
                return;
            }

            Serviseri = "";
            foreach (var serviser in s)
            {
                Serviseri += serviser.Ime + " " + serviser.Prezime + ", ";
            }

            Serviseri = Serviseri.Remove(Serviseri.Length - 2);

        }

        public void SetUredaji(List<string> uredaji)
        {
            if (uredaji.Count == 0)
            {
                return;
            }

            Uredaji = "";
            foreach (var u in uredaji)
            {
                Uredaji += u + ", ";
            }

            Uredaji = Uredaji.Remove(Uredaji.Length - 2);
        }

        public void SetOpis(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            this.OpisServisa = s;
        }

    }
}
