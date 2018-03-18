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
    public class KoristiController : Controller
    {
        private readonly RPPP15Context _context;

        public KoristiController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: Koristi
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.Koristi.Include(k => k.IdPodshemeNavigation).Include(k => k.IdPrezentacijeNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Koristi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var koristi = await _context.Koristi
                .Include(k => k.IdPodshemeNavigation)
                .Include(k => k.IdPrezentacijeNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (koristi == null)
            {
                return NotFound();
            }

            return View(koristi);
        }

        // GET: Koristi/Create
        public IActionResult Create()
        {
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv");
            ViewData["IdPrezentacije"] = new SelectList(_context.Prezentacija, "Id", "Id");
            return View();
        }

        // POST: Koristi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdPodsheme,IdPrezentacije,Od,Do")] Koristi koristi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(koristi);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv", koristi.IdPodsheme);
            ViewData["IdPrezentacije"] = new SelectList(_context.Prezentacija, "Id", "Id", koristi.IdPrezentacije);
            return View(koristi);
        }

        // GET: Koristi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var koristi = await _context.Koristi.SingleOrDefaultAsync(m => m.Id == id);
            if (koristi == null)
            {
                return NotFound();
            }
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv", koristi.IdPodsheme);
            ViewData["IdPrezentacije"] = new SelectList(_context.Prezentacija, "Id", "Id", koristi.IdPrezentacije);
            return View(koristi);
        }

        // POST: Koristi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPodsheme,IdPrezentacije,Od,Do")] Koristi koristi)
        {
            if (id != koristi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(koristi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KoristiExists(koristi.Id))
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
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv", koristi.IdPodsheme);
            ViewData["IdPrezentacije"] = new SelectList(_context.Prezentacija, "Id", "Id", koristi.IdPrezentacije);
            return View(koristi);
        }

        // GET: Koristi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var koristi = await _context.Koristi
                .Include(k => k.IdPodshemeNavigation)
                .Include(k => k.IdPrezentacijeNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (koristi == null)
            {
                return NotFound();
            }

            return View(koristi);
        }

        // POST: Koristi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var koristi = await _context.Koristi.SingleOrDefaultAsync(m => m.Id == id);
            _context.Koristi.Remove(koristi);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool KoristiExists(int id)
        {
            return _context.Koristi.Any(e => e.Id == id);
        }
    }
}
