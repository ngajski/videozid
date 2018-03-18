using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using Microsoft.EntityFrameworkCore;

namespace videozid.Controllers
{
    /// <summary>
    /// Upravljač za rad s administratorima
    /// </summary>
    public class AdministratorController : Controller
    {
        private readonly RPPP15Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">VideozidContext za pristup bazi podataka</param>
        public AdministratorController(RPPP15Context context)
        {
            this.context = context;
        }

        //Index 
        /// <summary>
        /// Postupak za dohvat liste administratora, koja se prosljeđuje pogledu
        /// </summary>
        /// <returns>Lista administratora</returns>
        public async Task<IActionResult> Index()
        {
            var admini = context.Administrator.Include(fk => fk.IdKorisnikaNavigation);
            return View(await admini.ToListAsync());
        }

        
    }
}
