using System;
using System.Collections.Generic;

namespace videozid.Models
{
    /// <summary>
    /// Podatkovni model Zamjenskog uređaja
    /// </summary>
    public partial class ZamjenskiUredaj
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID zamjenskog uređaja u tablici Uređaj
        /// </summary>
        public int IdUredaja { get; set; }

        /// <summary>
        /// ID uređaja u tablici Uređaj kojemu je ovaj uređaj zamjena
        /// </summary>
        public int IdZamjenaZa { get; set; }

        /// <summary>
        /// Navigacijski atribut
        /// </summary>
        public virtual Uredaj IdUredajaNavigation { get; set; }
        
        /// <summary>
        /// Navigacijski atributs
        /// </summary>
        public virtual Uredaj IdZamjenaZaNavigation { get; set; }
    }
}
