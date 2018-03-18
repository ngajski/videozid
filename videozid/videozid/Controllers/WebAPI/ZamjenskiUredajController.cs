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
using Microsoft.Extensions.Logging;

namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Web API servis za rad sa zamjenskim uređajima.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class ZamjenskiUredajController : Controller
    {
        private readonly RPPP15Context _context;
        private ILogger _logger;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        /// <param name="logger">>Loger koji zapisuje u bazu podataka</param>
        public ZamjenskiUredajController(RPPP15Context context, ILogger<ServisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ZamjenskiUredaj
        /// <summary>
        /// Postupak za dohvat svih zamjenskih uređaja. 
        /// </summary>
        /// <returns>Popis svih zamjenskih uređaja sortiran po ID-u</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ZamjenskiUredajApiModel>), Description = "Vraća enumeraciju zamjenskih uređaja")]
        public async Task<IEnumerable<ZamjenskiUredajApiModel>> ZamjenskiUredajiGet()
        {
            var list = await _context.ZamjenskiUredaj
                                .Include(z => z.IdUredajaNavigation)
                                .Include(z => z.IdZamjenaZaNavigation)
                                .Select(d => new ZamjenskiUredajApiModel
                                {
                                    Id = d.Id,
                                    IdUredaja = d.IdUredaja,
                                    Uredaj = d.IdUredajaNavigation.Naziv,
                                    IdZamjenaZa = d.IdZamjenaZa,
                                    Zamjena = d.IdZamjenaZaNavigation.Naziv,
                                })
                                .ToListAsync();
            list.Sort((a, b) => a.Id > b.Id ? 0 : 1);
            return list;
        }

        // GET api/ZamjenskiUredaj/ID
        /// <summary>
        /// Postupak za dohvat zamjenskog uređaja čiji je ID jednak poslanom parametru.
        /// </summary>
        /// <param name="idUredaja">ID zamjenskog uređaja</param>
        /// <returns>objekt tipa ZamjenskiUredajApiModel ili NotFound s pripadajućom porukom ako zamjenski uređaj s traženim ID-em ne postoji</returns>
        [HttpGet("{IdUredaja}", Name = "DohvatiZamjenskiUredaj")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(ZamjenskiUredajApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ZamjenskiUredajGet(int idUredaja)
        {
            var uredaj = await _context.ZamjenskiUredaj
                                        .AsNoTracking()
                                        .Where(u => u.Id == idUredaja)
                                        .FirstOrDefaultAsync();

            if (uredaj == null)
            {
                return NotFound("Zamjenski uređaj s navedenim ID-em ne postoji.");
            }
            else
            {
                var model = new ZamjenskiUredajApiModel
                {
                    Id = uredaj.Id,
                    IdUredaja = uredaj.IdUredaja,
                    IdZamjenaZa = uredaj.IdZamjenaZa
                };

                return new ObjectResult(model);
            }
        }

        // POST api/ZamjenskiUredaj
        /// <summary>
        /// Postupak kojim se deklarira novi zamjenski uređaj.
        /// </summary>
        /// <param name="zamjenski">Podaci o novom zamjenskom uređaju</param>
        /// <returns>201 ako je zamjenski uređaj uspješno pohranjen, te se ujedno vraća i spremljeni objekt, BadRequest s u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ZamjenskiUredajCreate([FromBody] ZamjenskiUredaj zamjenski)
        {
            if(zamjenski.IdUredaja == zamjenski.IdZamjenaZa)
            {
                _logger.LogWarning("Uređaj ne može biti sam sebi zamjena!");
                return BadRequest("Uređaj ne može biti sam sebi zamjena!");
            }
            if (await _context.Uredaj.FindAsync(zamjenski.IdZamjenaZa) == null)
            {
                _logger.LogWarning("Uređaj s navedenim ID-em (" + zamjenski.IdZamjenaZa + ") ne postoji.");
                return NotFound("Uređaj s navedenim ID-em (" + zamjenski.IdZamjenaZa + ") ne postoji.");
            }

            else if (await _context.Uredaj.FindAsync(zamjenski.IdUredaja) == null)
            {
                _logger.LogWarning("Uređaj s navedenim ID-em (" + zamjenski.IdUredaja + ") ne postoji.");
                return NotFound("Uređaj s navedenim ID-em (" + zamjenski.IdUredaja + ") ne postoji.");
            }

            else if (await _context.ZamjenskiUredaj.AsNoTracking().Where(u => u.IdUredaja == zamjenski.IdUredaja && u.IdZamjenaZa == zamjenski.IdZamjenaZa).FirstOrDefaultAsync() != null)
            {
                _logger.LogWarning(_context.Uredaj.Where(u => u.Id == zamjenski.IdUredaja).First().Naziv + " (" + zamjenski.IdUredaja + ") je već definiran kao zamjenski za uređaj " + _context.Uredaj.Where(u => u.Id == zamjenski.IdZamjenaZa).First().Naziv + " (" + zamjenski.IdZamjenaZa + ")");
                return BadRequest(_context.Uredaj.Where(u => u.Id == zamjenski.IdUredaja).First().Naziv + " (" + zamjenski.IdUredaja + ") je već definiran kao zamjenski za uređaj " + _context.Uredaj.Where(u => u.Id == zamjenski.IdZamjenaZa).First().Naziv + " (" + zamjenski.IdZamjenaZa + ")");
            }
            else if(await _context.ZamjenskiUredaj.AsNoTracking().Where(u => u.IdUredaja == zamjenski.IdZamjenaZa && u.IdZamjenaZa == zamjenski.IdUredaja).FirstOrDefaultAsync() != null)
            {
                _logger.LogWarning(_context.Uredaj.Where(u => u.Id == zamjenski.IdZamjenaZa).First().Naziv + " (" + zamjenski.IdZamjenaZa + ") je već definiran kao zamjenski za uređaj " + _context.Uredaj.Where(u => u.Id == zamjenski.IdUredaja).First().Naziv + " (" + zamjenski.IdUredaja + ")");
                return BadRequest(_context.Uredaj.Where(u => u.Id == zamjenski.IdZamjenaZa).First().Naziv + " (" + zamjenski.IdZamjenaZa + ") je već definiran kao zamjenski za uređaj " + _context.Uredaj.Where(u => u.Id == zamjenski.IdUredaja).First().Naziv + " (" + zamjenski.IdUredaja + ")");
            }

            else
            {
                _context.Add(zamjenski);
                await _context.SaveChangesAsync();
                var za = _context.ZamjenskiUredaj.Where(z => z.IdUredaja == zamjenski.IdUredaja && z.IdZamjenaZa == zamjenski.IdZamjenaZa).Include(z => z.IdUredajaNavigation).Include(z => z.IdZamjenaZaNavigation).First();
                _logger.LogInformation("Stvoren zamjenski uređaj ID = " + za.Id);
                return new ObjectResult(new
                {
                    IdVeze = za.Id,
                    Id1 = za.IdUredaja,
                    Id2 = za.IdZamjenaZa,
                    N1 = za.IdUredajaNavigation.Naziv,
                    N2 = za.IdZamjenaZaNavigation.Naziv,
                });
            }
        }

        // PUT api/ZamjenskiUredaj/Update
        /// <summary>
        /// Ažurira zamjenski uređaj s ID-em uređaja IdUredaja temeljem parametera iz zahtjeva.
        /// </summary>
        /// <param name="model">model tipa ZamjenskiUredajApiModel s novim podacima o zamjenskom uređaju</param>
        /// <returns>204 ako je uređaj uspješno ažuriran, 404 ako uređaj s traženom šifrom ne postoji ili BadRequest ako model nije ispravan</returns>
        [HttpPut]
        [ActionName("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ZamjenskiUredajUpdate([FromBody] ZamjenskiUredajApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning(ModelState.ToString());
                return BadRequest(ModelState);
            }
            else
            {
                var uredaj = await _context.ZamjenskiUredaj.FindAsync(model.Id);
                if (uredaj == null || await _context.Uredaj.FindAsync(model.IdUredaja) == null || await _context.Uredaj.FindAsync(model.IdZamjenaZa) == null)
                {
                    _logger.LogWarning("Traženi uređaj ne postoji");
                    return NotFound("Traženi uređaj ne postoji");
                }
                else if(uredaj.IdUredaja == uredaj.IdZamjenaZa)
                {
                    _logger.LogWarning("Uređaj ne može biti sam sebi zamjena!");
                    return BadRequest("Uređaj ne može biti sam sebi zamjena!");
                }
                else
                {
                    uredaj.IdUredaja = model.IdUredaja;
                    uredaj.IdZamjenaZa = model.IdZamjenaZa;

                    _logger.LogInformation("Uređan zajenski uređaj" + uredaj.Id);
                    await _context.SaveChangesAsync();
                    return NoContent();
                };
            }
        }

        // DELETE api/ZamjenskiUredaj/ID
        /// <summary>
        /// Briše zamjenski uređaj s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdUredaja">ID zamjenskog uređaja kojeg treba obrisati</param>
        /// <returns>NotFound ako uređaj nije pronađen ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdUredaja}")]
        [ActionName("Remove")]
        public async Task<IActionResult> ZamjenskiUredajDelete(int IdUredaja)
        {
            var zamjena = await _context.ZamjenskiUredaj
                                       .AsNoTracking()
                                       .Where(u => u.Id == IdUredaja)
                                       .FirstOrDefaultAsync();

            if (zamjena == null)
            {
                _logger.LogWarning("Ne postoji odnos uređaj-zamjenski uređaj između dva navedena uređaja.");
                return NotFound("Ne postoji odnos uređaj-zamjenski uređaj između dva navedena uređaja.");
            }

            _context.Remove(zamjena);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
