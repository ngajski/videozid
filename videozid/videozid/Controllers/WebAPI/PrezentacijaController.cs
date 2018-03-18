using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using videozid.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using videozid.ViewModels.Api;

namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Web API servis za rad s prezentacijama.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class PrezentacijaController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        public PrezentacijaController(RPPP15Context context)
        {
            _context = context;
        }

        // GET: api/Prezentacija
        /// <summary>
        /// Postupak za dohvat svih prezentacija. 
        /// </summary>
        /// <returns>Popis svih prezentacija</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<PrezentacijaApiModel>), Description = "Vraća enumeraciju sadrzaja")]
        public async Task<IEnumerable<PrezentacijaApiModel>> PrezentacijeGet()
        {
            var list = await _context.Prezentacija
                                    .Include(u => u.IdKategorijeNavigation)
                                    .Include(u => u.IdSadrzajaNavigation)
                                    .Select(d => new PrezentacijaApiModel
                                    {
                                        Id = d.Id,
                                        XKoord = d.XKoord,
                                        YKoord = d.YKoord,
                                        Sirina = d.Sirina,
                                        Visina = d.Visina,
                                        IdSadrzaja = d.IdSadrzaja,
                                        Sadrzaj = d.IdSadrzajaNavigation.Ime == null ? "/" : d.IdSadrzajaNavigation.Ime,
                                        IdKategorije = d.IdKategorije,
                                        Kategorija = d.IdKategorijeNavigation.Vrsta == null ? "/" : d.IdKategorijeNavigation.Vrsta
                                    })
                                    .ToListAsync();
            return list;
        }

        // GET api/Prezentacija/ID
        /// <summary>
        /// Postupak za dohvat prezentacije čiji je ID jednak poslanom parametru.
        /// </summary>
        /// <param name="idPrezentacije">ID prezentacije</param>
        /// <returns>objekt tipa PrezentacijaApiModel ili NotFound ako prezentacija s traženim ID-em ne postoji</returns>
        [HttpGet("{IdPrezentacije}", Name = "DohvatiPrezentaciju")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(PrezentacijaApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PrezentacijaGet(int idPrezentacije)
        {
            var prezentacija = await _context.Prezentacija
                                                .AsNoTracking()
                                                .Include(d => d.IdKategorijeNavigation)
                                                .Include(d => d.IdSadrzajaNavigation)
                                                .Where(d => d.Id == idPrezentacije)
                                                .FirstOrDefaultAsync();

            if(prezentacija == null)
            {
                return NotFound("Prezentacija s traženim ID-om ne postoji");
            } else
            {
                return new ObjectResult(new PrezentacijaApiModel
                {
                    Id = prezentacija.Id,
                    XKoord = prezentacija.XKoord,
                    YKoord = prezentacija.YKoord,
                    Sirina = prezentacija.Sirina,
                    Visina = prezentacija.Visina,
                    IdSadrzaja = prezentacija.IdSadrzaja,
                    Sadrzaj = prezentacija.IdSadrzajaNavigation.Ime == null ? "/" : prezentacija.IdSadrzajaNavigation.Ime,
                    IdKategorije = prezentacija.IdKategorije,
                    Kategorija = prezentacija.IdKategorijeNavigation.Vrsta == null ? "/" : prezentacija.IdKategorijeNavigation.Vrsta,
                });
            }
        }

        // POST api/Prezentacija
        /// <summary>
        /// Postupak kojim se unosi nova prezentacija.
        /// </summary>
        /// <param name="model">Podaci o novoj prezentaciji</param>
        /// <returns>201 ako je prezentacija uspješno pohranjena, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PrezentacijaCreate([FromBody] PrezentacijaApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                Prezentacija prezentacija = new Prezentacija
                {
                    XKoord = model.XKoord,
                    YKoord = model.YKoord,
                    Sirina = model.Sirina,
                    Visina = model.Visina,
                    IdSadrzaja = model.IdSadrzaja,
                    IdKategorije = model.IdKategorije,
                };

                _context.Add(prezentacija);
                await _context.SaveChangesAsync();
                var u = await _context.Prezentacija
                                        .AsNoTracking()
                                        .Include(d => d.IdKategorijeNavigation)
                                        .Include(d => d.IdSadrzajaNavigation)
                                        .LastOrDefaultAsync();

                return new ObjectResult(new PrezentacijaApiModel
                {
                    Id = u.Id,
                    XKoord = u.XKoord,
                    YKoord = u.YKoord,
                    Sirina = u.Sirina,
                    Visina = u.Visina,
                    IdSadrzaja = u.IdSadrzaja,
                    Sadrzaj = u.IdSadrzaja == null ? "/" : u.IdSadrzajaNavigation.Ime,
                    IdKategorije = u.IdKategorije,
                    Kategorija = u.IdKategorije == null ? "/" : u.IdKategorijeNavigation.Vrsta
                });
            }

            return BadRequest(ModelState);
        }

        // PUT api/Prezentacija
        /// <summary>
        /// Ažurira prezentaciju s parametrom ID iz zahtjeva.
        /// </summary>
        /// <param name="IdPrezentacije">ID prezentacije koju treba ažurirati</param>
        /// <param name="model">model s podacima o prezentaciji</param>
        /// <returns>204 ako je prezentacija uspješno ažurirana, 404 ako prezentacija s traženom šifrom ne postoji ili 400 ako model nije ispravan</returns>
        [HttpPut]
        [ActionName("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PrezentacijaUpdate([FromBody] PrezentacijaApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var prezentacija = await _context.Prezentacija.FindAsync(model.Id);
                if(prezentacija == null)
                {
                    return NotFound("Tražena prezentacija ne postoji(ID = " + model.Id + ")");
                } 
                else
                {
                    prezentacija.XKoord = model.XKoord;
                    prezentacija.YKoord = model.YKoord;
                    prezentacija.Sirina = model.Sirina;
                    prezentacija.Visina = model.Visina;
                    prezentacija.IdKategorije = model.IdKategorije;
                    prezentacija.IdSadrzaja = model.IdSadrzaja;

                    await _context.SaveChangesAsync();
                    return NoContent();
                };
            }
        }

        // DELETE api/Prezentacija/ID
        /// <summary>
        /// Briše prezentaciju s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdPrezentacije">oznaka prezentacije koju treba obrisati</param>
        /// <returns>404 ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdPrezentacije}")]
        [ActionName("Remove")]
        public async Task<IActionResult> PrezentacijaDelete(int IdPrezentacije)
        {
            var prezentacija = await _context.Prezentacija.FindAsync(IdPrezentacije);
            if(prezentacija == null)
            {
                return NotFound("Tražena prezentacija ne postoji(ID = " + IdPrezentacije + ")");
            }
            else
            {
                _context.Remove(prezentacija);
                _context.RemoveRange(_context.Koristi.Where(z => z.IdPrezentacije == prezentacija.Id));

                await _context.SaveChangesAsync();
                return NoContent();
            }

        }
    }
}
