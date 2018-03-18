using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using videozid.ViewModels.Api;
using System.Net;
using System.Diagnostics;
using System;
using videozid.ViewModels;

namespace Firma.Mvc.Controllers.WebApi
{
    /// <summary>
    /// Web API servis za rad s korisničkim računima
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class KorisnikController : Controller
    {
        private readonly RPPP15Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">VideozidContext za pristup bazi podataka</param>
        public KorisnikController(RPPP15Context context)
        {
            this.context = context;
        }


        // GET: api/korisnik
        /// <summary>
        /// Postupak za dohvat svih korisnika. 
        /// </summary>
        /// <returns>Popis svih korisnika sortiranih po prezimenu</returns>
        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IEnumerable<KorisnikApiModel>> KorisniciGet()
        {
            var admini = context.Administrator.Select(a => a.IdKorisnikaNavigation.KorisnickoIme).ToList();
       
            var korisnici = await context.Korisnik
            .Select(k => new KorisnikApiModel
            {
                Id = k.Id,
                Ime = k.Ime,
                Prezime = k.Prezime,
                Email = k.Email,
                Lozinka = k.Lozinka,
                KorisnickoIme = k.KorisnickoIme,
                Admin = false
            })
            .ToListAsync();
            korisnici.OrderBy(k => k.Prezime);

            foreach(var k in korisnici)
            {
                if (admini.Contains(k.KorisnickoIme))
                {
                    k.Admin = true;
                }
            }
           
            return korisnici;
        }

        // GET: api/korisnik
        /// <summary>
        /// Postupak za dohvat svih korisnika bez Dhmz računa. 
        /// </summary>
        /// <returns>Popis svih korisnika bez Dhmz računa sortiran po korisničkom imenu</returns>
        [HttpGet]
        [ActionName("WithoutDhmz")]
        public async Task<IEnumerable<KorisnikApiModel>> WithoutDhmzGet()
        {

            var korisnici = await context.Korisnik.Where(k => k.DhmzId==null)
            .Select(k => new KorisnikApiModel
            {
                Id = k.Id, 
                Ime = k.Ime,
                Prezime = k.Prezime,
                Email = k.Email,
                Lozinka = k.Lozinka,
                KorisnickoIme = k.KorisnickoIme,
                Admin = false
            })
            .ToListAsync();
            korisnici.OrderBy(k => k.KorisnickoIme);

            return korisnici;
        }

        // GET: api/korisnik
        /// <summary>
        /// Postupak za dohvat svih korisnika bez FerWweb računa. 
        /// </summary>
        /// <returns>Popis svih korisnika bez FerWeb računa sortiran po korisničkom imenu</returns>
        [HttpGet]
        [ActionName("WithoutFer")]
        public async Task<IEnumerable<KorisnikApiModel>> WithoutFerGet()
        {

            var korisnici = await context.Korisnik.Where(k => k.FerId == null)
            .Select(k => new KorisnikApiModel
            {
                Id = k.Id,
                Ime = k.Ime,
                Prezime = k.Prezime,
                Email = k.Email,
                Lozinka = k.Lozinka,
                KorisnickoIme = k.KorisnickoIme,
                Admin = false
            })
            .ToListAsync();
            korisnici.OrderBy(k => k.KorisnickoIme);

            return korisnici;
        }

        // GET api/Korisnik/KorisnickoIme
        /// <summary>
        /// Postupak za dohvat korisnika čije je korisničko ime jednako poslanom parametru
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime</param>
        /// <returns>objekt tipa KorisnikApiModel ili NotFound ako korisnik s traženim korisničkim imenom ne postoji</returns>
        [HttpGet("{KorisnickoIme}", Name = "DohvatiKorisnika")]
        [ProducesResponseType(typeof(KorisnikApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ActionName("Get")]
        public async Task<IActionResult> KorisnikGet(string KorisnickoIme)
        {
            var administratori = context.Administrator.Select(a => a.IdKorisnikaNavigation.KorisnickoIme);
            var admin = false;

            if (administratori.Contains(KorisnickoIme)) { 
                admin = true;
            }
            var korisnik = await context.Korisnik
                                  .AsNoTracking()
                                  .Where(k => k.KorisnickoIme == KorisnickoIme)
                                  .FirstOrDefaultAsync();
            if (korisnik == null)
            {
                return NotFound("Traženi korisnik ne postoji");
            }
            else
            {
                var model = new KorisnikApiModel
                {
                    Id = korisnik.Id,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Email = korisnik.Email,
                    Lozinka = korisnik.Lozinka,
                    KorisnickoIme = korisnik.KorisnickoIme,
                    Admin = admin
                };
                return new ObjectResult(model);
            }
        }

        // GET api/Korisnik/KorisnickoIme
        /// <summary>
        /// Postupak za dohvat korisnika čije je korisničko ime jednako poslanom parametru
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime</param>
        /// <returns>objekt tipa KorisnikApiModel ili NotFound ako korisnik s traženim korisničkim imenom ne postoji</returns>
        [HttpGet("{KorisnickoIme}", Name = "DohvatiKorisnikaMD")]
        [ProducesResponseType(typeof(KorisnikMDApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ActionName("GetMD")]
        public async Task<IActionResult> KorisnikMDGet(string KorisnickoIme)
        {
            var administratori = context.Administrator.Select(a => a.IdKorisnikaNavigation.KorisnickoIme);
            var admin = false;

            if (administratori.Contains(KorisnickoIme))
            {
                admin = true;
            }
            var korisnik = await context.Korisnik
                                  .AsNoTracking()
                                  .Where(k => k.KorisnickoIme == KorisnickoIme)
                                  .FirstOrDefaultAsync();
            if (korisnik == null)
            {
                return NotFound("Traženi korisnik ne postoji");
            }
            else
            {
                var model = new KorisnikMDApiModel
                {
                    Id = korisnik.Id,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Email = korisnik.Email,
                    Lozinka = korisnik.Lozinka,
                    KorisnickoIme = korisnik.KorisnickoIme,
                    Admin = admin,
                    Autor = context.Sadrzaj.Include(e => e.IdAutoraNavigation).Where(e => e.IdAutora == korisnik.Id).Select(e => new KorisnikMasterDetailHelper { Id = e.Id, Name = e.Ime }).ToList(),
                    Odobrio = context.Sadrzaj.Include(e => e.IdAutoraNavigation).Where(e => e.IdOdobrenOd == korisnik.Id).Select(e => new KorisnikMasterDetailHelper { Id = e.Id, Name = e.Ime }).ToList()
                };
                return new ObjectResult(model);
            }
        }

        // POST api/Korisnik
        /// <summary>
        /// Postupak kojim se unosi novi korisnik
        /// </summary>
        /// <param name="model">Podaci o novom korisniku</param>
        /// <returns>201 ako je korisnik uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ActionName("New")]
        public async Task<IActionResult> KorisnikCreate([FromBody] KorisnikApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                Korisnik korisnik = new Korisnik
                {
                    Ime = model.Ime,
                    Prezime = model.Prezime,
                    Email = model.Email,
                    Lozinka = model.Lozinka,
                    KorisnickoIme = model.KorisnickoIme
                };

                //ovdje bi slijedila validacija u poslovnom sloju prije snimanja (da smo napravili takav sloj)
                context.Add(korisnik);
                await context.SaveChangesAsync();

                return CreatedAtRoute("DohvatiKorisnika", new { KorisnickoIme = korisnik.KorisnickoIme }, model);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        // DELETE api/Korisnik/KorisnickoIme
        /// <summary>
        /// Briše korisnika s predanim korisničkim imenom u adresi zahtjeva
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime korisnika kojeg je potrebno obrisati</param>
        /// <returns>404 ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{KorisnickoIme}")]
        [ActionName("Remove")]
        public async Task<IActionResult> KorisnikDelete(string KorisnickoIme)
        {
            //maknuti ga s admin liste
            Administrator admin = null;
            try
            {
                admin = context.Administrator.Where(a => a.IdKorisnikaNavigation.KorisnickoIme.Equals(KorisnickoIme)).First();
            }catch(Exception e)
            {

            }
            if(admin != null)
            {
                context.Administrator.Remove(admin);
                context.SaveChanges();
            }

            

            var korisnik = context.Korisnik.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).ToList().First();
            if (korisnik == null)
            {
                return NotFound("Traženi korisnik ne postoji");
            }
            else
            {
                context.Remove(korisnik);
                await context.SaveChangesAsync();
                return NoContent();
            };
        }

        // PUT api/Korisnik
        /// <summary>
        /// Ažurira korisnika s temeljem parametera iz zahtjeva (korisničko ime)
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime korisnika kojeg je potrebno ažurirati</param>
        /// <param name="model">model s podacima o korisniku</param>
        /// <returns>204 ako je korisnik uspješno ažuriran, 404 ako korisnik s traženim korisničkim imenom ne postoji ili 400 ako model nije ispravan</returns>
        [HttpPut("{KorisnickoIme}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(string KorisnickoIme, [FromBody] KorisnikApiModel model)
        {
            if (model == null ||  !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var korisnik = context.Korisnik.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).ToList().First();
                if (korisnik == null)
                {
                    return NotFound("Traženi korisnik ne postoji");
                }
                else
                {
                    korisnik.Ime = model.Ime;
                    korisnik.Prezime = model.Prezime;
                    korisnik.Email = model.Email;
                    korisnik.Lozinka = model.Lozinka;
                    korisnik.KorisnickoIme = model.KorisnickoIme;

                    await context.SaveChangesAsync();
                    return NoContent();
                };
            }
        }
    }
}

