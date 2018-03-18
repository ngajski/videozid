using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;

namespace videozid.ViewModels
{
    public class ServisDetailsViewModel 
    {
        public Servis Servis { get; set; }
        public IEnumerable<Serviser> Serviseri { get; set; }
        public IEnumerable<Uredaj> Uredaji { get; set; }
        public TipServisa TipServisa { get; set; }

        public ServisDetailsViewModel(Servis servis, IEnumerable<Serviser> serviseri, IEnumerable<Uredaj> uredaji,TipServisa tip)
        {
            this.Servis = servis;
            this.Serviseri = serviseri;
            this.Uredaji = uredaji;
            this.TipServisa = tip;
        }
    }
}