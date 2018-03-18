using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    public class KorisnikDenorm
    {
        public int Id { get; set; }
        public string PrezimeIme { get; set; }
        public string Email { get; set; }
        public string KorisnickoIme { get; set; }
        public string Fer { get; set; }
        public string DHMZ { get; set; }
        public string Autor { get; set; }
        public string Odobrio { get; set; }


        public KorisnikDenorm()
        {
            this.PrezimeIme = "-";
            this.Email = "-";
            this.KorisnickoIme = "-";
            this.Fer = "-";
            this.DHMZ = "-";
            this.Autor = "-";
            this.Odobrio = "-";
            this.Id = 0;
        }
        public void SetAutor(List<string> autor)
        {
            if (autor.Count == 0)
            {
                return;
            }

            this.Autor = "";
            var numOf = autor.Count();
            var j = 0;
            foreach (var i in autor)
            {
                if (j == (numOf -1))
                {
                    this.Autor += i;
                }
                else
                {
                    this.Autor += i + "\n";
                }
                j++;
            }
        }
        public void SetOdobrio(List<string> odobrio)
        {
            if (odobrio.Count == 0)
            {
                return;
            }

            this.Odobrio = "";
            var numOf = odobrio.Count();
            var j = 0;
            foreach (var i in odobrio)
            {
                if (j == (numOf - 1))
                {
                    this.Odobrio += i;
                }
                else
                {
                    this.Odobrio += i + "\n";
                }
                j++;
                
            }
        }
        public void SetPrezimeIme(string ime, string prez)
        {
            this.PrezimeIme = "";
            this.PrezimeIme = prez + " " + ime;
        }
    }
}
