using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using videozid.Models;
using videozid.ViewModels;

namespace videozid.Controllers
{
    public class ShemaPrikazivanjaController : Controller
    {
        private readonly RPPP15Context _context;

        public ShemaPrikazivanjaController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: ShemaPrikazivanja
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShemaPrikazivanja.ToListAsync());
        }

        // GET: ShemaPrikazivanja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shemaPrikazivanja = await _context.ShemaPrikazivanja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (shemaPrikazivanja == null)
            {
                return NotFound();
            }

            var podsheme = _context.Sadrzi.Where(u => u.IdSheme == id).Include(u => u.IdPodshemeNavigation).ToList();
            //var sadrzi = _context.Sadrzi.Where(u => u.IdSheme == id).Include(u => u.Id).ToList();
            
            ShemaPrikazivanjaDetailsViewModel shema = new ShemaPrikazivanjaDetailsViewModel(shemaPrikazivanja, podsheme);
            return View(shema);
        }

        // GET: ShemaPrikazivanja/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShemaPrikazivanja/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Opis")] ShemaPrikazivanja shemaPrikazivanja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shemaPrikazivanja);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shemaPrikazivanja);
        }

        // GET: ShemaPrikazivanja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shemaPrikazivanja = await _context.ShemaPrikazivanja.SingleOrDefaultAsync(m => m.Id == id);
            if (shemaPrikazivanja == null)
            {
                return NotFound();
            }
            return View(shemaPrikazivanja);
        }

        // POST: ShemaPrikazivanja/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Opis")] ShemaPrikazivanja shemaPrikazivanja)
        {
            if (id != shemaPrikazivanja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shemaPrikazivanja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShemaPrikazivanjaExists(shemaPrikazivanja.Id))
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
            return View(shemaPrikazivanja);
        }

        // GET: ShemaPrikazivanja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shemaPrikazivanja = await _context.ShemaPrikazivanja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (shemaPrikazivanja == null)
            {
                return NotFound();
            }

            return View(shemaPrikazivanja);
        }

        // POST: ShemaPrikazivanja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shemaPrikazivanja = await _context.ShemaPrikazivanja.SingleOrDefaultAsync(m => m.Id == id);
            _context.ShemaPrikazivanja.Remove(shemaPrikazivanja);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ShemaPrikazivanjaExists(int id)
        {
            return _context.ShemaPrikazivanja.Any(e => e.Id == id);
        }

        public IActionResult Report(int? id)
        {
            var reportBytes = ShemaPrikazivanjaPdfReport.CreateInMemoryPdfReport(_context);
            return File(reportBytes, "application/pdf", "ShemaReport.pdf");
        }
    }
}
