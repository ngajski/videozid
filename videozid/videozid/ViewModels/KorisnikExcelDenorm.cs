using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    public class KorisnikExcelDenorm
    {
        public int Id { get; set; }
        public string Prezime { get; set; }
        public string Ime { get; set; }
        public string KorisnickoIme { get; set; }
        public string Email { get; set; }
        public string jeAdmin { get; set; }
        public string Lozinka { get; set; }

        public FerWebAcc Fer { get; set; }
        public DhmzAcc DHMZ { get; set; }
        public IOrderedEnumerable<Sadrzaj> Autor { get; set; }
        public IOrderedEnumerable<Sadrzaj> Odobrio { get; set; }

        public KorisnikExcelDenorm()
        {
            this.Prezime = "-";
            this.Ime = "-";
            this.Email = "-";
            this.KorisnickoIme = "-";
            this.jeAdmin = "NE";
            this.Fer = null;
            this.DHMZ = null;
            this.Autor = null;
            this.Odobrio = null;
            this.Id = 0;
        }
          
    }
}
