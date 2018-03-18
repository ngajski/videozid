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
    public class PodshemaPrikazivanjaController : Controller
    {
        private readonly RPPP15Context _context;

        public PodshemaPrikazivanjaController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: PodshemaPrikazivanja
        public async Task<IActionResult> Index()
        {
            return View(await _context.PodshemaPrikazivanja.ToListAsync());
        }

        // GET: PodshemaPrikazivanja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podshemaPrikazivanja = await _context.PodshemaPrikazivanja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (podshemaPrikazivanja == null)
            {
                return NotFound();
            }

            var prezentacije = _context.Koristi.Where(u => u.IdPodsheme == id).Include(u => u.IdPrezentacijeNavigation).ToList();
            PodshemaPrikazivanjaDetailsViewModel podshema = new PodshemaPrikazivanjaDetailsViewModel(podshemaPrikazivanja, prezentacije);
            return View(podshema);
        }

        // GET: PodshemaPrikazivanja/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PodshemaPrikazivanja/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Opis")] PodshemaPrikazivanja podshemaPrikazivanja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(podshemaPrikazivanja);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(podshemaPrikazivanja);
        }

        // GET: PodshemaPrikazivanja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podshemaPrikazivanja = await _context.PodshemaPrikazivanja.SingleOrDefaultAsync(m => m.Id == id);
            if (podshemaPrikazivanja == null)
            {
                return NotFound();
            }
            return View(podshemaPrikazivanja);
        }

        // POST: PodshemaPrikazivanja/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Opis")] PodshemaPrikazivanja podshemaPrikazivanja)
        {
            if (id != podshemaPrikazivanja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(podshemaPrikazivanja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PodshemaPrikazivanjaExists(podshemaPrikazivanja.Id))
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
            return View(podshemaPrikazivanja);
        }

        // GET: PodshemaPrikazivanja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podshemaPrikazivanja = await _context.PodshemaPrikazivanja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (podshemaPrikazivanja == null)
            {
                return NotFound();
            }

            return View(podshemaPrikazivanja);
        }

        // POST: PodshemaPrikazivanja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var podshemaPrikazivanja = await _context.PodshemaPrikazivanja.SingleOrDefaultAsync(m => m.Id == id);
            _context.PodshemaPrikazivanja.Remove(podshemaPrikazivanja);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PodshemaPrikazivanjaExists(int id)
        {
            return _context.PodshemaPrikazivanja.Any(e => e.Id == id);
        }

        public IActionResult Report(int? id)
        {
            var reportBytes = PodshemaPrikazivanjaPdfReport.CreateInMemoryPdfReport(_context);
            return File(reportBytes, "application/pdf", "PodshemaReport.pdf");
        }
    }
}
