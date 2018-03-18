using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using videozid;

namespace videozid.Controllers
{
    public class SadrzajController : Controller
    {
        private readonly RPPP15Context _context;

        public SadrzajController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: Sadrzaj
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.Sadrzaj.Include(s => s.IdAutoraNavigation).Include(s => s.IdOdobrenOdNavigation).Include(s => s.IdTipaNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Sadrzaj/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sadrzaj = await _context.Sadrzaj
                .Include(s => s.IdAutoraNavigation)
                .Include(s => s.IdOdobrenOdNavigation)
                .Include(s => s.IdTipaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sadrzaj == null)
            {
                return NotFound();
            }

            return View(sadrzaj);
        }

        // GET: Sadrzaj/Create
        public IActionResult Create()
        {
            ViewData["IdAutora"] = new SelectList(_context.Korisnik, "Id", "Email");
            ViewData["IdOdobrenOd"] = new SelectList(_context.Korisnik, "Id", "Email");
            ViewData["IdTipa"] = new SelectList(_context.TipSadrzaja, "Id", "Ime");
            return View();
        }

        // POST: Sadrzaj/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdAutora,IdOdobrenOd,IdTipa,Ime,Opis,Url,JeOdobren")] Sadrzaj sadrzaj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sadrzaj);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["IdAutora"] = new SelectList(_context.Korisnik, "Id", "Email", sadrzaj.IdAutora);
            ViewData["IdOdobrenOd"] = new SelectList(_context.Korisnik, "Id", "Email", sadrzaj.IdOdobrenOd);
            ViewData["IdTipa"] = new SelectList(_context.TipSadrzaja, "Id", "Ime", sadrzaj.IdTipa);
            return View(sadrzaj);
        }

        // GET: Sadrzaj/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sadrzaj = await _context.Sadrzaj.SingleOrDefaultAsync(m => m.Id == id);
            if (sadrzaj == null)
            {
                return NotFound();
            }
            ViewData["IdAutora"] = new SelectList(_context.Korisnik, "Id", "Email", sadrzaj.IdAutora);
            ViewData["IdOdobrenOd"] = new SelectList(_context.Korisnik, "Id", "Email", sadrzaj.IdOdobrenOd);
            ViewData["IdTipa"] = new SelectList(_context.TipSadrzaja, "Id", "Ime", sadrzaj.IdTipa);
            return View(sadrzaj);
        }

        // POST: Sadrzaj/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdAutora,IdOdobrenOd,IdTipa,Ime,Opis,Url,JeOdobren")] Sadrzaj sadrzaj)
        {
            if (id != sadrzaj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sadrzaj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SadrzajExists(sadrzaj.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["IdAutora"] = new SelectList(_context.Korisnik, "Id", "Email", sadrzaj.IdAutora);
            ViewData["IdOdobrenOd"] = new SelectList(_context.Korisnik, "Id", "Email", sadrzaj.IdOdobrenOd);
            ViewData["IdTipa"] = new SelectList(_context.TipSadrzaja, "Id", "Ime", sadrzaj.IdTipa);
            return View(sadrzaj);
        }

        // GET: Sadrzaj/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sadrzaj = await _context.Sadrzaj
                .Include(s => s.IdAutoraNavigation)
                .Include(s => s.IdOdobrenOdNavigation)
                .Include(s => s.IdTipaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sadrzaj == null)
            {
                return NotFound();
            }

            return View(sadrzaj);
        }

        // POST: Sadrzaj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sadrzaj = await _context.Sadrzaj.SingleOrDefaultAsync(m => m.Id == id);
            _context.Sadrzaj.Remove(sadrzaj);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SadrzajExists(int id)
        {
            return _context.Sadrzaj.Any(e => e.Id == id);
        }

        public IActionResult Next(int id)
        {
            var sadrzaji = _context.Sadrzaj.ToList();
            bool found = false;
            int index = 0;
            for (int i = 0; i < sadrzaji.Count; i = (i + 1) % sadrzaji.Count)
            {
                if (found)
                {
                    index = sadrzaji[i].Id;
                    break;
                }

                if (sadrzaji[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        public IActionResult Previous(int id)
        {
            var sadrzaji = _context.Sadrzaj.ToList();
            bool found = false;
            int index = 0;
            for (int i = sadrzaji.Count - 1; i > -1; i--)
            {
                if (found)
                {
                    index = sadrzaji[i].Id;
                    break;
                }

                if (sadrzaji[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = sadrzaji.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }

        public IActionResult Report(int? id)
        {
            var reportBytes = SadrzajPdfReport.CreateInMemoryPdfReport(_context);
            return File(reportBytes, "application/pdf", "SadrzajReport.pdf");
        }

        // GET: Sadrzaj
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        public IActionResult ApiDetails(int id)
        {
            return View("Api/Details", id);
        }

        public IActionResult ApiEdit(int id)
        {
            return View("Api/Edit", id);
        }
    }
}
