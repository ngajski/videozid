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
    /// Upravljaè za baratanje podacima o ekranima videozidova
    /// </summary>
    public class EkranZidaController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">kontekst za rukovanje bazom podataka</param>
        public EkranZidaController(RPPP15Context context)
        {
            _context = context;    
        }

        // GET: EkranZida
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje popis ekrana videozidova.
        /// </summary>
        /// <returns>Pogled koji sadrži pregled ekrana videozidova</returns>
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.EkranZida.Include(e => e.IdUredajaNavigation).Include(e => e.IdZidaNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Uredaj
        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje popis ekrana videozidova.
        /// </summary>
        /// <returns>Pogled koji sadrži metode za AJAX baratanje ekranima videozidova te odgovarajuæe postupke za njihov prikaz</returns>
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        // GET: EkranZida/Details/5
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje informacije o jednom ekranu videozida.
        /// </summary>
        /// <param name="id">ID ekrana videozida</param>
        /// <returns>Pogled koji prikazuje podatke o ekranu videozida</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ekranZida = await _context.EkranZida
                .Include(e => e.IdUredajaNavigation)
                .Include(e => e.IdZidaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ekranZida == null)
            {
                return NotFound();
            }

            return View(ekranZida);
        }

        /// <summary>
        /// Kontrola za navigiranje izmeðu detalja razlièitih ekrana videozidova unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog ekrana videozida</param>
        /// <returns>Pogled koji prikazuje podatke o ekranu videozida èiji je ID prvi veæi od trenutnog</returns>
        public IActionResult Next(int id)
        {
            var ekrani = _context.EkranZida.ToList();
            bool found = false;
            int index = 0;
            for (int i = 0; i < ekrani.Count; i = (i + 1) % ekrani.Count)
            {
                if (found)
                {
                    index = ekrani[i].Id;
                    break;
                }

                if (ekrani[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        /// <summary>
        /// Kontrola za navigiranje izmeðu detalja razlièitih ekrana videozida unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog ekrana videozida</param>
        /// <returns>Pogled koji prikazuje podatke o ekranu videozida èiji je ID prvi manji od trenutnog</returns>
        public IActionResult Previous(int id)
        {
            var ekrani = _context.EkranZida.ToList();
            bool found = false;
            int index = 0;
            for (int i = ekrani.Count - 1; i > -1; i--)
            {
                if (found)
                {
                    index = ekrani[i].Id;
                    break;
                }

                if (ekrani[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = ekrani.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }

        // GET: EkranZida/Create
        /// <summary>
        /// Postupak za dohvat ekrana koji služi za stvaranje novog ekrana videozida
        /// </summary>
        /// <returns>Pogled za stvaranje novog ekrana videozida</returns>
        public IActionResult Create()
        {
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv");
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Naziv");
            return View();
        }

        // POST: EkranZida/Create
        /// <summary>
        /// Interna metoda koja dohvaæeni ekran videozida sprema u bazu, ukoliko su svi atributi ispravno zadani
        /// </summary>
        /// <param name="ekranZida"> Objekt tipa EkranZida koji sadži informacije o novom ekranu videozida</param>
        /// <returns>Pogled na detelje o novostvorenom ekranu videozida ili pogled za stvaranje novog ekrana videozida s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUredaja,IdZida,XKoord,YKoord")] EkranZida ekranZida)
        {
            if (ModelState.IsValid && EkranZidaValid(ekranZida))
            {

                _context.Add(ekranZida);
                var uredaj = _context.Uredaj.Where(u => u.Id == ekranZida.IdUredaja).FirstOrDefault();
                uredaj.IdStatusa = _context.StatusUredaja.Where(s => s.Naziv.Equals("aktivan")).FirstOrDefault().Id;
                _context.Update(uredaj);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Ekran {_context.Uredaj.Where(u => u.Id == ekranZida.IdUredaja).FirstOrDefault().Naziv} pripada videozidu {_context.Videozid.Where(v => v.Id == ekranZida.IdZida).FirstOrDefault().Naziv}.";

                return RedirectToAction("Details" , new { id = ekranZida.Id });
                
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", ekranZida.IdUredaja);
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Naziv", ekranZida.IdZida);
            return View(ekranZida);
        }

        // GET: EkranZida/Edit/5
        /// <summary>
        /// Postupak za dohvat ekrana za ureðivanje detalja ekrana videozida
        /// </summary>
        /// <param name="id">ID ekrana videozida kojeg se ureðuje</param>
        /// <returns>NotFound ako ekran videozida s navedenim Id-em ne postoji, ekran za ureðivanje ekrana videozida inaèe</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ekranZida = await _context.EkranZida.SingleOrDefaultAsync(m => m.Id == id);
            if (ekranZida == null)
            {
                return NotFound();
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", ekranZida.IdUredaja);
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Lokacija", ekranZida.IdZida);
            return View(ekranZida);
        }

        // POST: EkranZida/Edit/5
        /// <summary>
        /// Interna metoda koja dohvaæeni ekran videozida ažurira primljenim informacijama
        /// </summary>
        /// <param name="id">ID ekrana videozida</param>
        /// <param name="ekranZida">informacije o ekranu videozida</param>
        /// <returns>Pogled na detelje o ažuriranom ekranu videozida ili pogled za ureðivanje videozida s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUredaja,IdZida,XKoord,YKoord")] EkranZida ekranZida)
        {
            if (id != ekranZida.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && EkranZidaValid(ekranZida))
            {

                    try
                    {
                        _context.Update(ekranZida);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EkranZidaExists(ekranZida.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                TempData["Success"] = $"Uspjesno ažuriranje podataka.";
                return RedirectToAction("Details", new { id = ekranZida.Id });
                
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", ekranZida.IdUredaja);
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Lokacija", ekranZida.IdZida);
            TempData["Error"] = $"Neuspjelo ažuriranje podataka.";
            return View(ekranZida);
        }

        /*// GET: EkranZida/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ekranZida = await _context.EkranZida
                .Include(e => e.IdUredajaNavigation)
                .Include(e => e.IdZidaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ekranZida == null)
            {
                return NotFound();
            }

            return View(ekranZida);
        }

        // POST: EkranZida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ekranZida = await _context.EkranZida.SingleOrDefaultAsync(m => m.Id == id);
            _context.EkranZida.Remove(ekranZida);
            await _context.SaveChangesAsync();
            TempData["Error"] = $"Ekran zida uspješno uklonjen.";

            return RedirectToAction("Index");
        }*/

        /// <summary>
        /// Postupak za dohvat pogleda koji prikauje infomacije o ekranu videozida prije nego što æe biti obrisan
        /// </summary>
        /// <param name="id">ID ekrana videozida</param>
        /// <returns>Not Found ako ekran videozida s zadanim ID-em ne postoji, ekran s popisom ekrana videozida inaèe</returns>
        public IActionResult Delete(int? id)
        {
            var ekran = _context.EkranZida.Find(id);
            if (ekran != null)
            {
                _context.EkranZida.Remove(ekran);
                TempData["Error"] = $"Ekran zida uspješno uklonjen.";
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            else
                return NotFound();
        }

        private bool EkranZidaExists(int id)
        {
            return _context.EkranZida.Any(e => e.Id == id);
        }

        /// <summary>
        /// Pomoæna metoda za provjeru ispravnosti stvorenog ekrana videozida.
        /// </summary>
        /// <param name="ekranZida">objekt tipa EkranZida</param>
        /// <returns>true ako je ekran zida za zadani videozida ispravno definiran, false inaèe</returns>
        private bool EkranZidaValid(EkranZida ekranZida)
        {
            Videozid zid = _context.Videozid.Where(z => z.Id == ekranZida.IdZida).FirstOrDefault();
            if (ekranZida.XKoord < zid.Sirina && ekranZida.YKoord < zid.Visina)
            {
                if (_context.EkranZida.Where(e => e.IdZida == ekranZida.IdZida && e.XKoord == ekranZida.XKoord && e.YKoord == ekranZida.YKoord).Count() == 0
                        && _context.EkranZida.Where(e => e.IdUredaja == ekranZida.Id).Count() == 0)
                    return true;
                else
                {
                    TempData["Error"] = $"Unesena pozicija u zidu je zauzeta!";
                    return false;
                }
            }
            else
            {

                TempData["Error"] = $"Unesena pozicija u zidu ne postoji! Dimenzije zida iznose: Sirina - {zid.Sirina} Visina - {zid.Visina}";
                return false;
            }
        }
    }
}
