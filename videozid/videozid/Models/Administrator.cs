using System;
using System.Collections.Generic;

namespace videozid.Models
{
    /// <summary>
    /// Administrator (šifrarnik)
    /// </summary>
    public partial class Administrator
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id Korisnika koji je administrator
        /// </summary>
        public int IdKorisnika { get; set; }

        /// <summary>
        /// Navigacija korisnikom preko id-a njegove tablice
        /// </summary>
        public virtual Korisnik IdKorisnikaNavigation { get; set; }
    }
}
