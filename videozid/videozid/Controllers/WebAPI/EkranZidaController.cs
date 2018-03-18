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
    /// Web API servis za rad s ekranima Videozida.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class EkranZidaController : Controller
    {
        private readonly RPPP15Context _context;
        private ILogger _logger;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        /// /// <param name="logger">>Loger koji zapisuje u bazu podataka</param>
        public EkranZidaController(RPPP15Context context, ILogger<ServisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/EkranZida
        /// <summary>
        /// Postupak za dohvat svih ekrana videozidova. 
        /// </summary>
        /// <returns>Popis svih ekrana sortiran po Id-u</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<EkranZidaApiModel>), Description = "Vraća enumeraciju ekrana videozidova")]
        public async Task<IEnumerable<EkranZidaApiModel>> EkraniGet()
        {
            var list = await _context.EkranZida
                                .Include(e => e.IdUredajaNavigation)
                                .Include(e => e.IdZidaNavigation)
                                .Select(d => new EkranZidaApiModel
                                {
                                    Id = d.Id,
                                    IdUredaja = d.IdUredaja,
                                    Uredaj = d.IdUredajaNavigation.Naziv,
                                    IdZida = d.IdZida,
                                    Zid = d.IdZidaNavigation.Naziv,
                                    XKoord = d.XKoord,
                                    YKoord = d.YKoord
                                })
                                .ToListAsync();
            list.Sort((a, b) =>  a.Id > b.Id ? 0 : 1);
            return list;
        }

        // GET api/Ekran/ID
        /// <summary>
        /// Postupak za dohvat ekrana čiji je ID jednak poslanom parametru.
        /// </summary>
        /// <param name="IdEkrana">ID ekrana</param>
        /// <returns>objekt tipa EkranZidaApiModel ili NotFound ako ekran videozida s traženim Id-em ne postoji</returns>
        [HttpGet("{IdEkrana}", Name = "DohvatiEkran")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(EkranZidaApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> EkranGet(int IdEkrana)
        {
            var ekran = await _context.EkranZida
                                        .AsNoTracking()
                                        .Where(u => u.Id == IdEkrana)
                                        .FirstOrDefaultAsync();

            if (ekran == null)
            {
                _logger.LogWarning("Ekran videozida s navedenim ID-em ne postoji.");
                return NotFound("Ekran videozida s navedenim ID-em ne postoji.");
            }
            else
            {
                var model = new EkranZidaApiModel
                {
                    Id = ekran.Id,
                    IdUredaja = ekran.IdUredaja,
                    IdZida = ekran.IdZida,
                    XKoord = ekran.XKoord,
                    YKoord = ekran.YKoord
                };

                return new ObjectResult(model);
            }
        }

        // GET: api/Ekran/IDZida
        /// <summary>
        /// Postupak za dohvat svih ekrana jednog videozida. 
        /// </summary>
        /// <returns>Popis svih ekrana sortiran po Id-u</returns>
        [HttpGet("{IdZida}", Name = "DohvatiEkraneZida")]
        [ActionName("GetForWall")]
        [ProducesResponseType(typeof(IEnumerable<EkranZidaApiModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> EkraniZidaGet(int IdZida)
        {
            var zid = await _context.Videozid
                                        .AsNoTracking()
                                        .Where(u => u.Id == IdZida)
                                        .FirstOrDefaultAsync();

            if (zid == null)
            {
                _logger.LogWarning("Videozid s navedenim ID-em ne postoji.");
                return NotFound("Videozid s navedenim ID-em ne postoji.");
            }
            else
            {
                var list = await _context.EkranZida
                                .Where(e => e.IdZida == IdZida)
                                .Select(d => new EkranZidaApiModel
                                {
                                    Id = d.Id,
                                    IdUredaja = d.IdUredaja,
                                    IdZida = d.IdZida,
                                    XKoord = d.XKoord,
                                    YKoord = d.YKoord
                                })
                                .ToListAsync();
                list.Sort((a, b) => a.Id > b.Id ? 0 : 1);
                return new ObjectResult(list);
            }
        }

        // POST api/Ekran
        /// <summary>
        /// Postupak kojim se unosi novi ekran videozida.
        /// </summary>
        /// <param name="ekran">Podaci o novom ekranu</param>
        /// <returns>201 ako je ekran videozida uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EkranCreate([FromBody] EkranZida ekran)
        {
            if (ekran.XKoord < 0 || ekran.YKoord < 0)
            {
                _logger.LogWarning("Neispravne koordinate!");
                return BadRequest("Neispravne koordinate!");
            }

            else if (await _context.Uredaj.FindAsync(ekran.IdUredaja) == null)
            {
                _logger.LogWarning("Uređaj s navedenim ID-em (" + ekran.IdUredaja + ") ne postoji.");
                return NotFound("Uređaj s navedenim ID-em (" + ekran.IdUredaja + ") ne postoji.");
            }

            else if (await _context.Videozid.FindAsync(ekran.IdZida) == null)
            {
                _logger.LogWarning("Videozid s navedenim ID-em (" + ekran.IdZida + ") ne postoji.");
                return NotFound("Videozid s navedenim ID-em (" + ekran.IdZida + ") ne postoji.");
            }

            else if (await _context.EkranZida.Where(ek => ek.IdUredaja == ekran.IdUredaja && ek.IdZida == ekran.IdZida).FirstOrDefaultAsync() != null)
            {
                _logger.LogWarning("Videozid već sadrži navedeni ekran!");
                return BadRequest("Videozid već sadrži navedeni ekran!");
            }

            else if (await _context.EkranZida.Where(ek => ek.IdUredaja == ekran.IdUredaja && ek.IdZida == ekran.IdZida).FirstOrDefaultAsync() != null)
            {
                _logger.LogWarning("Neki videozid već sadrži navedeni ekran!");
                return BadRequest("Neki videozid već sadrži navedeni ekran!");
            }

            else if (await _context.EkranZida.Where(ek => ek.XKoord == ekran.XKoord && ek.YKoord == ekran.YKoord && ek.IdZida == ekran.IdZida).FirstOrDefaultAsync() != null)
            {
                _logger.LogWarning("Videozid već sadrži ekran na navedenim koordinatama!");
                return BadRequest("Videozid već sadrži ekran na navedenim koordinatama!");
            }

            else
            {
                var zid = await _context.Videozid.FindAsync(ekran.IdZida);

                if (ekran.XKoord >= zid.Sirina || ekran.YKoord >= zid.Visina)
                {
                    _logger.LogWarning("Neispravne koordinate!");
                    return BadRequest("Neispravne koordinate!");
                }

                _context.Add(ekran);
                await _context.SaveChangesAsync();

                var ek = _context.EkranZida.Where(e => e.IdUredaja == ekran.IdUredaja && e.IdZida == ekran.IdZida).Include(e => e.IdUredajaNavigation).First();
                _logger.LogInformation("Stvoren ekran zida ID = " + ek.Id);
                return new ObjectResult(new
                {
                    IdVeze = ek.Id,
                    Id = ek.IdUredaja,
                    Name = ek.IdUredajaNavigation.Naziv + "(" + ek.XKoord + "," + ek.YKoord + ")",
                });
            }
        }

        // PUT api/Ekran
        /// <summary>
        /// Ažurira ekran videozida s ID-em IdEkrana temeljem parametera iz zahtjeva.
        /// </summary>
        /// <param name="model">model s podacima o ekranu zida</param>
        /// <returns>204 ako je ekran videozida uspješno ažuriran, NotFound ako ekran s traženom šifrom ne postoji ili BadRequest ako model nije ispravan</returns>
        [HttpPut]
        [ActionName("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EkranUpdate([FromBody] EkranZidaApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning(ModelState.ToString());
                return BadRequest(ModelState);
            }
            else
            {
                var ekran = await _context.EkranZida.FindAsync(model.Id);
                if (ekran == null)
                {
                    _logger.LogWarning("Traženi ekran videozida ne postoji");
                    return NotFound("Traženi ekran videozida ne postoji");
                }
                else
                {
                    ekran.IdUredaja = model.IdUredaja;
                    ekran.IdZida = model.IdZida;
                    ekran.XKoord = model.XKoord;
                    ekran.YKoord = model.YKoord;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Ekran ažuriran ID = " + ekran.Id);
                    return NoContent();
                };
            }
        }

        // DELETE api/Ekran/ID
        /// <summary>
        /// Briše ekran videozida s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdEkrana">ID ekrana videozida kojeg treba obrisati</param>
        /// <returns>NotFound ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdEkrana}")]
        [ActionName("Remove")]
        public async Task<IActionResult> EkranDelete(int IdEkrana)
        {
            var ekran = await _context.EkranZida
                                       .AsNoTracking()
                                       .Where(u => u.Id == IdEkrana)
                                       .FirstOrDefaultAsync();

            if (ekran == null)
            {
                _logger.LogWarning("Ne postoji odnos ekran-videozid između dva navedena parametra.");
                return NotFound("Ne postoji odnos ekran-videozid između dva navedena parametra.");
            }

            _context.Remove(ekran);
            _logger.LogInformation("Ekran obrisan.");
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
