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


namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Web API servis za rad s DHMZ računima
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class DhmzAccController : Controller
    {
        private readonly RPPP15Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">VideozidContext za pristup bazi podataka</param>
        public DhmzAccController(RPPP15Context context)
        {
            this.context = context;
        }

        // GET: api/DhmzAcc
        /// <summary>
        /// Postupak za dohvat svih DHMZ računa. 
        /// </summary>
        /// <returns>Popis svih DHMZ računa, sortiran po korisničkom imenu</returns>
        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IEnumerable<IndexAccApiModel>> DhmzGet()
        {
            var racuni = await context.Korisnik.Where(k => k.Dhmz != null)
            .Select(r => new IndexAccApiModel
            {
                Id = r.Dhmz.Id,
                KorisnickoIme = r.Dhmz.KorisnickoIme,
                Lozinka = r.Dhmz.Lozinka,
                DozvolaServer = r.Dhmz.DozvolaServer,
                Vlasnik = r.KorisnickoIme
            })
            .ToListAsync();
            racuni.OrderBy(k => k.KorisnickoIme);

            return racuni;
        }

        // GET api/DhmzAcc/KorisnickoIme
        /// <summary>
        /// Postupak za dohvat DHMZ računa čije je korisničko ime jednako poslanom parametru
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime</param>
        /// <returns>objekt tipa IndexAccApiModel ili NotFound ako račun s traženim korisničkim imenom ne postoji</returns>
        [HttpGet("{KorisnickoIme}", Name = "DohvatiRacun2")]
        [ProducesResponseType(typeof(IndexAccApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ActionName("Get")]
        public async Task<IActionResult> RacunGet(string KorisnickoIme)
        {
            
            var racun = await context.DhmzAcc
                                  .AsNoTracking()
                                  .Where(r => r.KorisnickoIme == KorisnickoIme)
                                  .FirstOrDefaultAsync();
            if (racun == null)
            {
                return NotFound("Traženi račun ne postoji");
            }
            else
            {
                var vlasnik = await context.Korisnik.AsNoTracking().Where(k => k.DhmzId.Equals(racun.Id))
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

        // POST api/DhmzAcc/PripadaKorisniku
        /// <summary>
        /// Postupak kojim se unosi novi Dhmz račun
        /// </summary>
        /// <param name="PripadaKorisniku">Pripada korisniku</param>
        /// <param name="model">Podaci o novom računu. Pod vlasnik se unosi korisničko ime korisnika kojemu želimo stvoriti novi Dhmz račun</param>
        /// <returns>201 ako je račun uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela ili korisnik već posjeduje Dhmz račun</returns>
        [HttpPost("{PripadaKorisniku}")]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RacunCreate(string PripadaKorisniku, [FromBody] AccNewApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                Korisnik vlasnik = null;
                vlasnik = await context.Korisnik.AsNoTracking().Where(k => k.KorisnickoIme.Equals(PripadaKorisniku))
                                            .FirstOrDefaultAsync();

                if (vlasnik == null)
                {
                    var tekst = string.Format("Korisnik kojem treba pripasti račun ne postoji");
                    return BadRequest(tekst);
                }


                DhmzAcc racun = new DhmzAcc
                {
                    DozvolaServer = model.DozvolaServer,
                    Lozinka = model.Lozinka,
                    KorisnickoIme = model.KorisnickoIme
                };



                if (vlasnik.DhmzId != null)
                {
                    var tekst = string.Format("Korisnik {0} već ima Dhmz račun!!", vlasnik.KorisnickoIme);
                    return BadRequest(tekst);

                }
                else
                {
                    context.Add(racun);
                    await context.SaveChangesAsync();

                    vlasnik.DhmzId = racun.Id;
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

                return CreatedAtRoute("DohvatiRacun2", new { KorisnickoIme = racun.KorisnickoIme }, modelVrati);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        // DELETE api/DhmzAcc/KorisnickoIme
        /// <summary>
        /// Briše račun s predanim korisničkim imenom u adresi zahtjeva
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime Dhmz računa koje je potrebno obrisati</param>
        /// <returns>404 ako nije obrisan ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{KorisnickoIme}")]
        [ActionName("Remove")]
        public async Task<IActionResult> DhmzDelete(string KorisnickoIme)
        {

            var racun = await context.DhmzAcc.SingleOrDefaultAsync(m => m.KorisnickoIme.Equals(KorisnickoIme));

            if (racun == null)
            {
                return NotFound("Traženi račun ne postoji");
            }
            else
            {
                //makni foreign key
                var korisnik = context.Korisnik.Where(k => k.Dhmz.KorisnickoIme.Equals(racun.KorisnickoIme)).First();
                korisnik.DhmzId = null;
                context.Korisnik.Update(korisnik);
                await context.SaveChangesAsync();

                //obrisi racun
                context.DhmzAcc.Remove(racun);
                await context.SaveChangesAsync();

                return NoContent();
            };
        }

        // PUT api/DhmzAcc/KorisnickoIme
        /// <summary>
        /// Ažurira Dhmz račun s temeljem parametera iz zahtjeva (korisničko ime)
        /// </summary>
        /// <param name="KorisnickoIme">Korisničko ime Dhmz računa kojeg je potrebno ažurirati</param>
        /// <param name="model">model s podacima o Dhmz računu</param>
        /// <returns>204 ako je račun uspješno ažuriran, 404 ako račun s traženim korisničkim imenom ne postoji ili 400 ako model nije ispravan</returns>
        [HttpPut("{KorisnickoIme}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ActionName("Update")]
        public async Task<IActionResult> DhmzUpdate(string KorisnickoIme, [FromBody] AccEditApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var racun = await context.DhmzAcc.SingleOrDefaultAsync(m => m.KorisnickoIme.Equals(KorisnickoIme));

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
