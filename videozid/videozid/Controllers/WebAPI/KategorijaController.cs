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
    /// Web API servis za rad s kategorijama.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class KategorijaController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        public KategorijaController(RPPP15Context context)
        {
            _context = context;
        }

        // GET: api/Kategorija
        /// <summary>
        /// Postupak za dohvat svih kategorija. 
        /// </summary>
        /// <returns>Popis svih kategorija</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<KategorijaApiModel>), Description = "Vraća enumeraciju kategorija")]
        public async Task<IEnumerable<KategorijaApiModel>> KategorijeGet()
        {
            var list = await _context.Kategorija
                                    .Select(d => new KategorijaApiModel
                                    {
                                        Id = d.Id,
                                        Vrsta = d.Vrsta
                                    })
                                    .ToListAsync();
            
            return list;
        }

        // GET api/Kategorija/ID
        /// <summary>
        /// Postupak za dohvat kategorije čiji je ID jednak poslanom parametru.
        /// </summary>
        /// <param name="idKategorije">ID kategorije</param>
        /// <returns>objekt tipa KategorijaApiModel ili NotFound ako kategorija s traženom oznakom ne postoji</returns>
        [HttpGet("{idKategorije}", Name = "DohvatiKategoriju")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(KategorijaApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> KategorijaGet(int idKategorije)
        {
            var kategorija = await _context.Kategorija
                                            .AsNoTracking()
                                            .Where(d => d.Id == idKategorije)
                                            .FirstOrDefaultAsync();
            if(kategorija == null)
            {
                return NotFound("Kategorija s trazenim ID-om ne postoji");
            }
            else
            {
                var model = new KategorijaApiModel
                {
                    Id = kategorija.Id,
                    Vrsta = kategorija.Vrsta
                };

                return new ObjectResult(kategorija);
            }
        }

        // POST api/Kategorija
        /// <summary>
        /// Postupak kojim se unosi nova kategorija.
        /// </summary>
        /// <param name="model">Podaci o novoj kategoriji</param>
        /// <returns>201 ako je kategorija uspješno pohranjena, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> KategorijaCreate([FromBody] KategorijaApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                Kategorija kat = new Kategorija
                {
                    Vrsta = model.Vrsta
                };

                _context.Add(kat);
                await _context.SaveChangesAsync();

                var kategorija = await _context.Kategorija
                                        .AsNoTracking()
                                        .LastOrDefaultAsync();

                return new ObjectResult(new KategorijaApiModel
                {
                    Id = kategorija.Id,
                    Vrsta = kategorija.Vrsta
                });
            }

            return BadRequest(ModelState);
        }

        // PUT api/Kategorija
        /// <summary>
        /// Ažurira kategoriju s ID-em kategorije temeljem parametera iz zahtjeva.
        /// </summary>
        /// <param name="IdKategorije">ID kategorije koju treba ažurirati</param>
        /// <param name="model">model s podacima o ekranu zida</param>
        /// <returns>204 ako je kategorija uspješno ažurirana, 404 ako ekran s traženom šifrom ne postoji ili 400 ako model nije ispravan</returns>
        [HttpPut("{IdKategorije}")]
        [ActionName("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> KategorijaUpdate(int IdKategorije, [FromBody] KategorijaApiModel model)
        {
            if (model == null || model.Id != IdKategorije || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var kategorija = await _context.Kategorija.FindAsync(IdKategorije);
                if (kategorija == null)
                {
                    return NotFound("Tražena kategorija ne postoji");
                }
                else
                { 
                
                    //kategorija.Id = model.Id;
                    kategorija.Vrsta = model.Vrsta;

                    await _context.SaveChangesAsync();
                    return NoContent();
                };
            }
        }

        // DELETE api/Kategorija/ID
        /// <summary>
        /// Briše kategoriju s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdKategorije">ID kategorije koju treba obrisati</param>
        /// <returns>404 ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdKategorije}")]
        [ActionName("Remove")]
        public async Task<IActionResult> KategorijanDelete(int IdKategorije)
        {
            var kategorija = await _context.Kategorija.FindAsync(IdKategorije);
            if (kategorija == null)
            {
                return NotFound("Tražena kategorija ne postoji");
            }
            else
            {
                _context.Remove(kategorija);
                await _context.SaveChangesAsync();
                return NoContent();
            };
        }

    }
}
