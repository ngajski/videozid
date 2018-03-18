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
    /// Web API servis za rad s uređajima.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class UredajController : Controller
    {
        private readonly RPPP15Context _context;
        private ILogger _logger;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">RPPP15Context za pristup bazi podataka</param>
        /// /// <param name="logger">>Loger koji zapisuje u bazu podataka</param>
        public UredajController(RPPP15Context context, ILogger<ServisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Uredaj
        /// <summary>
        /// Postupak za dohvat svih uređaja. 
        /// </summary>
        /// <returns>Popis svih uređaja sortiran po nazivu</returns>
        [HttpGet]
        [ActionName("GetAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<UredajApiModel>), Description = "Vraća enumeraciju uređaja")]
        public async Task<IEnumerable<UredajApiModel>> UredajiGet()
        {
            var list = await _context.Uredaj
                                .Include(u => u.IdNadredeneKomponenteNavigation)
                                .Include(u => u.IdStatusaNavigation)
                                .Include(u => u.IdZidaNavigation)
                                .Select(d => new UredajApiModel
                                {
                                    Id = d.Id,
                                    Naziv = d.Naziv,
                                    NabavnaCijena = d.NabavnaCijena,
                                    AktualnaCijena = d.AktualnaCijena,
                                    DatumNabavke = d.DatumNabavke.ToString("dd.MM.yyyy"),
                                    IdNadredeneKomponente = d.IdNadredeneKomponente,
                                    NadredenaKomponenta = d.IdNadredeneKomponenteNavigation.Naziv == null ? "/": d.IdNadredeneKomponenteNavigation.Naziv,
                                    IdZida = d.IdZida,
                                    Zid = d.IdZidaNavigation.Naziv == null ? "/" : d.IdZidaNavigation.Naziv,
                                    IdStatusa = d.IdStatusa,
                                    Status = d.IdStatusaNavigation.Naziv == null ? "/" : d.IdStatusaNavigation.Naziv,
                                    //Zamjenski = _context.ZamjenskiUredaj.Include(u => u.IdUredajaNavigation).Where(u => u.IdZamjenaZa == d.Id).Select(u => new MasterDetailHelper { Id = u.IdUredajaNavigation.Id, Name = u.IdUredajaNavigation.Naziv, IdVeze = u.Id }).ToList(),
                                    //ZamjenaZa = _context.ZamjenskiUredaj.Include(u => u.IdZamjenaZaNavigation).Where(u => u.IdUredaja == d.Id).Select(u => new MasterDetailHelper { Id = u.IdZamjenaZaNavigation.Id, Name = u.IdZamjenaZaNavigation.Naziv, IdVeze = u.Id }).ToList(),
                                    //PodredeneKomponente = _context.Uredaj.Where(u => u.IdNadredeneKomponente == d.Id).Select(u => new MasterDetailHelper { Id = u.Id, Name = u.Naziv, IdVeze = 0 }).ToList(),
                                    //Servisi = _context.Servisira.Include(s => s.IdServisNavigation).Where(u => u.IdUredaj == d.Id).Select(s => new MasterDetailHelper { Id = s.IdServisNavigation.Id, IdVeze = s.Id, Name = s.IdServisNavigation.Ime }).ToList(),
                                })
                                .ToListAsync();
            list.Sort((a, b) => a.Naziv.CompareTo(b.Naziv));
            return list;
        }

        // GET api/Uredaj/ID
        /// <summary>
        /// Postupak za dohvat uređaja čiji je ID jednak poslanom parametru.
        /// </summary>
        /// <param name="idUredaja">ID uređaja</param>
        /// <returns>objekt tipa UredajApiModel ili NotFound ako uređaj s traženim ID-em ne postoji</returns>
        [HttpGet("{IdUredaja}", Name = "DohvatiUredaj")]
        [ActionName("Get")]
        [ProducesResponseType(typeof(UredajApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UredajGet(int idUredaja)
        {
            var uredaj = await _context.Uredaj
                                        .AsNoTracking()
                                        .Include(u => u.IdNadredeneKomponenteNavigation)
                                        .Include(u => u.IdStatusaNavigation)
                                        .Include(u => u.IdZidaNavigation)
                                        .Where(u => u.Id == idUredaja)
                                        .FirstOrDefaultAsync();

            if (uredaj == null)
            {
                _logger.LogWarning("Uređaj s ID-em " + idUredaja + " ne postoji.");
                return NotFound("Uređaj s navedenim ID-em ne postoji.");
            }
            else
            {
                return new ObjectResult(new UredajApiModel
                {
                    Id = uredaj.Id,
                    Naziv = uredaj.Naziv,
                    NabavnaCijena = uredaj.NabavnaCijena,
                    AktualnaCijena = uredaj.AktualnaCijena,
                    DatumNabavke = uredaj.DatumNabavke.ToString("dd.MM.yyyy"),
                    IdNadredeneKomponente = uredaj.IdNadredeneKomponente,
                    NadredenaKomponenta = uredaj.IdNadredeneKomponente == null ? null : uredaj.IdNadredeneKomponenteNavigation.Naziv,
                    IdZida = uredaj.IdZida,
                    Zid = uredaj.IdZida == null ? null : uredaj.IdZidaNavigation.Naziv,
                    IdStatusa = uredaj.IdStatusa,
                    Status = uredaj.IdStatusaNavigation.Naziv,
                    Zamjenski = _context.ZamjenskiUredaj.Include(u => u.IdUredajaNavigation).Where(u => u.IdZamjenaZa == uredaj.Id).Select(u => new MasterDetailHelper { Id = u.IdUredajaNavigation.Id, Name = u.IdUredajaNavigation.Naziv, IdVeze = u.Id }).ToList(),
                    ZamjenaZa = _context.ZamjenskiUredaj.Include(u => u.IdZamjenaZaNavigation).Where(u => u.IdUredaja == uredaj.Id).Select(u => new MasterDetailHelper { Id = u.IdZamjenaZaNavigation.Id, Name = u.IdZamjenaZaNavigation.Naziv, IdVeze = u.Id }).ToList(),
                    PodredeneKomponente = _context.Uredaj.Where(u => u.IdNadredeneKomponente == uredaj.Id).Select(u => new MasterDetailHelper { Id = u.Id, Name = u.Naziv, IdVeze = 0 }).ToList(),
                    Servisi = _context.Servisira.Include(s => s.IdServisNavigation).Where(u => u.IdUredaj == uredaj.Id).Select(s => new MasterDetailHelper { Id = s.IdServisNavigation.Id, IdVeze = s.Id, Name = s.IdServisNavigation.Ime }).ToList(),
                });
            }
        }

        // GET api/Uredaj/ID
        /// <summary>
        /// Postupak za dohvat svih servisa.
        /// </summary>
        /// <returns>lista servisa</returns>
        [HttpGet]
        [ActionName("GetServisi")]
        [ProducesResponseType(typeof(IEnumerable<Servis>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ServisiGet()
        {
            var servisi = await _context.Servis
                                        .AsNoTracking()
                                        .Select(u => new { Id = u.Id, Name = u.Ime })
                                        .ToListAsync();

            return new ObjectResult(servisi);
        }

        // POST api/Uredaj
        /// <summary>
        /// Postupak kojim se dodaje novi servise uređaju.
        /// </summary>
        /// <param name="model">Podaci o novom servisu</param>
        /// <returns>201 ako je uređaj uspješno pohranjen, te se ujedno vraća i spremljeni objekt, BadRequest u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("AddServis")]
        [ProducesResponseType(typeof(MasterDetailHelper), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ServisAdd([FromBody] Servisira model)
        {
            if (model != null && ModelState.IsValid)
            {
                if (await _context.Uredaj.FindAsync(model.IdUredaj) == null)
                {
                    _logger.LogWarning("Uređaj s ID-em " + model.IdUredaj + " ne postoji.");
                    return NotFound("Uređaj s navedenim ID-em (" + model.IdUredaj  + ") ne postoji.");
                }

                else if (await _context.Servis.FindAsync(model.IdServis) == null)
                {
                    _logger.LogWarning("Servis s navedenim ID-em (" + model.IdServis + ") ne postoji.");
                    return NotFound("Servis s navedenim ID-em (" + model.IdServis + ") ne postoji.");
                }

                else if(_context.Servisira.Where(s => s.IdServis == model.IdServis && s.IdUredaj == model.IdUredaj).ToList().Count() > 0)
                {
                    _logger.LogWarning("Servis " + model.IdServis + " već servisira uređaj " + model.IdUredaj);
                    return BadRequest("Servis " + model.IdServis + " već servisira uređaj " + model.IdUredaj);
                }

                else
                {
                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    var servis = _context.Servisira.Where(s => s.IdUredaj == model.IdUredaj && s.IdServis == model.IdServis).Include(s=> s.IdServisNavigation).First();

                    return new ObjectResult(new {
                        IdVeze = servis.Id,
                        Name = servis.IdServisNavigation.Ime,
                        Id = servis.IdServis,
                    });
                }
            }

            return BadRequest(ModelState);
        }

        // POST api/Uredaj
        /// <summary>
        /// Postupak kojim se stvara novi uređaj.
        /// </summary>
        /// <param name="model">Podaci o novom uređaju</param>
        /// <returns>201 ako je uređaj uspješno pohranjen, te se ujedno vraća i spremljeni objekt, BadRequest u slučaju neispravnog modela</returns>
        [HttpPost]
        [ActionName("New")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UredajCreate([FromBody] UredajApiModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                Uredaj uredaj = new Uredaj
                {
                    Naziv = model.Naziv,
                    DatumNabavke = (model.DatumNabavke == null ? System.DateTime.Now : DateTime.Parse(model.DatumNabavke)),
                    NabavnaCijena = model.NabavnaCijena,
                    AktualnaCijena = model.AktualnaCijena,
                    IdNadredeneKomponente = model.IdNadredeneKomponente,
                    IdZida = model.IdZida,
                    IdStatusa = model.IdStatusa
                };

                _context.Add(uredaj);
                await _context.SaveChangesAsync();

                var u = await _context.Uredaj
                                        .AsNoTracking()
                                        .Include(ur => ur.IdNadredeneKomponenteNavigation)
                                        .Include(ur => ur.IdStatusaNavigation)
                                        .Include(ur => ur.IdZidaNavigation)
                                        .LastOrDefaultAsync();

                _logger.LogInformation("Stvoren novi uređaj, ID = " + u.Id);
                return new ObjectResult(new UredajApiModel {
                    Id = u.Id,
                    Naziv = u.Naziv,
                    NabavnaCijena = u.NabavnaCijena,
                    AktualnaCijena = u.AktualnaCijena,
                    DatumNabavke = u.DatumNabavke.ToString("dd.MM.yyyy"),
                    IdNadredeneKomponente = u.IdNadredeneKomponente,
                    NadredenaKomponenta = u.IdNadredeneKomponente == null ? "/" : u.IdNadredeneKomponenteNavigation.Naziv,
                    IdZida = u.IdZida,
                    Zid = u.IdZida == null ? "/" : u.IdZidaNavigation.Naziv,
                    IdStatusa = u.IdStatusa,
                    Status = u.IdStatusaNavigation.Naziv,
                });
            }

            _logger.LogWarning(ModelState.ToString());
            return BadRequest(ModelState);
        }
        /*
        // POST api/Uredaj
        /// <summary>
        /// Postupak kojim se dodaje novi zamjenski uređaj postojećem uređaju.
        /// </summary>
        /// <param name="zamjenski">Objekt tipa ZamjenskiUređaj koji definira ID-eve uređaja između kojih se stvara veza zamjenskog uređaja</param>
        /// <returns>201 ako je uređaj uspješno pohranjen, NotFound ako uređaj ili zamjenski uređaj s traženim ID-em ne postoji</returns>
        [HttpPost]
        [ActionName("AddZamjenski")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddZamjenski([FromBody] ZamjenskiUredaj zamjenski)
        {

            if(await _context.Uredaj.FindAsync(zamjenski.IdZamjenaZa) == null)
            {
                return NotFound("Uređaj s navedenim ID-em ("+ zamjenski.IdZamjenaZa + ") ne postoji.");
            }

            else if (await _context.Uredaj.FindAsync(zamjenski.IdUredaja) == null)
            {
                return NotFound("Uređaj s navedenim ID-em (" + zamjenski.IdUredaja + ") ne postoji.");
            }

            else if (await _context.ZamjenskiUredaj.AsNoTracking().Where(u => u.IdUredaja == zamjenski.IdUredaja && u.IdZamjenaZa == zamjenski.IdZamjenaZa).FirstOrDefaultAsync() != null
                || await _context.ZamjenskiUredaj.AsNoTracking().Where(u => u.IdUredaja == zamjenski.IdZamjenaZa && u.IdZamjenaZa == zamjenski.IdUredaja).FirstOrDefaultAsync() != null)
            {
                return BadRequest(_context.Uredaj.Where(u => u.Id == zamjenski.IdUredaja).First().Naziv + " s ID-em (" + zamjenski.IdUredaja + ") je već definiran kao zamjenski za uređaj" + _context.Uredaj.Where(u => u.Id == zamjenski.IdZamjenaZa).First().Naziv + " (" + zamjenski.IdZamjenaZa + ")");
            }

            else
            {
                _context.Add(zamjenski);
                await _context.SaveChangesAsync();
                var za = _context.ZamjenskiUredaj.Where(z => z.IdUredaja == zamjenski.IdUredaja && z.IdZamjenaZa == zamjenski.IdZamjenaZa).Include(z => z.IdUredajaNavigation).Include(z => z.IdZamjenaZaNavigation).First();
                return new ObjectResult(new {
                                    IdVeze = za.Id,
                                    Id1 = za.IdUredaja,
                                    Id2 = za.IdZamjenaZa,
                                    N1 = za.IdUredajaNavigation.Naziv,
                                    N2 = za.IdZamjenaZaNavigation.Naziv,
                });
            }
        }
        */

        // PUT api/Uredaj
        /// <summary>
        /// Ažurira uređaj s ID-em uređaja temeljem parametera iz zahtjeva.
        /// </summary>
        /// <param name="model">model s podacima o uređaju</param>
        /// <returns>204 ako je uređaj uspješno ažuriran, NotFound ako uređaj s traženom šifrom ne postoji ili BadRequest ako model nije ispravan</returns>
        [HttpPut]
        [ActionName("Update")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UredajUpdate([FromBody] UredajApiModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning(ModelState.ToString());
                return BadRequest(ModelState);
            }
            else
            {
                var uredaj = await _context.Uredaj.FindAsync(model.Id);
                if (uredaj == null)
                {
                    _logger.LogWarning("Traženi uređaj ne postoji(ID = " + model.Id + ")");
                    return NotFound("Traženi uređaj ne postoji(ID = " + model.Id + ")");
                }
                else
                {
                    uredaj.Naziv = model.Naziv;
                    uredaj.DatumNabavke = (model.DatumNabavke == null ? System.DateTime.Now : DateTime.Parse(model.DatumNabavke));
                    uredaj.NabavnaCijena = model.NabavnaCijena;
                    uredaj.AktualnaCijena = model.AktualnaCijena;
                    uredaj.IdNadredeneKomponente = model.IdNadredeneKomponente;
                    uredaj.IdZida = model.IdZida;
                    uredaj.IdStatusa = model.IdStatusa;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Uređaj ažuriran, ID = " + uredaj.Id);
                    return NoContent();
                };
            }
        }

        // DELETE api/Uredaj/ID
        /// <summary>
        /// Briše uređaj s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="IdUredaja">oznaka uređaja kojeg treba obrisati</param>
        /// <returns>NotFound ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{IdUredaja}")]
        [ActionName("Remove")]
        public async Task<IActionResult> UredajDelete(int IdUredaja)
        {
            var uredaj = await _context.Uredaj.FindAsync(IdUredaja);
            if (uredaj == null)
            {
                _logger.LogWarning("Traženi uređaj ne postoji(ID = " + IdUredaja + ")");
                return NotFound("Traženi uređaj ne postoji(ID = " + IdUredaja + ")");
            }
            else
            {
                _context.Remove(uredaj);
                _context.RemoveRange(_context.ZamjenskiUredaj.Where(z => z.IdUredaja == uredaj.Id));
                _context.RemoveRange(_context.EkranZida.Where(z => z.IdUredaja == uredaj.Id));
                _context.RemoveRange(_context.Servisira.Where(s => s.IdUredaj == uredaj.Id));

                await _context.Uredaj.Where(u => u.IdNadredeneKomponente == uredaj.Id).ForEachAsync(u => u.IdNadredeneKomponente = null);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Uređaj obrisan");
                return NoContent();
            };
        }

        // PUT api/Uredaj/
        /// <summary>
        /// Briše zamjenski uređaj s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="model">veza koju treba obrisati</param>
        /// <returns>NotFound ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpPut]
        [ActionName("RemovePodredeni")]
        public async Task<IActionResult> PodredeniDelete([FromBody] MasterDetailHelper model)
        {
            var uredaj = await _context.Uredaj
                                       .AsNoTracking()
                                       .Where(u => u.Id == model.Id)
                                       .Include(u => u.IdNadredeneKomponenteNavigation)
                                       .FirstOrDefaultAsync();

            if (uredaj == null)
            {
                _logger.LogWarning("Ne postoji odnos uređaj-podređeni uređaj između dva navedena uređaja.");
                return NotFound("Ne postoji odnos uređaj-podređeni uređaj između dva navedena uređaja.");
            }

            if (uredaj.IdNadredeneKomponente != model.IdVeze)
            {
                _logger.LogWarning("Uredaj " + uredaj.IdNadredeneKomponenteNavigation.Naziv + "(" + uredaj.IdNadredeneKomponente + ") nije nadređen uređaju " + uredaj.Naziv + " (" + uredaj.Id + ").");
                return BadRequest("Uredaj " + uredaj.IdNadredeneKomponenteNavigation.Naziv + "("+ uredaj.IdNadredeneKomponente + ") nije nadređen uređaju " + uredaj.Naziv + " (" + uredaj.Id + ").");
            }

            uredaj.IdNadredeneKomponente = null;
            uredaj.IdNadredeneKomponenteNavigation = null;
            _context.Uredaj.Update(uredaj);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Obrisan podređeni uređaj");
            return NoContent();
        }

        // PUT api/Uredaj/
        /// <summary>
        /// Briše zamjenski uređaj s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="model">veza koju treba obrisati</param>
        /// <returns>NotFound ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpPut]
        [ActionName("AddPodredeni")]
        public async Task<IActionResult> PodredeniAdd([FromBody] MasterDetailHelper model)
        {
            var uredaj = await _context.Uredaj
                                       .AsNoTracking()
                                       .Where(u => u.Id == model.Id)
                                       .FirstOrDefaultAsync();

            if (uredaj == null)
            {
                _logger.LogWarning("Ne postoji uređaj.");
                return NotFound("Ne postoji uređaj.");
            }

            if (uredaj.IdNadredeneKomponente != null)
            {
                _logger.LogWarning("Uredaj " + uredaj.Naziv + " (" + uredaj.Id + ") već ima jedan nadređeni uređaj.");
                return BadRequest("Uredaj " + uredaj.Naziv + " (" + uredaj.Id + ") već ima jedan nadređeni uređaj.");
            }

            uredaj.IdNadredeneKomponente = model.IdVeze;
            _context.Uredaj.Update(uredaj);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Dodan podređeni uređaj ID = " + model.Id + " uređaju ID = " + model.IdVeze);
            return NoContent();
        }

        // DELETE api/Uredaj/id
        /// <summary>
        /// Briše servis uređaja s ID-em predanim u adresi zahtjeva.
        /// </summary>
        /// <param name="idVeze">oznaka veze koju treba obrisati</param>
        /// <returns>404 ili 204 ako je brisanje uspješno</returns>          
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("{idVeze}")]
        [ActionName("RemoveServis")]
        public async Task<IActionResult> ServisDelete(int idVeze)
        {
            var servis = await _context.Servisira
                                       .AsNoTracking()
                                       .Where(u => u.Id == idVeze)
                                       .FirstOrDefaultAsync();

            if (servis == null)
            {
                _logger.LogWarning("Ne postoji odnos uređaj-servis između navedenih uređaja i servisa.");
                return NotFound("Ne postoji odnos uređaj-servis između navedenih uređaja i servisa.");
            }

            _context.Remove(servis);
            _logger.LogInformation("Uklonjen servis");
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
