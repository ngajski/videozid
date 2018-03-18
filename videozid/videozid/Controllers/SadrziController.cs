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
    public class SadrziController : Controller
    {
        private readonly RPPP15Context _context;

        public SadrziController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: Sadrzi
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.Sadrzi.Include(s => s.IdPodshemeNavigation).Include(s => s.IdShemeNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Sadrzi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sadrzi = await _context.Sadrzi
                .Include(s => s.IdPodshemeNavigation)
                .Include(s => s.IdShemeNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sadrzi == null)
            {
                return NotFound();
            }

            return View(sadrzi);
        }

        // GET: Sadrzi/Create
        public IActionResult Create()
        {
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv");
            ViewData["IdSheme"] = new SelectList(_context.ShemaPrikazivanja, "Id", "Naziv");
            return View();
        }

        // POST: Sadrzi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdSheme,IdPodsheme,Od,Do")] Sadrzi sadrzi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sadrzi);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv", sadrzi.IdPodsheme);
            ViewData["IdSheme"] = new SelectList(_context.ShemaPrikazivanja, "Id", "Naziv", sadrzi.IdSheme);
            return View(sadrzi);
        }

        // GET: Sadrzi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sadrzi = await _context.Sadrzi.SingleOrDefaultAsync(m => m.Id == id);
            if (sadrzi == null)
            {
                return NotFound();
            }
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv", sadrzi.IdPodsheme);
            ViewData["IdSheme"] = new SelectList(_context.ShemaPrikazivanja, "Id", "Naziv", sadrzi.IdSheme);
            return View(sadrzi);
        }

        // POST: Sadrzi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdSheme,IdPodsheme,Od,Do")] Sadrzi sadrzi)
        {
            if (id != sadrzi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sadrzi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SadrziExists(sadrzi.Id))
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
            ViewData["IdPodsheme"] = new SelectList(_context.PodshemaPrikazivanja, "Id", "Naziv", sadrzi.IdPodsheme);
            ViewData["IdSheme"] = new SelectList(_context.ShemaPrikazivanja, "Id", "Naziv", sadrzi.IdSheme);
            return View(sadrzi);
        }

        // GET: Sadrzi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sadrzi = await _context.Sadrzi
                .Include(s => s.IdPodshemeNavigation)
                .Include(s => s.IdShemeNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (sadrzi == null)
            {
                return NotFound();
            }

            return View(sadrzi);
        }

        // POST: Sadrzi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sadrzi = await _context.Sadrzi.SingleOrDefaultAsync(m => m.Id == id);
            _context.Sadrzi.Remove(sadrzi);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SadrziExists(int id)
        {
            return _context.Sadrzi.Any(e => e.Id == id);
        }
    }
}
