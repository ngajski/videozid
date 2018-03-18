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
    /// Upravljaè za baratanje podacima o zamjenskim ureðajima
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
        /// Postupak za dohvat ekrana koji prikazuje popis zamjenskih ureðaja.
        /// </summary>
        /// <returns>Pogled koji sadrži pregled zamjenskih ureðaja</returns>
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.ZamjenskiUredaj.Include(z => z.IdUredajaNavigation).Include(z => z.IdZamjenaZaNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Uredaj
        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje popis zamjenskih ureðaja.
        /// </summary>
        /// <returns>Pogled koji sadrži metode za AJAX baratanje zamjenskim ureðajima te odgovarajuæe postupke za njihov prikaz</returns>
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        // GET: ZamjenskiUredaj/Details/5
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje informacije o jednom zamjenskom ureðaju
        /// </summary>
        /// <param name="id">ID zamjenskog ureðaja</param>
        /// <returns>Pogled koji prikazuje podatke o zamjenskom ureðaju</returns>
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
        /// Kontrola za navigiranje izmeðu detalja razlièitih zamjenskih ureðaja unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog zamjenskog ureðaja</param>
        /// <returns>Pogled koji prikazuje podatke o zamjenskom ureðaju èiji je ID prvi veæi od trenutnog</returns>
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
        /// Kontrola za navigiranje izmeðu detalja razlièitih zamjenskih ureðaja unazad.
        /// </summary>
        /// <param name="id">ID trenutnog zamjenskog ureðaja</param>
        /// <returns>Pogled koji prikazuje podatke o zamjenskom ureðaju èiji je ID prvi manji od trenutnog</returns>
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
        /// Postupak za dohvat ekrana koji služi za stvaranje novog zamjenskog ureðaja
        /// </summary>
        /// <returns>Pogled za stvaranje novog zamjenskog ureðaja</returns>
        public IActionResult Create()
        {
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv");
            ViewData["IdZamjenaZa"] = new SelectList(_context.Uredaj, "Id", "Naziv");
            return View();
        }

        // POST: ZamjenskiUredaj/Create
        /// <summary>
        /// Interna metoda koja dohvaæeni zamjenskih ureðaj sprema u bazu, ukoliko su atributi ispravno zadani
        /// </summary>
        /// <param name="zamjenskiUredaj"> Objekt tipa ZamjenskiUredaj koji sadži informacije o novom zamjenskom ureðaju</param>
        /// <returns>Pogled na detelje o novostvorenom zamjenskom ureðaju ili pogled za stvaranje novog zamjenskog ureðaja s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUredaja,IdZamjenaZa")] ZamjenskiUredaj zamjenskiUredaj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zamjenskiUredaj);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{_context.Uredaj.Where(u => u.Id == zamjenskiUredaj.IdUredaja).FirstOrDefault().Naziv} uspješno definiran kao zamjena uredaju {_context.Uredaj.Where(u => u.Id == zamjenskiUredaj.IdZamjenaZa).FirstOrDefault().Naziv}.";

                return RedirectToAction("Details", new { id = zamjenskiUredaj.Id });
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdUredaja);
            ViewData["IdZamjenaZa"] = new SelectList(_context.Uredaj, "Id", "Naziv", zamjenskiUredaj.IdZamjenaZa);
            return View(zamjenskiUredaj);
        }

        // GET: ZamjenskiUredaj/Edit/5
        /// <summary>
        /// Postupak za dohvat ekrana za ureðivanje detalja zamjenskog ureðaja
        /// </summary>
        /// <param name="id">ID zamjenskog ureðaja kojeg se ureðuje</param>
        /// <returns>NotFound ako zamjenski ureðaj s navedenim Id-em ne postoji, ekran za ureživanje zamjenskog ureðaja inaèe</returns>
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
        /// Interna metoda koja dohvaæeni zamjenski ureðaj ažurira primljenim informacijama
        /// </summary>
        /// <param name="id">ID zamjenskog ureðaja</param>
        /// <param name="zamjenskiUredaj">informacije o zamjenskom ureðaju</param>
        /// <returns>Pogled na detelje o ažuriranom zamjenskom ureðaju ili pogled za ureðivanje zamjenskog ureðaja s porukom o atributima koji su krivo zadani</returns>
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
        /// Postupak za dohvat pogleda koji prikauje infomacije o zamjenskom ureðaju prije nego što æe biti obrisan
        /// </summary>
        /// <param name="id">Id zamjenskog ureðaja</param>
        /// <returns>Not Found ako zamjenski ureðaj s zadanim ID-em ne postoji, ekran s popisom zamjenskih ureðaja inaèe</returns>
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
        /// Pomoæni postupak za provjeru postojanja zamjenskog ureðaja s poslanim ID-em
        /// </summary>
        /// <param name="id">ID zamjenskog ureðaja</param>
        /// <returns>true ako zamjenski ureðaj postoji, false inaèe</returns>
        private bool ZamjenskiUredajExists(int id)
        {
            return _context.ZamjenskiUredaj.Any(e => e.Id == id);
        }
    }
}
