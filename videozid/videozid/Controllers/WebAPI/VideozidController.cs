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
    /// Web API servis za rad s videozidom.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class VideozidController : Controller
    {
        private readonly RPPP15Context _context;
        private ILogger _logger;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        /// /// <param name="logger">>Loger koji zapisuje u bazu podataka</param>
        public VideozidController(RPPP15Context context, ILogger<ServisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Videozid
        /// <summary>
        /// Postupak za dohvat svih videozidova. 
        /// </summary>
        /// <returns>Popis svih videozidova sortiran po nazivu</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<VideozidApiModel>), Description = "Vraća enumeraciju videozidova")]
        public async Task<IEnumerable<VideozidApiModel>> VideozidiGet()
        {
            var list = await _context.Videozid
                                .Select(d => new VideozidApiModel
                                {
                                    Id = d.Id,
                                    Naziv = d.Naziv,
                                    Lokacija = d.Lokacija,
                                    Sirina = d.Sirina,
                                    Visina = d.Visina,
                                    //Ekrani = _context.EkranZida.Include(e => e.IdUredajaNavigation).Where(e => e.IdZida == d.Id).Select(e => new MasterDetailHelper {IdVeze = e.Id, Id = e.IdUredajaNavigation.Id, Name = e.IdUredajaNavigation.Naziv }).ToList()
                                })
                                .ToListAsync();
            list.Sort((a, b) => a.Naziv.CompareTo(b.Naziv));
            return list;
        }

        // GET api/Videozid/ID
        /// <summary>
        /// Postupak za dohvat videozida čiji je ID jednak primljenom parametru.
        /// </summary>
        /// <param name="idZida">ID videozida</param>
        /// <returns>objekt tipa VideozidApiModel ili NotFound ako videozid s traženom oznakom ne postoji</returns>
        [HttpGet("{IdZida}", Name = "DohvatiVideozid")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(VideozidApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> VideozidGet(int idZida)
        {
            var videozid = await _context.Videozid
                                        .AsNoTracking()
                                        .Where(u => u.Id == idZida)
                                        .FirstOrDefaultAsync();

            if (videozid == null)
            {
                _logger.LogWarning("Videozid s navedenim ID-em ne postoji.");
                return NotFound("Videozid s navedenim ID-em ne postoji.");
            }
            else
            {
                var model = new VideozidApiModel
                {
                    Id = videozid.Id,
                    Naziv = videozid.Naziv,
                    Lokacija = videozid.Lokacija,
                    Sirina = videozid.Sirina,
                    Visina = videozid.Visina,
                    Ekrani = _context.EkranZida.Include(e => e.IdUredajaNavigation).Where(e => e.IdZida == videozid.Id).Select(e => new MasterDetailHelper { IdVeze = e.Id, Id = e.IdUredaja, Name = e.IdUredajaNavigation.Naziv + "(" + e.XKoord + "," + e.YKoord + ")" }).ToList()
                };

                return new ObjectResult(model);
            }
        }

        // POST api/Videozid
        /// <summary>
        /// Postupak kojim se stvara novi videozid.
        /// </summary>
        /// <param name="model">Podaci o novom videozidu</param>
        /// <returns>201 ako je videozid uspješno pohranjen te se ujedno vraća i spremljeni objekt, BadRequest u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> VideozidCreate([FromBody] VideozidApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                Videozid uredaj = new Videozid
                {
                    Naziv = model.Naziv,
                    Lokacija = model.Lokacija,
                    Sirina = model.Sirina,
                    Visina = model.Visina,
                };

                _context.Add(uredaj);
                await _context.SaveChangesAsync();

                var videozid = await _context.Videozid
                                        .AsNoTracking()
                                        .LastOrDefaultAsync();

                return new ObjectResult(new VideozidApiModel {
                    Id = videozid.Id,
                    Naziv = videozid.Naziv,
                    Lokacija = videozid.Lokacija,
                    Visina = videozid.Visina,
                    Sirina = videozid.Sirina,
                });
            }

            _logger.LogWarning(ModelState.ToString());
            return BadRequest(ModelState);
        }
        /*
        // POST api/Videozid
        /// <summary>
        /// Postupak kojim se dodaje novi ekran postojećem videozidu.
        /// </summary>
        /// <param name="ekran">Objekt tipa EkranZida koji definira ID-eve uređaja i videozida te koordinate pomoću kojih se stvara veza ekrana videozida</param>
        /// <returns>201 ako je ekran uspješno pohranjen, NotFound ako uređaj ili videozid s traženim ID-em ne postoji, BadRequest u slučaju neispravnog paramtera</returns>
        [HttpPost]
        [ActionName("AddEkran")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddEkran([FromBody] EkranZida ekran)
        {
            if(ekran.XKoord < 0 || ekran.YKoord < 0)
            {
                return BadRequest("Neispravne koordinate!");
            }

            else if (await _context.Uredaj.FindAsync(ekran.IdUredaja) == null)
            {
                return NotFound("Uređaj s navedenim ID-em (" + ekran.IdUredaja + ") ne postoji.");
            }

            else if (await _context.Videozid.FindAsync(ekran.IdZida) == null)
            {
                return NotFound("Videozid s navedenim ID-em (" + ekran.IdZida + ") ne postoji.");
            }

            else if (await _context.EkranZida.Where(ek => ek.IdUredaja == ekran.IdUredaja && ek.IdZida == ekran.IdZida).FirstOrDefaultAsync() != null)
            {
                return BadRequest("Videozid već sadrži navedeni ekran!");
            }

            else if (await _context.EkranZida.Where(ek => ek.IdUredaja == ekran.IdUredaja && ek.IdZida == ekran.IdZida).FirstOrDefaultAsync() != null)
            {
                return BadRequest("Neki videozid već sadrži navedeni ekran!");
            }

            else if(await _context.EkranZida.Where(ek => ek.XKoord == ekran.XKoord && ek.YKoord == ekran.YKoord && ek.IdZida == ekran.IdZida).FirstOrDefaultAsync() != null)
            {
                return BadRequest("Videozid već sadrži ekran na navedenim koordinatama!");
            }
            
            else
            {
                var zid = await _context.Videozid.FindAsync(ekran.IdZida);

                if(ekran.XKoord >= zid.Sirina || ekran.YKoord >= zid.Visina)
                {
                    return BadRequest("Neispravne koordinate!");
                }

                _context.Add(ekran);
                await _context.SaveChangesAsync();

                var ek = _context.EkranZida.Where(e => e.IdUredaja == ekran.IdUredaja && e.IdZida == ekran.IdZida).Include(e => e.IdUredajaNavigation).First();

                return new ObjectResult(new {
                    IdVeze = ek.Id,
                    Id = ek.IdUredaja,
                    Name = ek.IdUredajaNavigation.Naziv + "(" + ek.XKoord + "," + ek.YKoord + ")",
                });
            }
        }*/

        // PUT api/Videozid
        /// <summary>
        /// Ažurira videozid novim vrijednostima temeljem parametera iz zahtjeva.
        /// </summary>
        /// <param name="model">model s podacima o videozidu</param>
        /// <returns>204 ako je videozid uspješno ažuriran, NotFound ako videozid s traženom šifrom ne postoji ili BadRequest ako model nije ispravan</returns>
        [ActionName("Update")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> VideozidUpdate([FromBody] VideozidApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning(ModelState.ToString());
                return BadRequest(ModelState);
            }
            else
            {
                var videozid = await _context.Videozid.FindAsync(model.Id);
                if (videozid == null)
                {
                    _logger.LogWarning("Traženi videozid ne postoji");
                    return NotFound("Traženi videozid ne postoji");
                }
                else
                {

                    videozid.Naziv = model.Naziv;
                    videozid.Lokacija = model.Lokacija;
                    videozid.Sirina = model.Sirina;
                    videozid.Visina = model.Visina;

                    foreach(var ekran in await _context.EkranZida.Where(e => e.IdZida == videozid.Id).ToListAsync())
                    {
                        if (!(ekran.XKoord < videozid.Sirina) || !(ekran.YKoord < videozid.Visina))
                            _context.Remove(ekran);
                    }

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Videozid ažuriran ID = " + videozid.Id);
                    return NoContent();
                };
            }
        }

        // DELETE api/Videozid/ID
        /// <summary>
        /// Briše videozid s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdZida">oznaka videozida kojeg treba obrisati</param>
        /// <returns>NotFound ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdZida}")]
        [ActionName("Remove")]
        public async Task<IActionResult> VideozidDelete(int IdZida)
        {
            var videozid = await _context.Videozid.FindAsync(IdZida);
            if (videozid == null)
            {
                _logger.LogWarning("Traženi videozid ne postoji");
                return NotFound("Traženi videozid ne postoji");
            }
            else
            {
                _context.Remove(videozid);
                _context.RemoveRange(_context.EkranZida.Where(e => e.IdZida == videozid.Id));
                _context.RemoveRange(_context.Prikazuje.Where(p => p.IdZida == videozid.Id));

                await _context.Uredaj.Where(u => u.IdZida == videozid.Id).ForEachAsync(u => {u.IdZida = null; u.IdStatusa = 0; });

                await _context.SaveChangesAsync();
                _logger.LogInformation("Videozid obrisan.");
                return NoContent();
            };
        }
        /*
        // DELETE api/Videozid/
        /// <summary>
        /// Briše ekran s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="idVeze">ID veze koju treba obrisati</param>
        /// <returns>NotFound ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{idVeze}")]
        [ActionName("RemoveEkran")]
        public async Task<IActionResult> EkranDelete(int idVeze)
        {
            var ekran = await _context.EkranZida
                                       .AsNoTracking()
                                       .Where(u => u.Id == idVeze)
                                       .FirstOrDefaultAsync();

            if (ekran == null)
            {
                return NotFound("Ne postoji odnos ekran-videozid između dva navedena parametra.");
            }

            _context.Remove(ekran);
            await _context.SaveChangesAsync();
            return NoContent();
        }*/
    }
}
