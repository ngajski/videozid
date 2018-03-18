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
    /// Upravljaè za baratanje podacima o ureðajima
    /// </summary>
    public class UredajController : Controller
    {
        private readonly RPPP15Context _context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">kontekst za rukovanje bazom podataka</param>
        public UredajController(RPPP15Context context)
        {
            _context = context;
        }

        // GET: Uredaj
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje popis ureðaja.
        /// </summary>
        /// <returns>Pogled koji sadrži pregled ureðaja</returns>
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = _context.Uredaj.Include(u => u.IdNadredeneKomponenteNavigation).Include(u => u.IdStatusaNavigation).Include(u => u.IdZidaNavigation);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Uredaj
        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje popis ureðaja.
        /// </summary>
        /// <returns>Pogled koji sadrži metode za AJAX baratanje ureðajima te odgovarajuæe postupke za njihov prikaz</returns>
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        /// <summary>
        /// Postupak za dohvat API ekrana koji prikazuje detalje jednog ureðaja.
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <returns>Pogled koji sadrži metode za AJAX ureðivanje ureðaja</returns>
        public IActionResult ApiDetails(int id)
        {
            return View("Api/Details", id);
        }

        /// <summary>
        /// Postupak za dohvat API ekrana koji sadrži kontrole za ažuriranje ureðaja.
        /// </summary>
        /// <param name="id">ID videozida</param>
        /// <returns>Pogled koji sadrži metode za AJAX ureðivanje osnovnih informacija o ureðaju</returns>
        public IActionResult ApiEdit(int id)
        {
            return View("Api/Edit", id);
        }

        // GET: Uredaj/Details/5
        /// <summary>
        /// Postupak za dohvat ekrana koji prikazuje informacije o jednom ureðaju
        /// </summary>
        /// <param name="id">ID ureðaja</param>
        /// <returns>Pogled koji prikazuje podatke o ureðaju</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uredaj = await _context.Uredaj
                .Include(u => u.IdNadredeneKomponenteNavigation)
                .Include(u => u.IdStatusaNavigation)
                .Include(u => u.IdZidaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);

            var ur = LoadDetails(uredaj);

            return View(ur);
        }

        /// <summary>
        /// Kontrola za navigiranje izmeðu detalja razlièitih ureðaja unaprijed
        /// </summary>
        /// <param name="id">ID trenutnog ureðaja</param>
        /// <returns>Pogled koji prikazuje podatke o ureðaju èiji je ID prvi veæi od trenutnog</returns>
        public IActionResult Next(int id)
        {
            var uredaji = _context.Uredaj.ToList();
            bool found = false;
            int index = 0;
            for(int i=0; i<uredaji.Count; i=(i+1)%uredaji.Count)
            {
                if (found)
                {
                    index = uredaji[i].Id;
                    break;
                }

                if (uredaji[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        /// <summary>
        /// Kontrola za navigiranje izmeðu detalja razlièitih ureðaja unazad.
        /// </summary>
        /// <param name="id">ID trenutnog ureðaja</param>
        /// <returns>Pogled koji prikazuje podatke o ureðaju èiji je ID prvi manji od trenutnog</returns>
        public IActionResult Previous(int id)
        {
            var uredaji = _context.Uredaj.ToList();
            bool found = false;
            int index = 0;
            for (int i = uredaji.Count -1 ; i > -1; i--)
            {
                if (found)
                {
                    index = uredaji[i].Id;
                    break;
                }

                if (uredaji[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = uredaji.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }

        // GET: Uredaj/Create
        /// <summary>
        /// Postupak za dohvat ekrana koji služi za stvaranje novog ureðaja
        /// </summary>
        /// <returns>Pogled za stvaranje novog ureðaja</returns>
        public IActionResult Create()
        {
            ViewData["IdNadredeneKomponente"] = new SelectList(_context.Uredaj, "Id", "Naziv");
            ViewData["IdStatusa"] = new SelectList(_context.StatusUredaja, "Id", "Naziv");
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Naziv", null);
            return View();
        }

        // POST: Uredaj/Create
        /// <summary>
        /// Interna metoda koja dohvaæeni ureðaj sprema u bazu, ukoliko su atributi ispravno zadani
        /// </summary>
        /// <param name="uredaj"> Objekt tipa Uredaj koji sadži informacije o novom ureðaju</param>
        /// <returns>Pogled na detelje o novostvorenom ureðaju ili pogled za stvaranje novog ureðaja s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,NabavnaCijena,AktualnaCijena,DatumNabavke,IdNadredeneKomponente,IdZida,IdStatusa")] Uredaj uredaj)
        { 

            if (ModelState.IsValid)
            {
                if (uredaj.IdNadredeneKomponente.HasValue)
                {
                    uredaj.IdZida = _context.Uredaj.Where(u => u.Id == uredaj.IdNadredeneKomponente).FirstOrDefault().IdZida;
                    uredaj.IdStatusa = _context.Uredaj.Where(u => u.Id == uredaj.IdNadredeneKomponente).FirstOrDefault().IdStatusa;

                }

                _context.Add(uredaj);

                TempData["Success"] = $"Uredaj {uredaj.Naziv} dodan.";

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = uredaj.Id });
            }

            ViewData["IdNadredeneKomponente"] = new SelectList(_context.Uredaj, "Id", "Naziv", uredaj.IdNadredeneKomponente);
            ViewData["IdStatusa"] = new SelectList(_context.StatusUredaja, "Id", "Naziv", uredaj.IdStatusa);
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Lokacija", uredaj.IdZida);

            return View(uredaj);
        }

        // GET: Uredaj/Edit/5
        /// <summary>
        /// Postupak za dohvat ekrana za ureðivanje detalja ureðaja
        /// </summary>
        /// <param name="id">ID ureðaja kojeg se ureðuje</param>
        /// <returns>NotFound ako ureðaj s navedenim Id-em ne postoji, ekran za ureživanje ureðaja inaèe</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uredaj = await _context.Uredaj.SingleOrDefaultAsync(m => m.Id == id);
            if (uredaj == null)
            {
                return NotFound();
            }
            ViewData["IdNadredeneKomponente"] = new SelectList(_context.Uredaj, "Id", "Naziv", uredaj.IdNadredeneKomponente);
            ViewData["IdStatusa"] = new SelectList(_context.StatusUredaja, "Id", "Naziv", uredaj.IdStatusa);
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Naziv", uredaj.IdZida);
            return View(uredaj);
        }

        // POST: Uredaj/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Interna metoda koja dohvaæeni ureðaj ažurira primljenim informacijama
        /// </summary>
        /// <param name="id">ID ureðaja</param>
        /// <param name="uredaj">infomacije o ureðaju</param>
        /// <returns>Pogled na detelje o ažuriranom ureðaju ili pogled za ureðivanje ureðaja s porukom o atributima koji su krivo zadani</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,NabavnaCijena,AktualnaCijena,DatumNabavke,IdNadredeneKomponente,IdZida,IdStatusa")] Uredaj uredaj)
        {
            if (id != uredaj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (uredaj.IdNadredeneKomponente.HasValue)
                    {
                        uredaj.IdZida = _context.Uredaj.Where(u => u.Id == uredaj.IdNadredeneKomponente).FirstOrDefault().IdZida;
                        uredaj.IdStatusa = _context.Uredaj.Where(u => u.Id == uredaj.IdNadredeneKomponente).FirstOrDefault().IdStatusa;

                    }
                    TempData["Success"] = $"Uredaj {uredaj.Naziv} uspjesno ureden.";

                    _context.Update(uredaj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UredajExists(uredaj.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                if(uredaj.IdStatusa == _context.StatusUredaja.Where(u => u.Naziv.Equals("zamjenski")).Single().Id)
                {
                    var list = _context.ZamjenskiUredaj.Where(u => u.IdZamjenaZa == uredaj.Id).ToList();

                    foreach(var el in list)
                    {
                        _context.Remove(el);
                    }
                    await _context.SaveChangesAsync();

                }
                return RedirectToAction("Details", new { id = uredaj.Id });
            }
            ViewData["IdNadredeneKomponente"] = new SelectList(_context.Uredaj, "Id", "Naziv", uredaj.IdNadredeneKomponente);
            ViewData["IdStatusa"] = new SelectList(_context.StatusUredaja, "Id", "Naziv", uredaj.IdStatusa);
            ViewData["IdZida"] = new SelectList(_context.Videozid, "Id", "Naziv", uredaj.IdZida);
            return View(uredaj);
        }

        /*// GET: Uredaj/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uredaj = await _context.Uredaj
                .Include(u => u.IdNadredeneKomponenteNavigation)
                .Include(u => u.IdStatusaNavigation)
                .Include(u => u.IdZidaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (uredaj == null)
            {
                return NotFound();
            }

            return View(uredaj);
        }

        // POST: Uredaj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uredaj = await _context.Uredaj.SingleOrDefaultAsync(m => m.Id == id);
            _context.Uredaj.Remove(uredaj);

            _context.RemoveRange(_context.ZamjenskiUredaj.Where(u => u.IdUredaja == id || u.IdZamjenaZa == id));
            _context.RemoveRange(_context.EkranZida.Where(e => e.IdUredaja == id));
            _context.RemoveRange(_context.Servisira.Where(s => s.IdUredaj == id));

            _context.Uredaj.Where(u => u.IdNadredeneKomponente == id).ToList().ForEach(u => u.IdNadredeneKomponente = null);

            await _context.SaveChangesAsync();

            TempData["Error"] = $"Uredaj {uredaj.Naziv} uspjesno izbrisan.";
            return RedirectToAction("Index");
        }
        */
        /// <summary>
        /// Postupak za dohvat pogleda koji prikauje infomacije o ureðaju prije nego što æe biti obrisan
        /// </summary>
        /// <param name="id">ID ureðaja</param>
        /// <returns>Not Found ako ureðaj s zadanim ID-em ne postoji, ekran s popisom ureðaja inaèe</returns>
        public IActionResult Delete(int? id)
        {
            Uredaj uredaj = _context.Uredaj.Find(id);

            if (uredaj != null)
            {
                _context.Uredaj.Remove(uredaj);

                _context.RemoveRange(_context.ZamjenskiUredaj.Where(u => u.IdUredaja == id || u.IdZamjenaZa == id));
                _context.RemoveRange(_context.EkranZida.Where(e => e.IdUredaja == id));
                _context.RemoveRange(_context.Servisira.Where(s => s.IdUredaj == id));

                _context.Uredaj.Where(u => u.IdNadredeneKomponente == id).ToList().ForEach(u => u.IdNadredeneKomponente = null);

                _context.SaveChanges();

                TempData["Error"] = $"Uredaj {uredaj.Naziv} uspjesno izbrisan.";
            }

            //var uredaji = _context.Uredaj.Include(u => u.IdNadredeneKomponenteNavigation).Include(u => u.IdStatusaNavigation).Include(u => u.IdZidaNavigation).ToList();
            //return View("Index", uredaji);
            return RedirectToAction("Index");
        }

        /*public void DeleteAsync(int? id)
        {
            Uredaj uredaj = _context.Uredaj.Find(id);

            if(uredaj != null)
            {
                _context.Uredaj.Remove(uredaj);

                _context.RemoveRange(_context.ZamjenskiUredaj.Where(u => u.IdUredaja == id || u.IdZamjenaZa == id));
                _context.RemoveRange(_context.EkranZida.Where(e => e.IdUredaja == id));
                _context.RemoveRange(_context.Servisira.Where(s => s.IdUredaj == id));

                _context.Uredaj.Where(u => u.IdNadredeneKomponente == id).ToList().ForEach(u => u.IdNadredeneKomponente = null);

                _context.SaveChanges();

               // TempData["Error"] = $"Uredaj {uredaj.Naziv} uspjesno izbrisan.";
            }
        }*/

        /// <summary>
        /// Pomoæni postupak za provjeru postojanja ureðaja s poslanim ID-em
        /// </summary>
        /// <param name="id">ID ureðaja</param>
        /// <returns>true ako ureðaj postoji, false inaèe</returns>
        private bool UredajExists(int id)
        {
            return _context.Uredaj.Any(e => e.Id == id);
        }

        /// <summary>
        /// Pomoæni postupak za stvaranje objekta tipa UredajDetailsViewModel na temelju objekta tipa Uredaj.
        /// </summary>
        /// <param name="ur">Ureðaj kojeg treba pretvorit</param>
        /// <returns>objekt tipa UredajDetailsViewModel</returns>
        private UredajDetailsViewModel LoadDetails(Uredaj ur)
        {

            var zamjenaZa = _context.ZamjenskiUredaj.Where(u => u.IdUredaja == ur.Id).Include(u => u.IdZamjenaZaNavigation).ToList();
            var zamjena = _context.ZamjenskiUredaj.Where(u => u.IdZamjenaZa == ur.Id).Include(u => u.IdUredajaNavigation).ToList();
            var servisira = _context.Servisira.Where(s => s.IdUredaj == ur.Id).Include(s => s.IdServisNavigation);

            UredajDetailsViewModel ure = new UredajDetailsViewModel(ur, zamjenaZa, zamjena, servisira);
            return ure;
        }
    }
}
