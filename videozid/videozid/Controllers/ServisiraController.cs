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
    public class ServisiraController : Controller
    {
        private readonly RPPP15Context _context;

        public ServisiraController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: Servisiras
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.Servisira.Include(s => s.IdServisNavigation).Include(s => s.IdUredajNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Servisiras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var servisira = await _context.Servisira
                .Include(s => s.IdServisNavigation)
                .Include(s => s.IdUredajNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (servisira == null)
            {
                TempData[Constants.Error] = "Servis sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            return View(servisira);
        }

        // GET: Servisiras/Create
        public IActionResult Create()
        {
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime");
            ViewData["IdUredaj"] = new SelectList(_context.Uredaj, "Id", "Naziv");
            return View();
        }

        // POST: Servisiras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdServis,IdUredaj")] Servisira servisira)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servisira);
                await _context.SaveChangesAsync();

                TempData[Constants.Success] = "Poveznica uspješno dodana.";

                return RedirectToAction("Index");
            }

            TempData[Constants.Error] = $"Servisira  id=" + servisira.Id + " nije dodan.";
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", servisira.IdServis);
            ViewData["IdUredaj"] = new SelectList(_context.Uredaj, "Id", "Naziv", servisira.IdUredaj);

            return View(servisira);
        }

        // GET: Servisiras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var servisira = await _context.Servisira.SingleOrDefaultAsync(m => m.Id == id);
            if (servisira == null)
            {
                TempData[Constants.Error] = "Servisira sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", servisira.IdServis);
            ViewData["IdUredaj"] = new SelectList(_context.Uredaj, "Id", "Naziv", servisira.IdUredaj);
            return View(servisira);
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
                    _context.Update(servisira);
                    await _context.SaveChangesAsync();
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
            ViewData["IdServis"] = new SelectList(_context.Servis, "Id", "Ime", servisira.IdServis);
            ViewData["IdUredaj"] = new SelectList(_context.Uredaj, "Id", "Naziv", servisira.IdUredaj);
            return View(servisira);
        }

        // GET: Servisiras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var servisira = await _context.Servisira
                .Include(s => s.IdServisNavigation)
                .Include(s => s.IdUredajNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (servisira == null)
            {
                TempData[Constants.Error] = "Servisira sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            return View(servisira);
        }

        // POST: Servisiras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servisira = await _context.Servisira.SingleOrDefaultAsync(m => m.Id == id);
            _context.Servisira.Remove(servisira);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Poveznica uspješno obrisana.";
            return RedirectToAction("Index");
        }

        private bool ServisiraExists(int id)
        {
            return _context.Servisira.Any(e => e.Id == id);
        }


        public void DeleteAsync(int? id)
        {
            Servisira servisira = _context.Servisira.Find(id);

            if (servisira != null)
            {
                _context.Servisira.Remove(servisira);
                _context.SaveChanges();
            }
        }

        public void DeleteForServisAndUredaj(int idServis, int idUredaj)
        {
            var servisira = _context.Servisira.Where(s => s.IdServis == idServis && s.IdUredaj == idUredaj).First();
            if (servisira != null)
            {
                _context.Servisira.Remove(servisira);
                _context.SaveChanges();
            }
        }

    }
}
