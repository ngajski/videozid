using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.ViewModels
{
    /// <summary>
    /// Sadrži osnovne informacije koje entitet pruža o objektima koje sadrži
    /// </summary>
    public class MasterDetailHelper
    {

        /// <summary>
        /// ID Veze
        /// </summary>
        public int IdVeze { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ime
        /// </summary>
        public string Name { get; set; }
    }
}
