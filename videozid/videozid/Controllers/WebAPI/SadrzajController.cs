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
    /// Web API servis za rad sa sadržajima.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class SadrzajController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        public SadrzajController(RPPP15Context context)
        {
            _context = context;
        }

        // GET: api/Sadrzaj
        /// <summary>
        /// Postupak za dohvat svih sadržaja. 
        /// </summary>
        /// <returns>Popis svih sadržaja</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<SadrzajApiModel>), Description = "Vraća enumeraciju sadrzaja")]
        public async Task<IEnumerable<SadrzajApiModel>> SadrzajiGet()
        {
            var list = await _context.Sadrzaj
                                        .Include(u => u.IdAutoraNavigation)
                                        .Include(u => u.IdOdobrenOdNavigation)
                                        .Include(u => u.IdTipaNavigation)
                                        .Select(d => new SadrzajApiModel
                                        {
                                            Id = d.Id,
                                            Ime = d.Ime,
                                            Opis = d.Opis,
                                            Url = d.Url,
                                            JeOdobren = d.JeOdobren,
                                            IdAutora = d.IdAutora,
                                            Autor = d.IdAutoraNavigation.Ime + " " + d.IdAutoraNavigation.Prezime,
                                            IdOdobrenOd = d.IdOdobrenOd,
                                            OdobrenOd = d.IdOdobrenOdNavigation.Ime + " " + d.IdOdobrenOdNavigation.Prezime,
                                            IdTipa = d.IdTipa,
                                            Tip = d.IdTipaNavigation.Ime,
                                            Prezentacija = _context.Prezentacija.Include(e => e.IdSadrzajaNavigation).Where(e => e.IdSadrzaja == d.Id).Select(e => new MasterDetailHelper { Id = e.Id }).ToList()
                                        })
                                        .ToListAsync();
            return list;
        }

        // GET api/Sadrzaj/ID
        /// <summary>
        /// Postupak za dohvat sadržaja čiji je ID jednak poslanom parametru.
        /// </summary>
        /// <param name="idSadrzaja">ID sadržaja</param>
        /// <returns>objekt tipa SadrzajApiModel ili NotFound ako sadržaj s traženim ID-em ne postoji</returns>
        [HttpGet("{IdSadrzaja}", Name = "DohvatiSadrzaj")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(SadrzajApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SadrzajGet(int idSadrzaja)
        {
            var sadrzaj = await _context.Sadrzaj
                                        .AsNoTracking()
                                        .Include(d => d.IdAutoraNavigation)
                                        .Include(d => d.IdOdobrenOdNavigation)
                                        .Include(d => d.IdTipaNavigation)
                                        .Where(d => d.Id == idSadrzaja)
                                        .FirstOrDefaultAsync();

            if(sadrzaj == null)
            {
                return NotFound("Sadržaj s traženim ID-om ne postoji");
            } else
            {
                return new ObjectResult(new SadrzajApiModel
                {
                    Id = sadrzaj.Id,
                    Ime = sadrzaj.Ime,
                    Opis = sadrzaj.Opis,
                    Url = sadrzaj.Url,
                    JeOdobren = sadrzaj.JeOdobren,
                    IdAutora = sadrzaj.IdAutora,
                    Autor = sadrzaj.IdAutoraNavigation.Ime + " " + sadrzaj.IdAutoraNavigation.Prezime,
                    IdOdobrenOd = sadrzaj.IdOdobrenOd,
                    OdobrenOd = sadrzaj.IdOdobrenOdNavigation.Ime + " " + sadrzaj.IdOdobrenOdNavigation.Prezime,
                    IdTipa = sadrzaj.IdTipa,
                    Tip = sadrzaj.IdTipaNavigation.Ime,
                    Prezentacija = _context.Prezentacija.Where(p => p.IdSadrzaja == sadrzaj.Id).Select(s => new MasterDetailHelper { Id = s.Id }).ToList()
                });
            }
        }


        // POST api/Sadrzaj
        /// <summary>
        /// Postupak kojim se unosi novi sadržaj.
        /// </summary>
        /// <param name="model">Podaci o novom sadržaju</param>
        /// <returns>201 ako je sadržaj uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SadrzajCreate([FromBody] SadrzajApiModel model)
        {
            
                if (model != null && ModelState.IsValid)
                {
                    Sadrzaj sadrzaj = new Sadrzaj
                    {
                        Ime = model.Ime,
                        Opis = model.Opis,
                        Url = model.Url,
                        JeOdobren = model.JeOdobren,
                        IdAutora = model.IdAutora,
                        IdOdobrenOd = model.IdOdobrenOd,
                        IdTipa = model.IdTipa
                    };

                    _context.Add(sadrzaj);
                    await _context.SaveChangesAsync();
                    var u = await _context.Sadrzaj
                                            .AsNoTracking()
                                            .Include(d => d.IdAutoraNavigation)
                                            .Include(d => d.IdOdobrenOdNavigation)
                                            .Include(d => d.IdTipaNavigation)
                                            .LastOrDefaultAsync();

                    return new ObjectResult(new SadrzajApiModel
                    {
                        Id = u.Id,
                        Ime = u.Ime,
                        Opis = u.Opis,
                        Url = u.Url,
                        JeOdobren = u.JeOdobren,
                        IdAutora = u.IdAutora,
                        Autor = u.IdAutoraNavigation.Ime + " " + u.IdAutoraNavigation.Prezime,
                        IdOdobrenOd = u.IdOdobrenOd,
                        OdobrenOd = u.IdOdobrenOdNavigation.Ime + " " + u.IdOdobrenOdNavigation.Prezime,
                        IdTipa = u.IdTipa,
                        Tip = u.IdTipaNavigation.Ime
                    });
                }
                return BadRequest(ModelState);
            
        }

        // PUT api/Sadrzaj
        /// <summary>
        /// Ažurira sadržaj s parametrom ID iz zahtjeva.
        /// </summary>
        /// <param name="IdSadrzaja">ID sadržaja koji treba ažurirati</param>
        /// <param name="model">model s podacima o sadržaju</param>
        /// <returns>204 ako je sadržaj uspješno ažuriran, 404 ako uređaj s traženom šifrom ne postoji ili 400 ako model nije ispravan</returns>
        [HttpPut]
        [ActionName("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SadrzajUpdate([FromBody] SadrzajApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var sadrzaj = await _context.Sadrzaj.FindAsync(model.Id);
                if (sadrzaj == null)
                {
                    return NotFound("Traženi sadržaj ne postoji(ID = " + model.Id + ")");
                }
                else
                {
                    sadrzaj.Ime = model.Ime;
                    sadrzaj.Opis = model.Opis;
                    sadrzaj.Url = model.Url;
                    sadrzaj.JeOdobren = model.JeOdobren;
                    sadrzaj.IdAutora = model.IdAutora;
                    sadrzaj.IdOdobrenOd = model.IdOdobrenOd;
                    sadrzaj.IdTipa = model.IdTipa;

                    await _context.SaveChangesAsync();
                    return NoContent();

                };
            }
        }

        // DELETE api/Sadrzaj/ID
        /// <summary>
        /// Briše sadržaj s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdSadrzaja">oznaka sadržaja koji treba obrisati</param>
        /// <returns>404 ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdSadrzaja}")]
        [ActionName("Remove")]
        public async Task<IActionResult> SadrzajDelete(int IdSadrzaja)
        {
            var sadrzaj = await _context.Sadrzaj.FindAsync(IdSadrzaja);
            if(sadrzaj == null)
            {
                return NotFound("Traženi sadržaj ne postoji(ID = " + IdSadrzaja + ")");
            }
            else
            {
                _context.Remove(sadrzaj);
                _context.RemoveRange(_context.Prezentacija.Where(z => z.IdSadrzaja == sadrzaj.Id));

                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
