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
    /// Upravlja� za baratanje podacima o ekranima videozidova
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
        /// <returns>Pogled koji sadr�i pregled ekrana videozidova</returns>
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.EkranZida.Include(e => e.IdUredajaNavigation).Include(e => e.IdZidaNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Uredaj
        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje popis ekrana videozidova.
        /// </summary>
        /// <returns>Pogled koji sadr�i metode za AJAX baratanje ekranima videozidova te odgovaraju�e postupke za njihov prikaz</returns>
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
        /// Kontrola za navigiranje izme�u detalja razli�itih ekrana videozidova unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog ekrana videozida</param>
        /// <returns>Pogled koji prikazuje podatke o ekranu videozida �iji je ID prvi ve�i od trenutnog</returns>
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
        /// Kontrola za navigiranje izme�u detalja razli�itih ekrana videozida unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog ekrana videozida</param>
        /// <returns>Pogled koji prikazuje podatke o ekranu videozida �iji je ID prvi manji od trenutnog</returns>
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
        /// Postupak za dohvat ekrana koji slu�i za stvaranje novog ekrana videozida
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
        /// Interna metoda koja dohva�eni ekran videozida sprema u bazu, ukoliko su svi atributi ispravno zadani
        /// </summary>
        /// <param name="ekranZida"> Objekt tipa EkranZida koji sad�i informacije o novom ekranu videozida</param>
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
        /// Postupak za dohvat ekrana za ure�ivanje detalja ekrana videozida
        /// </summary>
        /// <param name="id">ID ekrana videozida kojeg se ure�uje</param>
        /// <returns>NotFound ako ekran videozida s navedenim Id-em ne postoji, ekran za ure�ivanje ekrana videozida ina�e</returns>
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
        /// Interna metoda koja dohva�eni ekran videozida a�urira primljenim informacijama
        /// </summary>
        /// <param name="id">ID ekrana videozida</param>
        /// <param name="ekranZida">informacije o ekranu videozida</param>
        /// <returns>Pogled na detelje o a�uriranom ekranu videozida ili pogled za ure�ivanje videozida s porukom o atributima koji su krivo zadani</returns>
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
                TempData["Success"] = $"Uspjesno a�uriranje podataka.";
                return RedirectToAction("Details", new { id = ekranZida.Id });
                
            }
            ViewData["IdUredaja"] = new SelectList(_context.Uredaj, "Id", "Naziv", ekranZida.IdUredaja);
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Lokacija", ekranZida.IdZida);
            TempData["Error"] = $"Neuspjelo a�uriranje podataka.";
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
            TempData["Error"] = $"Ekran zida uspje�no uklonjen.";

            return RedirectToAction("Index");
        }*/

        /// <summary>
        /// Postupak za dohvat pogleda koji prikauje infomacije o ekranu videozida prije nego �to �e biti obrisan
        /// </summary>
        /// <param name="id">ID ekrana videozida</param>
        /// <returns>Not Found ako ekran videozida s zadanim ID-em ne postoji, ekran s popisom ekrana videozida ina�e</returns>
        public IActionResult Delete(int? id)
        {
            var ekran = _context.EkranZida.Find(id);
            if (ekran != null)
            {
                _context.EkranZida.Remove(ekran);
                TempData["Error"] = $"Ekran zida uspje�no uklonjen.";
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
        /// Pomo�na metoda za provjeru ispravnosti stvorenog ekrana videozida.
        /// </summary>
        /// <param name="ekranZida">objekt tipa EkranZida</param>
        /// <returns>true ako je ekran zida za zadani videozida ispravno definiran, false ina�e</returns>
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
