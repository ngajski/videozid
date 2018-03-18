using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Logging;

namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Kontroler za upravljanje servisira.
    /// </summary>
    [Route("api/[controller]")]
    public class ServisiraController : Controller
    {
        private readonly RPPP15Context context;
        private ILogger logger;

        /// <summary>
        /// Kontroler za upravljanje servisira.
        /// </summary>
        /// <param name="context">Kontekst za pristup bazi podataka</param>
        /// <param name="logger">NLogger koji zapisuje trag u bazu podataka</param>
        public ServisiraController(RPPP15Context context, ILogger<ServisController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Metoda koja dohvaca sve servisira i vrati pogled Index.
        /// </summary>
        /// <returns>Pogled Index sa pripadajućim modelom</returns>
        [HttpGet]
        public async Task<IActionResult> GetIndex()
        {
            var rPPP15Context = context.Servisira.Include(s => s.IdServisNavigation).Include(s => s.IdUredajNavigation);
            return View("IndexAPI",await rPPP15Context.ToListAsync());
        }

        /// <summary>
        /// Metoda koja odabire tip servisa za detaljniji pregled na temelju ID-a.
        /// </summary>
        /// <param name="id">id tipa servisa</param>
        /// <returns>Pogled Details sa odgovarajućim modelom</returns>
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetails(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("IndexAPI");
            }

            var servisira = await context.Servisira
                .Include(s => s.IdServisNavigation)
                .Include(s => s.IdUredajNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (servisira == null)
            {
                TempData[Constants.Error] = "Servis sa id=" + id + " ne postoji.";
                return RedirectToAction("IndexAPI");
            }

            return View("DetailsAPI",servisira);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> GetEdit(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("IndexAPI");
            }

            var servisira = await context.Servisira.SingleOrDefaultAsync(m => m.Id == id);
            if (servisira == null)
            {
                TempData[Constants.Error] = "Servisira sa id=" + id + " ne postoji.";
                return RedirectToAction("IndexAPI");
            }
            ViewData["IdServis"] = new SelectList(context.Servis, "Id", "Ime", servisira.IdServis);
            ViewData["IdUredaj"] = new SelectList(context.Uredaj, "Id", "Naziv", servisira.IdUredaj);
            return View("EditAPI",servisira);
        }

        // POST: Servisiras/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdServis,IdUredaj")] Servisira servisira)
        {
            if (id != servisira.Id)
            {
                TempData[Constants.Error] = "ID-evi se ne poklapaju: " + id + "!=" + servisira.Id;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(servisira);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServisiraExists(servisira.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Success"] = "Poveznica uspješno ažurirana.";

                return RedirectToAction("Index");
            }
            ViewData["IdServis"] = new SelectList(context.Servis, "Id", "Ime", servisira.IdServis);
            ViewData["IdUredaj"] = new SelectList(context.Uredaj, "Id", "Naziv", servisira.IdUredaj);
            return View(servisira);
        }

        private bool ServisiraExists(int id)
        {
            return context.Servisira.Any(e => e.Id == id);
        }

        /// <summary>
        /// Procedura koja briše servisira zadanog s ID-om
        /// </summary>
        /// <param name="id">id servisira</param>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServisApiModel>), Description = "Brisanje servisa uspješno")]
        public void DeleteServisira(int? id)
        {
            if (id == null) return;

            var servisira = context.Servisira.Where(s => s.Id == id).FirstOrDefault();
            if (servisira != null)
            {
                context.Servisira.Remove(servisira);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Procedura koja briše servisira. Servisira je jednoznačno određen sa ID-em uređaja i ID-em servisa.
        /// </summary>
        /// <param name="idServis"></param>
        /// <param name="idUredaj"></param>
        [HttpDelete("delete/{idUredaj}/{idServis}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IEnumerable<ServisApiModel>), Description = "Brisanje servisira za servis i uređaj uspješno")]
        public IActionResult DeleteForServisAndUredaj(int idServis, int idUredaj)
        {
            var servisira = context.Servisira.Where(s => s.IdServis == idServis && s.IdUredaj == idUredaj).FirstOrDefault();
            if (servisira != null)
            {
                try
                {
                    context.Servisira.Remove(servisira);
                    context.SaveChanges();
                    logger.LogInformation("Servisira uspješno izbrisan.");
                    return new ObjectResult(servisira);
                } catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    return BadRequest(ex.Message);
                }  
            }

            return BadRequest("Nije pronađen željeni servis ili uređaj!");
        }

    }
}
