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
    public class TipSadrzajaController : Controller
    {
        private readonly RPPP15Context _context;

        public TipSadrzajaController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: TipSadrzaja
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipSadrzaja.ToListAsync());
        }

        // GET: TipSadrzaja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSadrzaja = await _context.TipSadrzaja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tipSadrzaja == null)
            {
                return NotFound();
            }

            return View(tipSadrzaja);
        }

        // GET: TipSadrzaja/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipSadrzaja/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime")] TipSadrzaja tipSadrzaja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipSadrzaja);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tipSadrzaja);
        }

        // GET: TipSadrzaja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSadrzaja = await _context.TipSadrzaja.SingleOrDefaultAsync(m => m.Id == id);
            if (tipSadrzaja == null)
            {
                return NotFound();
            }
            return View(tipSadrzaja);
        }

        // POST: TipSadrzaja/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime")] TipSadrzaja tipSadrzaja)
        {
            if (id != tipSadrzaja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipSadrzaja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipSadrzajaExists(tipSadrzaja.Id))
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
            return View(tipSadrzaja);
        }

        // GET: TipSadrzaja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipSadrzaja = await _context.TipSadrzaja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tipSadrzaja == null)
            {
                return NotFound();
            }

            return View(tipSadrzaja);
        }

        // POST: TipSadrzaja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipSadrzaja = await _context.TipSadrzaja.SingleOrDefaultAsync(m => m.Id == id);
            _context.TipSadrzaja.Remove(tipSadrzaja);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TipSadrzajaExists(int id)
        {
            return _context.TipSadrzaja.Any(e => e.Id == id);
        }

        // GET: TipSadrzaja
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }
    }
}
