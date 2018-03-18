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
    public class PrezentacijaController : Controller
    {
        private readonly RPPP15Context _context;

        public PrezentacijaController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: Prezentacija
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.Prezentacija.Include(p => p.IdKategorijeNavigation).Include(p => p.IdSadrzajaNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Prezentacija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prezentacija = await _context.Prezentacija
                .Include(p => p.IdKategorijeNavigation)
                .Include(p => p.IdSadrzajaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (prezentacija == null)
            {
                return NotFound();
            }

            return View(prezentacija);
        }

        // GET: Prezentacija/Create
        public IActionResult Create()
        {
            ViewData["IdKategorije"] = new SelectList(_context.Kategorija, "Id", "Vrsta");
            ViewData["IdSadrzaja"] = new SelectList(_context.Sadrzaj, "Id", "Ime");
            return View();
        }

        // POST: Prezentacija/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdSadrzaja,IdKategorije,XKoord,YKoord,Sirina,Visina")] Prezentacija prezentacija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prezentacija);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["IdKategorije"] = new SelectList(_context.Kategorija, "Id", "Vrsta", prezentacija.IdKategorije);
            ViewData["IdSadrzaja"] = new SelectList(_context.Sadrzaj, "Id", "Ime", prezentacija.IdSadrzaja);
            return View(prezentacija);
        }

        // GET: Prezentacija/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prezentacija = await _context.Prezentacija.SingleOrDefaultAsync(m => m.Id == id);
            if (prezentacija == null)
            {
                return NotFound();
            }
            ViewData["IdKategorije"] = new SelectList(_context.Kategorija, "Id", "Vrsta", prezentacija.IdKategorije);
            ViewData["IdSadrzaja"] = new SelectList(_context.Sadrzaj, "Id", "Ime", prezentacija.IdSadrzaja);
            return View(prezentacija);
        }

        // POST: Prezentacija/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdSadrzaja,IdKategorije,XKoord,YKoord,Sirina,Visina")] Prezentacija prezentacija)
        {
            if (id != prezentacija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prezentacija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrezentacijaExists(prezentacija.Id))
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
            ViewData["IdKategorije"] = new SelectList(_context.Kategorija, "Id", "Vrsta", prezentacija.IdKategorije);
            ViewData["IdSadrzaja"] = new SelectList(_context.Sadrzaj, "Id", "Ime", prezentacija.IdSadrzaja);
            return View(prezentacija);
        }

        // GET: Prezentacija/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prezentacija = await _context.Prezentacija
                .Include(p => p.IdKategorijeNavigation)
                .Include(p => p.IdSadrzajaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (prezentacija == null)
            {
                return NotFound();
            }

            return View(prezentacija);
        }

        // POST: Prezentacija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prezentacija = await _context.Prezentacija.SingleOrDefaultAsync(m => m.Id == id);
            _context.Prezentacija.Remove(prezentacija);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PrezentacijaExists(int id)
        {
            return _context.Prezentacija.Any(e => e.Id == id);
        }

        public IActionResult Next(int id)
        {
            var preze = _context.Prezentacija.ToList();
            bool found = false;
            int index = 0;
            for (int i = 0; i < preze.Count; i = (i + 1) % preze.Count)
            {
                if (found)
                {
                    index = preze[i].Id;
                    break;
                }

                if (preze[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        public IActionResult Previous(int id)
        {
            var preze = _context.Prezentacija.ToList();
            bool found = false;
            int index = 0;
            for (int i = preze.Count - 1; i > -1; i--)
            {
                if (found)
                {
                    index = preze[i].Id;
                    break;
                }

                if (preze[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = preze.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }

        public IActionResult Report(int? id)
        {
            var reportBytes = PrezentacijaPdfReport.CreateInMemoryPdfReport(_context);
            return File(reportBytes, "application/pdf", "PrezReport.pdf");
        }

        // GET: Prezentacija
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
