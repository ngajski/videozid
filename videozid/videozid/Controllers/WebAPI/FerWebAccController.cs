using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using videozid.ViewModels.Api;
using System.Net;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Web API servis za rad s FerWeb računima
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class FerWebAccController : Controller
    {
        private readonly RPPP15Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">VideozidContext za pristup bazi podataka</param>
        public FerWebAccController(RPPP15Context context)
        {
            this.context = context;
        }

        // GET: api/FerWebAcc
        /// <summary>
        /// Postupak za dohvat svih FerWeb računa. 
        /// </summary>
        /// <returns>Popis svih FerWeb računa, sortiran po korisničkom imenu</returns>
        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IEnumerable<IndexAccApiModel>> FerWebGet()
        {
            var racuni = await context.Korisnik.Where(k => k.Fer != null)
            .Select(r => new IndexAccApiModel
            {
                Id = r.Fer.Id,
                KorisnickoIme = r.Fer.KorisnickoIme,
                Lozinka = r.Fer.Lozinka,
                DozvolaServer = r.Fer.DozvolaServer,
                Vlasnik = r.KorisnickoIme
            })
            .ToListAsync();
            racuni.OrderBy(k => k.KorisnickoIme);

            return racuni;
        }

        // GET api/FerWebAcc/KorisnickoIme
        /// <summary>
        /// Postupak za dohvat FerWeb računa čije je korisničko ime jednako poslanom parametru
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime</param>
        /// <returns>objekt tipa FerWebAccApiModel ili NotFound ako račun s traženim korisničkim imenom ne postoji</returns>
        [HttpGet("{KorisnickoIme}", Name = "DohvatiRacun1")]
        [ProducesResponseType(typeof(IndexAccApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ActionName("Get")]
        public async Task<IActionResult> RacunGet(string KorisnickoIme)
        {
            var racun = await context.FerWebAcc
                                  .AsNoTracking()
                                  .Where(r => r.KorisnickoIme == KorisnickoIme)
                                  .FirstOrDefaultAsync();
            if (racun == null)
            {
                return NotFound("Traženi račun ne postoji");
            }
            else
            {
                var vlasnik = await context.Korisnik.AsNoTracking().Where(k => k.FerId.Equals(racun.Id))
                                              .FirstOrDefaultAsync();
                                       
                var model = new IndexAccApiModel
                {
                    Id = racun.Id,
                    Lozinka = racun.Lozinka,
                    KorisnickoIme = racun.KorisnickoIme,
                    DozvolaServer = racun.DozvolaServer,
                    Vlasnik = vlasnik.KorisnickoIme
                };
                return new ObjectResult(model);
            }
        }

        // POST api/FerWebAcc
        /// <summary>
        /// Postupak kojim se unosi novi FerWeb račun
        /// </summary>
        /// <param name="PripadaKorisniku">Pripada korisniku</param>
        /// <param name="model">Podaci o novom računu. Pod vlasnik se unosi korisničko ime korisnika kojemu želimo stvoriti novi FerWeb račun</param>
        /// <returns>201 ako je račun uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela ili korisnik već posjeduje FerWeb račun</returns>
        [HttpPost("{PripadaKorisniku}")]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RacunCreate(string PripadaKorisniku, [FromBody] AccNewApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                
                Korisnik vlasnik= null;
                vlasnik = await context.Korisnik.AsNoTracking().Where(k => k.KorisnickoIme.Equals(PripadaKorisniku))
                                            .FirstOrDefaultAsync();

                if(vlasnik == null)
                {
                    var tekst = string.Format("Korisnik kojem treba pripasti račun ne postoji");
                    return BadRequest(tekst);
                }
                

                FerWebAcc racun = new FerWebAcc
                {
                    DozvolaServer = model.DozvolaServer,
                    Lozinka = model.Lozinka,
                    KorisnickoIme = model.KorisnickoIme
                };



                if (vlasnik.FerId != null)
                {
                    var tekst = string.Format("Korisnik {0} već ima FerWeb račun!!", vlasnik.KorisnickoIme);
                    return BadRequest(tekst);

                }
                else
                {
                    context.Add(racun);
                    await context.SaveChangesAsync();

                    vlasnik.FerId = racun.Id;
                    context.Update(vlasnik);
                    context.Update(racun);
                    await context.SaveChangesAsync();
                }

                IndexAccApiModel modelVrati = new IndexAccApiModel
                {
                    DozvolaServer = racun.DozvolaServer,
                    KorisnickoIme = racun.KorisnickoIme,
                    Lozinka = racun.Lozinka,
                    Vlasnik = vlasnik.KorisnickoIme,
                    Id = racun.Id
                };

                return CreatedAtRoute("DohvatiRacun1", new { KorisnickoIme = racun.KorisnickoIme }, modelVrati);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        // DELETE api/FerWebAcc/KorisnickoIme
        /// <summary>
        /// Briše račun s predanim korisničkim imenom u adresi zahtjeva
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime FerWeb računa koje je potrebno obrisati</param>
        /// <returns>404 ako nije obrisan ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{KorisnickoIme}")]
        [ActionName("Remove")]
        public async Task<IActionResult> FerWebDelete(string KorisnickoIme)
        {

            var racun = await context.FerWebAcc.SingleOrDefaultAsync(m => m.KorisnickoIme.Equals(KorisnickoIme));

            if (racun == null)
            {
                return NotFound("Traženi račun ne postoji");
            }
            else
            {
                //makni foreign key
                var korisnik = context.Korisnik.Where(k => k.Fer.KorisnickoIme.Equals(racun.KorisnickoIme)).First();
                korisnik.FerId = null;
                context.Korisnik.Update(korisnik);
                await context.SaveChangesAsync();

                //obrisi racun
                context.FerWebAcc.Remove(racun);
                await context.SaveChangesAsync();

                return NoContent();
            };
        }

        // PUT api/FerWebAcc/KorisnickoIme
        /// <summary>
        /// Ažurira FerWeb račun s temeljem parametera iz zahtjeva (korisničko ime)
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime FerWeb računa kojeg je potrebno ažurirati</param>
        /// <param name="model">model s podacima o FerWeb računu</param>
        /// <returns>204 ako je račun uspješno ažuriran, 404 ako račun s traženim korisničkim imenom ne postoji ili 400 ako model nije ispravan</returns>
        [HttpPut("{KorisnickoIme}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ActionName("Update")]
        public async Task<IActionResult> FerWebUpdate(string KorisnickoIme, [FromBody] AccEditApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var racun = await context.FerWebAcc.SingleOrDefaultAsync(m => m.KorisnickoIme.Equals(KorisnickoIme));

                if (racun == null)
                {
                    return NotFound("Traženi račun ne postoji");
                }
                else
                {
                    racun.Lozinka = model.Lozinka;
                    racun.DozvolaServer = model.DozvolaServer;

                    await context.SaveChangesAsync();
                    return NoContent();
                };
            }
        }

    }
}
