using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace videozid.Models
{
    /// <summary>
    /// Podatkovni model ekrana videozida
    /// </summary>
    public partial class EkranZida
    {
        /// <summary>
        /// ID ekrana videozida
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID ekrana videozida u tablici Uređaj
        /// </summary>
        public int IdUredaja { get; set; }

        /// <summary>
        /// Id videozida kojem pripada ovaj ekran
        /// </summary>
        public int IdZida { get; set; }

        /// <summary>
        /// X koordinata na kojoj se ekran zida nalazi
        /// </summary>
        [Range(0, 100)]
        public int XKoord { get; set; }

        /// <summary>
        /// Y koordinata na kojoj se ekran zida nalazi
        /// </summary>
        [Range(0, 100)]
        public int YKoord { get; set; }

        /// <summary>
        /// IdUredajaNavigation
        /// </summary>
        public virtual Uredaj IdUredajaNavigation { get; set; }

        /// <summary>
        /// IdZidaNavigation
        /// </summary>
        public virtual Videozid IdZidaNavigation { get; set; }
    }
}
