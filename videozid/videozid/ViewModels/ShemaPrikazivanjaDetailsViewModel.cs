using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using videozid.Models;

namespace videozid.ViewModels
{
    public class ShemaPrikazivanjaDetailsViewModel
    {
        public List<Sadrzi> podsheme { get; set; }
        public List<Sadrzi> sadrzi { get; set; }

        public ShemaPrikazivanja ShemaPrikazivanja { get; set; }
        public IEnumerable<Sadrzi> Podsheme { get; set; }

       

        public ShemaPrikazivanjaDetailsViewModel(ShemaPrikazivanja shemaPrikazivanja, IEnumerable<Sadrzi> podsheme)
        {
            ShemaPrikazivanja = shemaPrikazivanja;
            Podsheme = podsheme;
        }

       
    }
}
