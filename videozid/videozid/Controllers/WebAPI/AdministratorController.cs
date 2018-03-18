using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using videozid.ViewModels.Api;
using System.Net;

namespace Firma.Mvc.Controllers.WebApi
{
    /// <summary>
    /// Web API servis za rad s korisničkim računima
    /// </summary>
    [Route("api/[controller]/[action]")]
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

        // POST api/Administrator
        /// <summary>
        /// Postupak kojim se dodaje novi administrator
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime korisnika koji će postati administrator</param>
        /// <returns>201 ako je korisnik uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost("{KorisnickoIme}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ActionName("New")]
        public async Task<IActionResult> AdminCreate(string KorisnickoIme)
        {
            var korisnik = context.Korisnik.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).ToList().First();
            if (korisnik == null)
            {
                return NotFound("Traženi korisnik ne postoji");
            }
            else
            {
                Administrator admin = new Administrator();
                admin.IdKorisnika = korisnik.Id;

                context.Administrator.Add(admin);
                context.SaveChanges();
                return NoContent();
            };

        }

        // PUT api/Administrator
        /// <summary>
        /// Postupak kojim se ažurira administrator
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime korisnika koji će postati administrator</param>
        /// <returns>201 ako je korisnik uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPut("{KorisnickoIme}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ActionName("Update")]
        public async Task<IActionResult> AdminUpdate(string KorisnickoIme)
        {
            var korisnik = context.Korisnik.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).ToList().First();
            if (korisnik == null)
            {
                return NotFound("Traženi korisnik ne postoji");
            }
            else
            {
                Administrator admin = new Administrator();
                admin.IdKorisnika = korisnik.Id;

                context.Administrator.Add(admin);
                context.SaveChanges();
                return NoContent();
            };

        }
    }
}

