using System;
using System.Collections.Generic;

namespace videozid.Models
{
    public partial class Sadrzi
    {
        public int Id { get; set; }
        public int IdSheme { get; set; }
        public int IdPodsheme { get; set; }
        public TimeSpan Od { get; set; }
        public TimeSpan Do { get; set; }

        public virtual PodshemaPrikazivanja IdPodshemeNavigation { get; set; }
        public virtual ShemaPrikazivanja IdShemeNavigation { get; set; }
    }
}
