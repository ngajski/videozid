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
    /// <summary>
    /// Upravlja� za baratanje podacima o zamjenskim ure�ajima
    /// </summary>
    public class ZamjenskiUredajController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">kontekst za rukovanje bazom podataka</param>
        public ZamjenskiUredajController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: ZamjenskiUredaj
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje popis zamjenskih ure�aja.
        /// </summary>
        /// <returns>Pogled koji sadr�i pregled zamjenskih ure�aja</returns>
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.ZamjenskiUredaj.Include(z => z.IdUredajaNavigation).Include(z => z.IdZamjenaZaNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Uredaj
        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje popis zamjenskih ure�aja.
        /// </summary>
        /// <returns>Pogled koji sadr�i metode za AJAX baratanje zamjenskim ure�ajima te odgovaraju�e postupke za njihov prikaz</returns>
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        // GET: ZamjenskiUredaj/Details/5
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje informacije o jednom zamjenskom ure�aju
        /// </summary>
        /// <param name="id">ID zamjenskog ure�aja</param>
        /// <returns>Pogled koji prikazuje podatke o zamjenskom ure�aju</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zamjenskiUredaj = await _context.ZamjenskiUredaj
                .Include(z => z.IdUredajaNavigation)
                .Include(z => z.IdZamjenaZaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (zamjenskiUredaj == null)
            {
                return NotFound();
            }

            return View(zamjenskiUredaj);
        }

        /// <summary>
        /// Kontrola za navigiranje izme�u detalja razli�itih zamjenskih ure�aja unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog zamjenskog ure�aja</param>
        /// <returns>Pogled koji prikazuje podatke o zamjenskom ure�aju �iji je ID prvi ve�i od trenutnog</returns>
        public IActionResult Next(int id)
        {
            var zamj = _context.ZamjenskiUredaj.ToList();
            bool found = false;
            int index = 0;
            for (int i = 0; i < zamj.Count; i = (i + 1) % zamj.Count)
            {
                if (found)
                {
                    index = zamj[i].Id;
                    break;
                }

                if (zamj[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        /// <summary>
        /// Kontrola za navigiranje izme�u detalja razli�itih zamjenskih ure�aja unazad.
        /// </summary>
        /// <param name="id">ID trenutnog zamjenskog ure�aja</param>
        /// <returns>Pogled koji prikazuje podatke o zamjenskom ure�aju �iji je ID prvi manji od trenutnog</returns>
        public IActionResult Previous(int id)
        {
            var zamj = _context.ZamjenskiUredaj.ToList();
            bool found = false;
            int index = 0;
            for (int i = zamj.Count - 1; i > -1; i--)
            {
                if (found)
                {
                    index = zamj[i].Id;
                    break;
                }

                if (zamj[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = zamj.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }

        // GET: ZamjenskiUredaj/Create
        /// <summary>
        /// Postupak za dohvat ekrana koji slu�i za stvaranje novog zamjenskog ure�aja
        /// </summary>
        /// <returns>Pogled za stvaranje novog zamjenskog ure�aja</returns>
        public IActionResult Create()
        {
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv");
            ViewData["IdZamjenaZa"] = new SelectList(_context.Uredaj, "Id", "Naziv");
            return View();
        }

        // POST: ZamjenskiUredaj/Create
        /// <summary>
        /// Interna metoda koja dohva�eni zamjenskih ure�aj sprema u bazu, ukoliko su atributi ispravno zadani
        /// </summary>
        /// <param name="zamjenskiUredaj"> Objekt tipa ZamjenskiUredaj koji sad�i informacije o novom zamjenskom ure�aju</param>
        /// <returns>Pogled na detelje o novostvorenom zamjenskom ure�aju ili pogled za stvaranje novog zamjenskog ure�aja s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUredaja,IdZamjenaZa")] ZamjenskiUredaj zamjenskiUredaj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zamjenskiUredaj);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{_context.Uredaj.Where(u => u.Id == zamjenskiUredaj.IdUredaja).FirstOrDefault().Naziv} uspje�no definiran kao zamjena uredaju {_context.Uredaj.Where(u => u.Id == zamjenskiUredaj.IdZamjenaZa).FirstOrDefault().Naziv}.";

                return RedirectToAction("Details", new { id = zamjenskiUredaj.Id });
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdUredaja);
            ViewData["IdZamjenaZa"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdZamjenaZa);
            return View(zamjenskiUredaj);
        }

        // GET: ZamjenskiUredaj/Edit/5
        /// <summary>
        /// Postupak za dohvat ekrana za ure�ivanje detalja zamjenskog ure�aja
        /// </summary>
        /// <param name="id">ID zamjenskog ure�aja kojeg se ure�uje</param>
        /// <returns>NotFound ako zamjenski ure�aj s navedenim Id-em ne postoji, ekran za ure�ivanje zamjenskog ure�aja ina�e</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zamjenskiUredaj = await _context.ZamjenskiUredaj.SingleOrDefaultAsync(m => m.Id == id);
            if (zamjenskiUredaj == null)
            {
                return NotFound();
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdUredaja);
            ViewData["IdZamjenaZa"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdZamjenaZa);
            return View(zamjenskiUredaj);
        }

        // POST: ZamjenskiUredaj/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Interna metoda koja dohva�eni zamjenski ure�aj a�urira primljenim informacijama
        /// </summary>
        /// <param name="id">ID zamjenskog ure�aja</param>
        /// <param name="zamjenskiUredaj">informacije o zamjenskom ure�aju</param>
        /// <returns>Pogled na detelje o a�uriranom zamjenskom ure�aju ili pogled za ure�ivanje zamjenskog ure�aja s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUredaja,IdZamjenaZa")] ZamjenskiUredaj zamjenskiUredaj)
        {
            if (id != zamjenskiUredaj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zamjenskiUredaj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZamjenskiUredajExists(zamjenskiUredaj.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = $"Uspjeh.";
                return RedirectToAction("Details", new { id = zamjenskiUredaj.Id });
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdUredaja);
            ViewData["IdZamjenaZa"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdZamjenaZa);
            TempData["Error"] = $"Greska.";
            return View(zamjenskiUredaj);
        }

        /* // GET: ZamjenskiUredaj/Delete/5
         public async Task<IActionResult> Delete(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var zamjenskiUredaj = await _context.ZamjenskiUredaj
                 .Include(z => z.IdUredajaNavigation)
                 .Include(z => z.IdZamjenaZaNavigation)
                 .SingleOrDefaultAsync(m => m.Id == id);
             if (zamjenskiUredaj == null)
             {
                 return NotFound();
             }

             return View(zamjenskiUredaj);
         }

         // POST: ZamjenskiUredaj/Delete/5
         [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteConfirmed(int id)
         {
             var zamjenskiUredaj = await _context.ZamjenskiUredaj.SingleOrDefaultAsync(m => m.Id == id);
             _context.ZamjenskiUredaj.Remove(zamjenskiUredaj);
             TempData["Error"] = $"Zamjenski uredaj uspjesno uklonjen.";

             await _context.SaveChangesAsync();
             return RedirectToAction("Index");
         }
         */
        /// <summary>
        /// Postupak za dohvat pogleda koji prikauje infomacije o zamjenskom ure�aju prije nego �to �e biti obrisan
        /// </summary>
        /// <param name="id">Id zamjenskog ure�aja</param>
        /// <returns>Not Found ako zamjenski ure�aj s zadanim ID-em ne postoji, ekran s popisom zamjenskih ure�aja ina�e</returns>
        public IActionResult Delete(int? id)
        {
            var zamjenskiUredaj = _context.ZamjenskiUredaj.Find(id);

            if (zamjenskiUredaj != null)
            {
                _context.ZamjenskiUredaj.Remove(zamjenskiUredaj);
                TempData["Error"] = $"Zamjenski uredaj uspjesno uklonjen.";

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            else
                return NotFound();
        }

        /// <summary>
        /// Pomo�ni postupak za provjeru postojanja zamjenskog ure�aja s poslanim ID-em
        /// </summary>
        /// <param name="id">ID zamjenskog ure�aja</param>
        /// <returns>true ako zamjenski ure�aj postoji, false ina�e</returns>
        private bool ZamjenskiUredajExists(int id)
        {
            return _context.ZamjenskiUredaj.Any(e => e.Id == id);
        }
    }
}
