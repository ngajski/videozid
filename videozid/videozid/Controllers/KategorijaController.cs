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
    public class KategorijaController : Controller
    {
        private readonly RPPP15Context _context;

        public KategorijaController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: Kategorija
        public async Task<IActionResult> Index()
        {
            return View(await _context.Kategorija.ToListAsync());
        }

       
        // GET: Kategorija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategorija = await _context.Kategorija
                .SingleOrDefaultAsync(m => m.Id == id);
            if (kategorija == null)
            {
                return NotFound();
            }

            return View(kategorija);
        }

        // GET: Kategorija/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kategorija/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Vrsta")] Kategorija kategorija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kategorija);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(kategorija);
        }

        // GET: Kategorija/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategorija = await _context.Kategorija.SingleOrDefaultAsync(m => m.Id == id);
            if (kategorija == null)
            {
                return NotFound();
            }
            return View(kategorija);
        }

        // POST: Kategorija/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Vrsta")] Kategorija kategorija)
        {
            if (id != kategorija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategorija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategorijaExists(kategorija.Id))
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
            return View(kategorija);
        }

        // GET: Kategorija/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategorija = await _context.Kategorija
                .SingleOrDefaultAsync(m => m.Id == id);
            if (kategorija == null)
            {
                return NotFound();
            }

            return View(kategorija);
        }

        // POST: Kategorija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategorija = await _context.Kategorija.SingleOrDefaultAsync(m => m.Id == id);
            _context.Kategorija.Remove(kategorija);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool KategorijaExists(int id)
        {
            return _context.Kategorija.Any(e => e.Id == id);
        }

        // GET: Kategorija
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }
    }
}
