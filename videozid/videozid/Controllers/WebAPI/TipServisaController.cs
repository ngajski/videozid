using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace videozid.Controllers.WebAPI
{
    [Route("api/[controller]")]
    public class TipServisaController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Kontroler za upravljanje tipvoima servisa.
        /// </summary>
        /// <param name="context">Kontekst za pristup bazi podataka</param>
        public TipServisaController(RPPP15Context context)
        {
            _context = context;
        }

        // GET: TipServisas
        /// <summary>
        /// Metoda koja dohvaca sve tipove servisa i vrati pogled Index.
        /// </summary>
        /// <returns>Pogled Index sa pripadajućim modelom</returns>
        [HttpGet]
        public async Task<IActionResult> GetTipoviServisa()
        {
            var rPPP15Context = _context.TipServisa.Include(t => t.IdServisNavigation);
            return View("IndexAPI", await rPPP15Context.ToListAsync());
        }

        /// <summary>
        /// Metoda koja odabire tip servisa za detaljniji pregled na temelju ID-a.
        /// </summary>
        /// <param name="id">id tipa servisa</param>
        /// <returns>Pogled Details sa odgovarajućim modelom</returns>
        [HttpGet("details/{id}", Name ="DetailsTip")]
        public async Task<IActionResult> GetDetails(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var tipServisa = await _context.TipServisa
                .Include(t => t.IdServisNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tipServisa == null)
            {
                TempData[Constants.Error] = "Tip servisa sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            return View("DetailsAPI",tipServisa);
        }

        /// <summary>
        /// Metoda koja odabire tip servisa za uredivanje na temelju ID-a.
        /// </summary>
        /// <param name="id">id tipa servisa</param>
        /// <returns>Pogled Edit sa odgovarajućim modelom</returns>>
        /// <returns></returns>
        [HttpGet("edit/{id}", Name = "EditTip")]
        public async Task<IActionResult> GetEdit(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var tipServisa = await _context.TipServisa.SingleOrDefaultAsync(m => m.Id == id);
            if (tipServisa == null)
            {
                TempData[Constants.Error] = "Tip servisa sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", tipServisa.IdServis);
            return View("EditAPI",tipServisa);
        }


        /// <summary>
        /// Metoda koja sprema promjete u tipu servisa zadanom određenom s ID-om.
        /// </summary>
        /// <param name="id">id tipa</param>
        /// <param name="tipServisa">ime tipa</param>
        /// <returns>Pogled Post</returns>
        [HttpPost("edit/{id}", Name ="EditTipPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(int id, [FromBody] TipServisa tipServisa)
        {
            if (id != tipServisa.Id)
            {
                TempData[Constants.Error] = "ID-evi se ne poklapaju: " + id + "!=" + tipServisa.Id;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipServisa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipServisaExists(tipServisa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Success"] = "Tip servisa \"" + tipServisa.Tip + "\" uspiješno promjenjen.";

                return RedirectToAction("GetTipoviServisa");
            }

            ViewData["IdServis"] = new SelectList(_context.Servisira, "Id", "Id", tipServisa.IdServis);
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", tipServisa.IdServis);

            TempData["Error"] = "Tip servisa \"" + tipServisa.Tip + "\" nije promjenjen.";

            return View(tipServisa);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"> id tipa servisa</param>
        /// <returns>True ako tip servisa postoji u bazi podataka</returns>
        private bool TipServisaExists(int id)
        {
            return _context.TipServisa.Any(e => e.Id == id);
        }
    }
}