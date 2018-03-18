using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace videozid.Controllers
{
    public class ServiserController : Controller
    {
        private readonly RPPP15Context _context;
        private ILogger logger;

        public ServiserController(RPPP15Context context, ILogger<ServisController> logger)
        {
            _context = context;
            this.logger = logger;
        }

        // GET: Serviser
        public async Task<IActionResult> Index()
        {
            logger.LogTrace("Listanje index stranice");
            logger.LogWarning("Listanje servisera!");

            var rPPP15Context = _context.Serviser.Include(s => s.IdServisNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Serviser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var serviser = await _context.Serviser
                .Include(s => s.IdServisNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (serviser == null)
            {
                TempData[Constants.Error] = "Serviser sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            return View(serviser);
        }

        // GET: Serviser/Create
        public IActionResult Create()
        {
            logger.LogInformation("Serviser se stvara");
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime");
            return View();
        }

        // POST: Serviser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdServis,Ime,Prezime,Opis")] Serviser serviser)
        {
            logger.LogTrace(JsonConvert.SerializeObject(serviser));
            if (ModelState.IsValid)
            {
                _context.Add(serviser);

                TempData[Constants.Success] = "Serviser \"" + serviser.Prezime + "\" uspješno dodan.";
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            logger.LogError("Serviser nije dodan: " + JsonConvert.SerializeObject(serviser));

            TempData[Constants.Error] = "Model servisera nije valjan.";
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", serviser.IdServis);
            return View(serviser);
        }

        // GET: Serviser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //dokaz da je najnovija
            logger.LogInformation("Serviser se editira: " + id);
            if (id == null)
            {
                logger.LogWarning("Serviser ne postoji: " + id);
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var serviser = await _context.Serviser.SingleOrDefaultAsync(m => m.Id == id);
            if (serviser == null)

            {
                TempData[Constants.Error] = "Serviser sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", serviser.IdServis);
            return View(serviser);
        }

        // POST: Serviser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdServis,Ime,Prezime,Opis")] Serviser serviser)
        {
            if (id != serviser.Id)
            {
                logger.LogError("Greška kod editiranja: " + id);
                TempData[Constants.Error] = "ID-evi se ne poklapaju: " + id + "!=" + serviser.Id;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiserExists(serviser.Id))
                    {
                        logger.LogTrace("Serviser uspiješno editiran: " + id);
                        TempData[Constants.Error] = "Serviser sa ID=" + id + " ne postoji.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData[Constants.Success] = "Serviser \"" + serviser.Prezime + "\" uspješno ažuriran.";
                return RedirectToAction("Index");
            }

            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", serviser.IdServis);
            return View(serviser);
        }

        // GET: Serviser/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logger.LogError("Testni error kod brisanja.");
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var serviser = await _context.Serviser
                .Include(s => s.IdServisNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (serviser == null)
            {
                TempData[Constants.Error] = "Serviser sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            return View(serviser);
        }

        // POST: Serviser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logger.LogError("Testni error kod brisanja.");
            var serviser = await _context.Serviser.SingleOrDefaultAsync(m => m.Id == id);
            _context.Serviser.Remove(serviser);
            await _context.SaveChangesAsync();

            logger.LogWarning("Serviser obrisan:" + id);
            TempData[Constants.Success] = "Serviser \"" + serviser.Prezime + "\" uspješno obrisan.";
            return RedirectToAction("Index");
        }

        // GET: Uredaj
        public IActionResult IndexAPI()
        {
            return View("IndexAPI");
        }

        public IActionResult EditAPI(int id)
        {
            return View("EditAPI", id);
        }

        public IActionResult DetailsAPI(int id)
        {
            return View("DetailsAPI", id);
        }

        private bool ServiserExists(int id)
        {
            return _context.Serviser.Any(e => e.Id == id);
        }

        public void DeleteAsync(int? id)
        {
            logger.LogError("Testni error kod brisanja.");
            Serviser serviser = _context.Serviser.Find(id);

            if (serviser != null)
            {
                _context.Serviser.Remove(serviser);
                _context.SaveChanges();
            }
        }

    }
}
