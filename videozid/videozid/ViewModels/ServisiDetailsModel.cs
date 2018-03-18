using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    public class ServisiDetailsModel
    {
        public IEnumerable<ServisDetailsViewModel> Servisi { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public ServisFilter filter { get; set; }

        
        public ServisiDetailsModel(IEnumerable<ServisDetailsViewModel> ServisiDetails, PagingInfo Info)
        {
            this.Servisi = ServisiDetails;
            this.PagingInfo = Info;
        }
    }
}
