using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    public class PodshemaPrikazivanjaDetailsViewModel
    {
        public PodshemaPrikazivanja PodshemaPrikazivanja { get; set; }
        public IEnumerable<Koristi> Prezentacije { get; set; }



        public PodshemaPrikazivanjaDetailsViewModel(PodshemaPrikazivanja podshemaPrikazivanja, IEnumerable<Koristi> prezentacije)
        {
            PodshemaPrikazivanja = podshemaPrikazivanja;
            Prezentacije = prezentacije;
        }


    }
}
