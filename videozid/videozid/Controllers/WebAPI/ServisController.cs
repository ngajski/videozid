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
using Org.BouncyCastle.Utilities.Collections;

namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Kontroler za upravljanje servisima
    /// </summary>
    [Route("api/[controller]")]
    public class ServisController : Controller
    {

        private readonly RPPP15Context context;
        private ILogger logger;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontekst za pristup bazi podataka</param>
        /// <param name="logger">Loger koji zapisuje u bazu podataka</param>
        public ServisController(RPPP15Context context, ILogger<ServisController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Postupak za dohvat svih servisa. 
        /// </summary>
        /// <returns>Popis svih servisa</returns>
        [HttpGet]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServisApiModel>), Description = "Dohvat svih servisa")]
        public async Task<IEnumerable<ServisApiModel>> GetServisi()
        {
            logger.LogInformation("Dohvat svih servisera");
            var list = await context.Servis
                                .Select(d => new ServisApiModel
                                {
                                    Id = d.Id,
                                    Ime = d.Ime,
                                    Opis = d.Opis,
                                    Racun = d.ZiroRacun,
                                    TipServisa = context.TipServisa.Where(s => s.IdServis == d.Id).First().Tip,
                                    Serviseri = context.Serviser.Where(s => s.IdServis == d.Id).ToList()
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
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServisApiModel>), Description = "Brisanje servisa zadanog s ID-em")]
        public void DeleteServis(int? id)
        {
            if (id == null)
            {
                logger.LogWarning("Id je null");
                return;
            }

            var servisiraServis = context.Servisira.Where(s => s.IdServis == id).ToList();
            foreach (var servisira in servisiraServis)
            {
                context.Servisira.Remove(servisira);
            }

            Servis servis = context.Servis.Find(id);

            if (servis != null)
            {
                context.Servis.Remove(servis);
                context.SaveChanges();
                logger.LogInformation("Servis " + servis.Ime + "uspiješno obrisan");
            } else
            {
                logger.LogWarning("Servis s id=" + id + " ne postoji");
            }
        }

        /// <summary>
        /// Procedura koja dohvača servis kojemu je ID jednak predanom parametru id.
        /// </summary>
        /// <param name="id">ID koji jednoznačno određuje servis</param>
        /// <returns>Servis ukoliko je pronađen, inače Error 404</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServisApiModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServisApiModel>), Description = "Dohvat servisa zadanog s ID-em")]
        public async Task<IActionResult> GetServis(int id)
        {
            var servis = await context.Servis
                              .Select(d => new ServisApiModel
                              {
                                  Id = d.Id,
                                  Ime = d.Ime,
                                  Opis = d.Opis,
                                  Racun = d.ZiroRacun,
                                  TipServisa = context.TipServisa.Where(t => t.IdServis == id).First().Tip,
                                  Serviseri = context.Serviser.Where(s => s.IdServis == id).ToList()
                              }).Where(s => s.Id == id).ToListAsync();
            if (servis.Count != 1)
            {
                logger.LogWarning("Servis s id=" + id + " ne postoji");
                return NotFound("Servis ne postoji! ID: " + id);
            }

            var uredajIdNaziv = (from ur in context.Uredaj
                                 join servisira in context.Servisira on ur.Id equals servisira.IdUredaj
                                 where (servisira.IdServis == id)
                                 select new
                                 {
                                     id = ur.Id,
                                     naziv = ur.Naziv
                                 }).ToList();

            List<Uredaj> uredaji = new List<Uredaj>();
            for (int i = 0; i < uredajIdNaziv.Count; i++)
            {
                Uredaj uredaj = new Uredaj();
                uredaj.Id = uredajIdNaziv[i].id;
                uredaj.Naziv = uredajIdNaziv[i].naziv;
                uredaji.Add(uredaj);
            }

            if (servis[0].Serviseri.Count == 0) servis[0].Serviseri = null;

            servis[0].Uredaji = uredaji;

            logger.LogInformation("Servis " + servis[0].Ime + " dohvaćen");
            return new ObjectResult(servis[0]);
        }

        /// <summary>
        /// Postupak kojim se stvara novi servis
        /// </summary>
        /// <param name="model">Model servisa</param>
        /// <returns>201 ako je servis uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServisApiModel>), Description = "Stvaranje novog servisa")]
        public IActionResult CreateServis([FromBody] ServisApiModel model)
        {
            if (model != null)
            {
                if (IBANExists(model.Racun))
                {
                    return BadRequest("IBAN postoji");
                }

                Servis servis = new Servis
                {
                    Ime = model.Ime,
                    Opis = model.Opis,
                    ZiroRacun = model.Racun,
                    TipServisa = new List<TipServisa>()
                };

                servis.TipServisa.Add(new TipServisa
                {
                    Tip = model.TipServisa
                });

                try
                {
                    servis.Serviser = new List<Serviser>();
                    context.Add(servis);
                    context.SaveChanges();

                    logger.LogInformation("Servis " + model.Ime + " uspješno dodan.");
                    return new ObjectResult(servis);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    return BadRequest(ex.Message);
                }
                

                
            }
            else
            {
                logger.LogError("Servis nije spremljen.");
                return BadRequest("Servis nije spremljen.");
            }
        }

        /// <summary>
        /// Postupak kojim se mijenja postojeći servis
        /// </summary>
        /// <param name="model">Model servisa (ServisApiModel)</param>
        /// <param name="idServis">ID servisa koji se mijenja</param>
        /// <returns>201 ako je servis uspješno pohranjen, te se ujedno vraća i spremljeni objekt, 400 u slučaju neispravnog modela</returns>
        [HttpPut("{idServis}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(ServisApiModel), Description = "Stvaranje novog servisa")]
        public IActionResult UpdateServis([FromBody] ServisApiModel model, int idServis)
        {
            if (model != null && ModelState.IsValid)
            {

                var servis = context.Servis.Find(idServis);
                if (servis == null)
                {
                    logger.LogError("Servis s ID=" + idServis + " ne postoji");
                    return BadRequest("Servis s ID=" + idServis + " ne postoji");
                }

                if (IBANExists(model.Racun) && !servis.ZiroRacun.Equals(model.Racun))
                {
                    return BadRequest("IBAN postoji");
                }

                

                servis.Ime = model.Ime;
                servis.Opis = model.Opis;
                servis.ZiroRacun = model.Racun;

                servis.TipServisa = new List<TipServisa>();
                var tip = new TipServisa
                {
                    Tip = model.TipServisa
                };
                servis.TipServisa.Add(tip);

                try
                {
                    context.Update(servis);
                    context.SaveChanges();
                    logger.LogInformation("Servis " + servis.Ime + " uspješno izmjenjen");
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
                logger.LogError("Servis nije spremljen.");
                return BadRequest("Servis nije spremljen.");
            }
        }

        /// <summary>
        /// Procedura koja dodaje servisera zadanog ID-em servisu zadanom ID-em.
        /// </summary>
        /// <param name="idServis">id servisa</param>
        /// <param name="idServiser">id servisera</param>
        /// <returns>status 200 i dodanog servisera ukoliko je sve u redu, inače error 400</returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("addserviser/{idServis}/{idServiser}")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<Serviser>), Description = "Dodavanje servisera zadanog s ID-em servisu")]
        public IActionResult AddServiser(int idServis, int idServiser)
        {

            Serviser serviser = context.Serviser.Find(idServiser);
            Servis servis = context.Servis.Find(idServis);

            if (serviser == null || servis == null)
            {
                logger.LogError("Traženi serviser ili servis ne postoji");
                return BadRequest("Traženi serviser ili servis ne postoji");
            }

            try
            {
                var noviServiser = new Serviser
                {
                    Ime = serviser.Ime,
                    Prezime = serviser.Prezime,
                    Opis = serviser.Opis,
                    IdServis = idServis
                };

                context.Serviser.Add(noviServiser);
                context.SaveChanges();

                context.Serviser.Remove(serviser);
                context.SaveChanges();

                logger.LogInformation("Serviser " + serviser.Ime + " uspjepno dodan servisu");
                return new ObjectResult(serviser);
            } catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }            
        }

        /// <summary>
        /// Procedura koja vraća listu svih uređaja koje servis zadan s ID-em NE servisira
        /// </summary>
        /// <param name="idServis">id servisa</param>
        /// <returns>listu svih uređaja koje servis zadan s ID-em NE servisira</returns>
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [HttpGet("uredaji/{idServis}")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<UredajApiModelHelper>), Description = "Dohvat svih servisera")]
        public IEnumerable<UredajApiModelHelper> GetUredaji(int idServis)
        {
            logger.LogInformation("Dohvat svih uređaja.");
            var list = getOtherUredajiForServis(idServis);
            return list;

        }

        /// <summary>
        /// Procedura koja dodaje postojeć uređaj zadan ID-em servisu zadanom ID-em.
        /// </summary>
        /// <param name="idServis">id servisa</param>
        /// <param name="idUredaj">id uređaja</param>
        /// <returns>Dodani uređaj i status 202 u skučaju uspjeha, inače status 400</returns>
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("adduredaj/{idUredaj}/{idServis}")]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(Uredaj), Description = "Dodavanje uređaja zadanog s ID-em servisu")]
        public IActionResult AddUredaj(int idServis, int idUredaj)
        {
            Servis servis = context.Servis.Find(idServis);
            Uredaj uredaj = context.Uredaj.Find(idUredaj);

            if (servis == null || uredaj == null)
            {
                logger.LogWarning("Traženi servis ili uređaj nije pronađen");
                return BadRequest("Traženi servis ili uređaj nije pronađen");
            }

            var existis = context.Servisira.Where(s => s.IdServis == idServis && s.IdUredaj == idUredaj).FirstOrDefault();
            if (existis != null) return BadRequest("Servis " + servis.Ime + " već servisira " + uredaj.Naziv);

            try
            {
                var servisira = new Servisira
                {
                    IdServis = idServis,
                    IdUredaj = idUredaj
                };

                context.Servisira.Add(servisira);
                context.SaveChanges();
                logger.LogInformation("Uređaj " + uredaj.Naziv + " uspješno dodan servisu.");
                return new ObjectResult(uredaj);
            }
            catch (Exception ex)
            {
                logger.LogError("Uređaj " + uredaj.Naziv + " nije dodan servisu");
                return BadRequest("Uređaj " + uredaj.Naziv + " nije dodan servisu:" + ex.Message);
            }
        }

        private HashSet<UredajApiModelHelper> getOtherUredajiForServis(int id)
        {
            var allUredaji = context.Uredaj.ToList();
            var servisira = context.Servisira.Where(s => s.IdServis != id).ToList();

            HashSet<UredajApiModelHelper> uredaji = new HashSet<UredajApiModelHelper>();
            foreach (var uredaj in allUredaji)
            {
                var s = context.Servisira.Where(u => u.IdServis == id && u.IdUredaj == uredaj.Id).FirstOrDefault();
                if (s == null)
                {
                    var u = new UredajApiModelHelper
                    {
                        Id = uredaj.Id,
                        Ime = uredaj.Naziv
                    };
                    uredaji.Add(u);
                }
            }

            return uredaji;
        }

        /// <summary>
        /// Procedura koja provjerava je li iban u dobrom formatu.
        /// </summary>
        /// <param name="iban"></param>
        /// <returns></returns>
        private Boolean CheckIBAN(string iban)
        {
            if (iban.Length != 21)
            {
                return false;
            }
            else if (!iban.ToUpper().StartsWith("HR"))
            {
                return false;
            }
            return true;
        }

        private bool IBANExists(string ziroRacun)
        {
            var servis = context.Servis.Where(s => s.ZiroRacun == ziroRacun).FirstOrDefault();
            if (servis == null)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Klasa koja predstavlja model Servisa pogodan za API.
    /// </summary>
    public class ServisApiModel
    {
        /// <summary>
        /// Id servisa
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ime servisa
        /// </summary>
        public string Ime { get; set; }
        /// <summary>
        /// ZiroRacun servisa
        /// </summary>
        public string Racun { get; set; }
        /// <summary>
        /// Opis servisa
        /// </summary>
        public string Opis { get; set; }
        /// <summary>
        /// Lista servisara koji pripadaju servisu
        /// </summary>
        public virtual List<Serviser> Serviseri { get; set; }
        /// <summary>
        /// Tip servisa
        /// </summary>
        public virtual string TipServisa { get; set; }
        /// <summary>
        /// List uredaja koje servis servisira
        /// </summary>
        public virtual List<Uredaj> Uredaji { get; set; }
    }

    /// <summary>
    /// Model uređaja koji se koristi prilikom prijenosa paramateara u post zahtjevu.
    /// </summary>
    public class UredajApiModelHelper
    {
        /// <summary>
        /// Id uređaja.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ime uređaja
        /// </summary>
        public string Ime { get; set; }
    }
}