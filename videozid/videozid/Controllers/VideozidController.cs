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
    /// <summary>
    /// Upravljaè za baratanje podacima o videozidovima
    /// </summary>
    public class VideozidController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">kontekst za rukovanje bazom podataka</param>
        public VideozidController(RPPP15Context context)
        {
            _context = context;    
        }

        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje popis videozidova.
        /// </summary>
        /// <returns>Pogled koji sadrži metode za AJAX baratanje videozidovima te odgovarajuæe postupke za njihov prikaz</returns>
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje detalje jednog videozida.
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <returns>Pogled koji sadrži metode za AJAX ureðivanje videozida</returns>
        public IActionResult ApiDetails(int id)
        {
            return View("Api/Details", id);
        }

        /// <summary>
        /// Postupak za dohvat API ekrana koji sadrži kontrole za ažuriranje videozida.
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <returns>Pogled koji sadrži metode za AJAX ureðivanje osnovnih informacija o videozidu</returns>
        public IActionResult ApiEdit(int id)
        {
            return View("Api/Edit", id);
        }

        // GET: Videozid
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje popis videozidova.
        /// </summary>
        /// <returns>Pogled koji sadrži pregled videozidova</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Videozid.ToListAsync());
        }

        // GET: Videozid/Details/5
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje informacije o jednom videozidu.
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <returns>Pogled koji prikazuje podatke o videozidu</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videozid = await _context.Videozid
                .SingleOrDefaultAsync(m => m.Id == id);
            if (videozid == null)
            {
                return NotFound();
            }

            var ekrani = _context.EkranZida.Where(u => u.IdZida == videozid.Id).Include(u => u.IdUredajaNavigation).ToList();

            VideozidDetailsViewModel v = new VideozidDetailsViewModel(videozid, ekrani);

            return View(v);
        }

        /// <summary>
        /// Kontrola za navigiranje izmeðu detalja razlièitih videozidova unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog videozida</param>
        /// <returns>Pogled koji prikazuje podatke o videozidu èiji je ID prvi veæi od trenutnog</returns>
        public IActionResult Next(int id)
        {
            var zidovi = _context.Videozid.ToList();
            bool found = false;
            int index = 0;
            for (int i = 0; i < zidovi.Count; i = (i + 1) % zidovi.Count)
            {
                if (found)
                {
                    index = zidovi[i].Id;
                    break;
                }

                if (zidovi[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        /// <summary>
        /// Kontrola za navigiranje izmeðu detalja razlièitih videozidova unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog videozida</param>
        /// <returns>Pogled koji prikazuje podatke o videozidu èiji je ID prvi manji od trenutnog</returns>
        public IActionResult Previous(int id)
        {
            var zidovi = _context.Videozid.ToList();
            bool found = false;
            int index = 0;
            for (int i = zidovi.Count - 1; i > -1; i--)
            {
                if (found)
                {
                    index = zidovi[i].Id;
                    break;
                }

                if (zidovi[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = zidovi.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }

        // GET: Videozid/Create
        /// <summary>
        /// Postupak za dohvat ekrana koji služi za stvaranje novog videozida
        /// </summary>
        /// <returns>Pogled za stvaranje novog videozida</returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Videozid/Create
        /// <summary>
        /// Interna metoda koja dohvaæeni videozid sprema u bazu, ukoliko su svi atributi ispravno zadani
        /// </summary>
        /// <param name="videozid"> Objekt tipa Videozid koji sadži informacije o novom videozidu</param>
        /// <returns>Pogled na detelje o novostvorenom videozidu ili pogled za stvaranje novog videozida s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Lokacija,Sirina,Visina")] Videozid videozid)
        {
            if (ModelState.IsValid && Check(videozid))
            {
                _context.Add(videozid);

                TempData["Success"] = $"Videozid {videozid.Naziv} uspjesno dodan.";

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = videozid.Id });
            }
            return View(videozid);
        }

        // GET: Videozid/Details/5
        /// <summary>
        /// Pomoæni postupak za provjeru ispravnosti atributa videozida koji se treba stvoriti.
        /// </summary>
        /// <param name="videozid"></param>
        /// <returns>true ako je videozid ispravno zadan, false inaèe</returns>
        private bool Check(Videozid videozid)
        {
            if (videozid.Sirina >= 1 && videozid.Visina >= 1)
                return true;
            else
            {
                TempData["Error"] = $"Zid mora imati pozitivnu sirinu i visinu.";
                return false;

            }

        }

        // GET: Videozid/Edit/5
        /// <summary>
        /// Postupak za dohvat ekrana za ureðivanje detalja videozida
        /// </summary>
        /// <param name="id">ID videozida kojeg se ureðuje</param>
        /// <returns>NotFound ako videozid s navedenim Id-em ne postoji, ekran za ureðivanje videozida inaèe</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videozid = await _context.Videozid.SingleOrDefaultAsync(m => m.Id == id);
            if (videozid == null)
            {
                return NotFound();
            }
            return View(videozid);
        }

        // POST: Videozid/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Interna metoda koja dohvaæeni videozid ažurira primljenim informacijama
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <param name="videozid">informacije o videozidu</param>
        /// <returns>Pogled na detelje o ažuriranom videozidu ili pogled za ureðivanje videozida s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Lokacija,Sirina,Visina")] Videozid videozid)
        {
            if (id != videozid.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && Check(videozid))
            {
                try
                {
                    _context.Update(videozid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideozidExists(videozid.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = $"Videozid {videozid.Naziv} uspješno azuriran.";
                return RedirectToAction("Details", new { id = videozid.Id });
            }
            return View(videozid);
        }

        /*// GET: Videozid/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videozid = await _context.Videozid
                .SingleOrDefaultAsync(m => m.Id == id);
            if (videozid == null)
            {
                return NotFound();
            }

            return View(videozid);
        }

        // POST: Videozid/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var videozid = await _context.Videozid.SingleOrDefaultAsync(m => m.Id == id);
            _context.Videozid.Remove(videozid);

            _context.RemoveRange(_context.EkranZida.Where(e => e.IdZida == id));
            _context.RemoveRange(_context.Prikazuje.Where(p => p.IdZida == id));
            _context.Uredaj.Where(u => u.IdZida == id).ToList().ForEach(u => u.IdZida = null);

            TempData["Error"] = $"Videozid {videozid.Naziv} uspjesno izbrisan.";

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }*/

        // GET: Videozid/Delete/5
        /// <summary>
        /// Postupak za dohvat pogleda koji prikauje infomacije o videozidu prije nego što æe biti obrisan
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <returns>Not Found ako ekran videozida s zadanim ID-em ne postoji, ekran s popisom videozidova inaèe</returns>
        public IActionResult Delete(int? id)
        {
            var videozid = _context.Videozid.Find(id);
            if (videozid != null)
            {
                _context.Videozid.Remove(videozid);

                _context.RemoveRange(_context.EkranZida.Where(e => e.IdZida == id));
                _context.RemoveRange(_context.Prikazuje.Where(p => p.IdZida == id));
                _context.Uredaj.Where(u => u.IdZida == id).ToList().ForEach(u => u.IdZida = null);

                TempData["Error"] = $"Videozid {videozid.Naziv} uspjesno izbrisan.";

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            else
                return NotFound();
        }

        /// <summary>
        /// Pomoæna metoda koja provjerava postoji li videozid s navedenim ID-em
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <returns>true ako vidoezid s primljenim ID-em postoji, false inaèe</returns>
        private bool VideozidExists(int id)
        {
            return _context.Videozid.Any(e => e.Id == id);
        }
    }
}
