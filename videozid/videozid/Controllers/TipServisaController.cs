using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using videozid.Models;

namespace videozid.Controllers
{
    public class TipServisaController : Controller
    {
        private readonly RPPP15Context _context;

        public TipServisaController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: TipServisas
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.TipServisa.Include(t => t.IdServisNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: TipServisas/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(tipServisa);
        }

        // GET: TipServisas/Create
        public IActionResult Create()
        {
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime");
            return View();
        }

        // POST: TipServisas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdServis,Tip")] TipServisa tipServisa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipServisa);

                TempData[Constants.Success] = "Tip servisa \"" + tipServisa.Tip + "\" uspješno dodan.";

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            TempData[Constants.Error] = $"Tip servisa \"" + tipServisa.Tip + "\" nije dodan.";
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", tipServisa.IdServis);

            return View(tipServisa);
        }

        // GET: TipServisas/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(tipServisa);
        }

        // POST: TipServisas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdServis,Tip")] TipServisa tipServisa)
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

                return RedirectToAction("Index");
            }

            ViewData["IdServis"] = new SelectList(_context.Servisira, "Id", "Id", tipServisa.IdServis);
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", tipServisa.IdServis);

            TempData["Error"] = "Tip servisa \"" + tipServisa.Tip + "\" nije promjenjen.";

            return View(tipServisa);
        }

        // GET: TipServisa/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(tipServisa);
        }

        // POST: TipServisas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var tipServisa = await _context.TipServisa.SingleOrDefaultAsync(m => m.Id == id);

            if (tipServisa == null)
            {
                TempData[Constants.Error] = "Tip servisa sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }


            _context.TipServisa.Remove(tipServisa);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tip servisa \"" + tipServisa.Tip + "\" uspiješno obrisan.";

            return RedirectToAction("Index");
        }

        public void DeleteAsync(int? id)
        {
            TipServisa tip = _context.TipServisa.Find(id);

            if (tip != null)
            {
                _context.TipServisa.Remove(tip);
                _context.SaveChanges();
                TempData["Success"] = "Tip servisa \"" + tip.Tip + "\" uspiješno obrisan.";
            }
        }

        private bool TipServisaExists(int id)
        {
            return _context.TipServisa.Any(e => e.Id == id);
        }
    }
}
