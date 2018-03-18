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
    /// Web API servis za rad s tipovima sadržaja.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class TipSadrzajaController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        public TipSadrzajaController(RPPP15Context context)
        {
            _context = context;
        }

        // GET: api/TipSadrzaja
        /// <summary>
        /// Postupak za dohvat svih tipova sadržaja. 
        /// </summary>
        /// <returns>Popis svih tipova sadržaja</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<TipSadrzajaApiModel>), Description = "Vraća enumeraciju tipova sadrzaja")]
        public async Task<IEnumerable<TipSadrzajaApiModel>> TipSadrzajaGet()
        {
            var list = await _context.TipSadrzaja
                                    .Select(d => new TipSadrzajaApiModel
                                    {
                                        Id = d.Id,
                                        Ime = d.Ime
                                    })
                                    .ToListAsync();

            return list;
        }

        // GET api/TipSadrzaja/ID
        /// <summary>
        /// Postupak za dohvat tipa sadrzaja čiji je ID jednak poslanom parametru.
        /// </summary>
        /// <param name="idTipSadrzaja">ID tipa sadrzaja</param>
        /// <returns>objekt tipa TipSadrzajApiModel ili NotFound ako kategorija s traženom oznakom ne postoji</returns>
        [HttpGet("{idTipSadrzaja}", Name = "DohvatiTipSadrzaja")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(TipSadrzajaApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> TipSadrzajaGet(int idTipSadrzaja)
        {
            var tip = await _context.TipSadrzaja
                                            .AsNoTracking()
                                            .Where(d => d.Id == idTipSadrzaja)
                                            .FirstOrDefaultAsync();
            if (tip == null)
            {
                return NotFound("Kategorija s trazenim ID-om ne postoji");
            }
            else
            {
                var model = new TipSadrzajaApiModel
                {
                    Id = tip.Id,
                    Ime = tip.Ime
                };

                return new ObjectResult(tip);
            }
        }

        // POST api/TipSadrzaja
        /// <summary>
        /// Postupak kojim se unosi novi tip sadržaja.
        /// </summary>
        /// <param name="model">Podaci o novom tipu</param>
        /// <returns>201 ako je tip uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> TipSadrzajaCreate([FromBody] TipSadrzajaApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                TipSadrzaja tip = new TipSadrzaja
                {
                    Ime = model.Ime,
                };

                _context.Add(tip);
                await _context.SaveChangesAsync();
                var u = await _context.TipSadrzaja
                                        .AsNoTracking()
                                        .LastOrDefaultAsync();
                return new ObjectResult(new TipSadrzajaApiModel
                {
                   Id = u.Id,
                   Ime = u.Ime,
                });
            }

            return BadRequest(ModelState);
        }


        // DELETE api/TipSadrzaja/ID
        /// <summary>
        /// Briše tip sadržaja s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdTipSadrzaja">ID tipa koji treba obrisati</param>
        /// <returns>404 ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdTipSadrzaja}")]
        [ActionName("Remove")]
        public async Task<IActionResult> TipSadrzajaDelete(int IdTipSadrzaja)
        {
            var tip = await _context.TipSadrzaja.FindAsync(IdTipSadrzaja);
            if (tip == null)
            {
                return NotFound("Traženi tip ne postoji");
            }
            else
            {
                _context.Remove(tip);
                await _context.SaveChangesAsync();
                return NoContent();
            };
        }
    }
}
