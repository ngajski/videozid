using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Kontroler za upravljanje serviserima.
    /// </summary>
    [Route("api/[controller]")]
    public class ServiserController : Controller
    {
        private readonly RPPP15Context _context;
        private ILogger logger;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontekst za pristup bazi podataka</param>
        /// <param name="logger">Logger sa spremanje traga rada aplikacije u bazu podataka</param>
        public ServiserController(RPPP15Context context, ILogger<ServiserController> logger)
        {
            _context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Postupak za dohvat svih servisera. 
        /// </summary>
        /// <returns>Popis svih servisera</returns>
        [HttpGet]
        //[ActionName("GetSAll")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServiserApiModel>), Description = "Dohvat svih servisera")]
        public async Task<IEnumerable<ServiserApiModel>> GetServiseri()
        {
            logger.LogInformation("Dohvat svih servisera");
            var list = await _context.Serviser
                                .Select(d => new ServiserApiModel
                                {
                                    Id = d.Id,
                                    IdServis = d.IdServis,
                                    Ime = d.Ime,
                                    Prezime = d.Prezime,
                                    Opis = d.Opis,
                                    Servis = _context.Servis.Where(s => s.Id == d.IdServis).First().Ime
                                })
                                .ToListAsync();
            return list;

        }

        /// <summary>
        /// Metoda koja briše servisera zadanog s ID-om
        /// </summary>
        /// <param name="id">id servisera</param>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpDelete("delete/{id}")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServiserApiModel>), Description = "Brisanje servisera zadanog s ID-em")]
        public void DeleteServiser(int? id)
        {
            if (id == null)
            {
                logger.LogWarning("ID je null");
                return;
            }

            Serviser serviser = _context.Serviser.Find(id);

            if (serviser != null)
            {
                _context.Serviser.Remove(serviser);
                _context.SaveChanges();
                logger.LogInformation("Serviser uspiješno obrisan");
            } else
            {
                logger.LogWarning("Serviser s id=" + id + " ne postoji");
            }
        }

        /// <summary>
        /// Postupak kojim se stvara novi servis
        /// </summary>
        /// <param name="model">Model servisa</param>
        /// <returns>201 ako je servis uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(ServiserApiModel), Description = "Stvaranje novog servisera")]
        public IActionResult CreateServiser([FromBody] ServiserApiModel model)
        {
            if (model != null)
            {

                var servis = _context.Servis.Find(model.IdServis);
                if (servis == null)
                {
                    return BadRequest("Servis s ID=" + model.IdServis + " ne postoji.");
                }

                Serviser serviser = new Serviser
                {
                    Ime = model.Ime,
                    Prezime = model.Prezime,
                    Opis = model.Opis,
                    IdServis = model.IdServis
                };

                try
                {
                    _context.Serviser.Add(serviser);
                    _context.SaveChanges();

                    logger.LogInformation("Serviser " + model.Ime + " " + model.Prezime + " uspješno dodan.");
                    return new ObjectResult(serviser);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                logger.LogError("Servis nije odabran.");
                return BadRequest("Servis nije odabran.");
            }
        }

        /// <summary>
        /// Postupak kojim se mijenja postojeći serviser
        /// </summary>
        /// <param name="model">Model servisaera (ServiserApiModel)</param>
        /// <param name="idServiser">ID servisera koji se mijenja</param>
        /// <returns>201 ako je serviser uspješno izmjenjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPut("{idServiser}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(ServiserApiModel), Description = "Stvaranje novog servisera")]
        public IActionResult UpdateServiser([FromBody] ServiserApiModel model, int idServiser)
        {
            if (model != null && ModelState.IsValid)
            {
                var servis = _context.Servis.Find(model.IdServis);
                if (servis == null)
                {
                    logger.LogError("Servis s ID=" + model.IdServis + " ne postoji");
                    return BadRequest("Servis s ID=" + model.IdServis + " ne postoji");
                }

                var serviser = _context.Serviser.Find(idServiser);
                if (serviser == null)
                {
                    logger.LogError("Serviser s ID=" + idServiser + " ne postoji");
                    return BadRequest("Serviser s ID=" + idServiser + " ne postoji");
                }

                serviser.Ime = model.Ime;
                serviser.Prezime = model.Prezime;
                serviser.Opis = model.Opis;
                serviser.IdServis = servis.Id;

                try
                {
                    _context.Update(serviser);
                    _context.SaveChanges();
                    logger.LogInformation("Serviser " + serviser.Ime + " " + serviser.Prezime + " uspješno izmjenjen");
                    return new ObjectResult(model);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                logger.LogError("Serviser nije spremljen.");
                return BadRequest("Serviser nije spremljen.");
            }
        }


    }

    /// <summary>
    /// Model koji predstavlja Servisera za korištenje s APIjem 
    /// </summary>
    public class ServiserApiModel
    {
        /// <summary>
        /// Id servisera
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id servisa za koji serviser radi
        /// </summary>
        public int IdServis { get; set; }
        /// <summary>
        /// Ime servisera
        /// </summary>
        public string Ime { get; set; }
        /// <summary>
        /// Prezime servisera
        /// </summary>
        public string Prezime { get; set; }
        /// <summary>
        /// Opis servisera
        /// </summary>
        public string Opis { get; set; }
        /// <summary>
        /// Imde servisa za koji serviser radi
        /// </summary>
        public string Servis { get; set; }
    }
}