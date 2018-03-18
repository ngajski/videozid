using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using Microsoft.EntityFrameworkCore;
using videozid.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace videozid.Controllers
{
    /// <summary>
    /// Upravljač za rad s Dhmz računima
    /// </summary>
    public class DhmzAccController : Controller
    {
        private readonly RPPP15Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">VideozidContext za pristup bazi podataka</param>
        public DhmzAccController(RPPP15Context context)
        {
            this.context = context;
        }


        // GET: Dhmz računi
        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        public IActionResult ApiDetails(int id)
        {
           
            DhmzAcc racun = context.DhmzAcc.Where(r => r.Id.Equals(id)).First();
            string korisnickoIme = racun.KorisnickoIme;

            ViewBag.id = id;

            return View("Api/Details", korisnickoIme);
        }

        public IActionResult ApiEdit(int id)
        {
            DhmzAcc racun = context.DhmzAcc.Where(r => r.Id.Equals(id)).First();
            string korisnickoIme = racun.KorisnickoIme;

            ViewBag.id = id;

            return View("Api/Edit", korisnickoIme);
        }


        //Index 
        /// <summary>
        /// Postupak za dohvat liste Dhmz računa
        /// </summary>
        /// <returns>Lista administratora</returns>
        public async Task<IActionResult> Index()
        {
            var racuni = context.DhmzAcc.Include(fk => fk.Korisnik);
            return View(await racuni.ToListAsync());
        }

        // GET: Details/id
        /// <summary>
        /// Postupak za dohvat pojedinačnog Dhmz računa  
        /// </summary>
        /// <param name="id">Id od traženog Dhmz računa</param>
        /// <returns>Primjerak traženog računa ili status da nije pronađen</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var racun = await context.DhmzAcc
                .SingleOrDefaultAsync(m => m.Id == id);
            if (racun == null)
            {
                return NotFound();
            }
            var korisnik = context.Korisnik.Where(k => k.DhmzId.Equals(id)).First();

            DhmzAccDetailsViewModel view = new DhmzAccDetailsViewModel(racun, korisnik);

            return View(view);
        }

        // GET: DhmzAcc/Create
        /// <summary>
        /// Postupak za dohvat forme za unos novog Dhmz računa
        /// </summary>
        /// <param name="id">Id korisnika kojemu će se pridjeliti novi Dhmz račun</param>
        /// <returns>Forma za unos novog računa</returns>
        public IActionResult Create(int? id)
        {
            ViewData["id"] = id;
            return View();
        }

        // POST: DhmzAcc/Create
        /// <summary>
        /// Postupak za kreiranje novog Dhmz računa
        /// </summary>
        /// <param name="racun">Uneseni podaci za novog korisnika</param>
        /// <param name="id">Id korisnika kojemu će se pridjeliti novi Dhmz račun</param>
        /// <returns>Pogled na stvoreni račun ili formu za ponovni unos podataka računa s ispisom razloga zbog čega isti nije uspješno kreiran</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Lozinka,KorisnickoIme,DozvolaServer")] DhmzAcc racun,int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Add(racun);
                    await context.SaveChangesAsync();

                    //spojiti korisnika s DHMZ računom
                    var korisnik = context.Korisnik.Where(k => k.Id.Equals(id)).First();


                    korisnik.DhmzId = racun.Id;
                    context.Update(korisnik);
                    context.Update(racun);

                    await context.SaveChangesAsync();

                    TempData["Success"] = $"Račun uspješno dodan.";
                    return RedirectToAction("Details", new { id = racun.Id });
                }
            }catch(Exception e)
            {
                TempData["Error"] = $"Korisnički račun već napravljen! Stisnite 'Povratak'!";
                return View(racun);
            }
            

            return View(racun);

        }

        //Za samostalno dodavanje
        // GET: DhmzAcc/Create2
        public IActionResult Create2()
        {
            var listaKorisnika = context.Korisnik.Where(k => k.DhmzId == null && !k.KorisnickoIme.Equals("sadrzaj"));
            var lista = new SelectList(listaKorisnika,"Id", "KorisnickoIme");

            if (!listaKorisnika.Any())
            {
                return View("Error");

            }

            ViewData["Id"] = lista;
            return View();
        }

        // POST: DhmzAcc/Create2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create2([Bind("Lozinka,KorisnickoIme,DozvolaServer")] DhmzAcc racun, int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Id == 0)
                    {
                        var listaKorisnika = context.Korisnik.Where(k => k.DhmzId == null && !k.KorisnickoIme.Equals("sadrzaj"));
                        var lista = new SelectList(listaKorisnika, "Id", "KorisnickoIme");

                        ViewData["Id"] = lista;

                        return View(racun);
                    }
                    context.Add(racun);
                    await context.SaveChangesAsync();

                    //spojiti korisnika s DHMZ računom
                    var korisnik = context.Korisnik.Where(k => k.Id.Equals(Id)).First();


                    korisnik.DhmzId = racun.Id;
                    context.Update(korisnik);
                    context.Update(racun);

                    await context.SaveChangesAsync();

                    TempData["Success"] = $"Račun uspješno dodan.";
                    return RedirectToAction("Details", new { id = racun.Id });
                }
            }catch(Exception e)
            {
                TempData["Error"] = $"Korisnički račun već napravljen! Stisnite 'Povratak'!";
                return View(racun);

            }
            return View(racun);

        }

        // GET:DhmzAcc/Delete/?id
        /// <summary>
        /// Postupak za brisanje pojedinog Dhmz računa
        /// </summary>
        /// <param name="id">Id računa koji se želi obrisati</param>
        /// <returns>Detaljni pogled na račun koji se želi obrisati ili status o nepostojećem računu za navedeni id</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var racun = await context.DhmzAcc.SingleOrDefaultAsync(m => m.Id == id);
            if (racun == null)
            {
                return NotFound();            }
            var korisnik = context.Korisnik.Where(k => k.DhmzId.Equals(id)).First();

            DhmzAccDetailsViewModel view = new DhmzAccDetailsViewModel(racun, korisnik);
            

            return View(view);
        }

        // POST: DhmzAcc/Delete/?id
        /// <summary>
        /// Postupak za brisanje pojedinog Dhmz računa
        /// </summary>
        /// <param name="id">Id računa koji se želi obrisati</param>
        /// <returns>Status o uspješnosti brisanja računa</returns>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                var racun = await context.DhmzAcc.SingleOrDefaultAsync(m => m.Id == id);

                var korisnik = context.Korisnik.Where(k => k.DhmzId.Equals(racun.Id)).First();
                korisnik.DhmzId = null;
                context.Korisnik.Update(korisnik);
                await context.SaveChangesAsync();

                context.DhmzAcc.Remove(racun);
                await context.SaveChangesAsync();

                TempData["Success"] = $"Račun uspješno obrisan.";

                return RedirectToAction("Index", "DhmzAcc");
            }catch(Exception e)
            {
                return RedirectToAction("Index", "DhmzAcc");
            }
            
        }

        // GET: DhmbAcc/Edit/?id
        /// <summary>
        /// Postupak za ažuriranje pojedinog Dhmz računa
        /// </summary>
        /// <param name="id">Id računa kojeg je potrebno ažurirati</param>
        /// <returns>Forma za ažuriranje računa ili status da račun nije pronađen za navedeni id</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var racun = await context.DhmzAcc.SingleOrDefaultAsync(m => m.Id == id);
            if (racun == null)
            {
                return NotFound();
            }
            return View(racun);
        }

        // POST: DhmzAcc/Edit/?id
        /// <summary>
        /// Postupak za ažuriranje pojedinog Dhmz računa
        /// </summary>
        /// <param name="id">Id računa kojeg je potrebno ažurirati</param>
        /// <param name="racun">Uneseni podaci koje je potrebno ažurirati</param>
        /// <returns>Detaljni pogled na ažurirani račun ili forma za ponovni unos podataka s navedenim razlozima neuspješnosti prošlog unosa</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Lozinka,KorisnickoIme,DozvolaServer")] DhmzAcc racun)
        {
          
            if (id != racun.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(racun);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RacunExists(racun.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }
                TempData["Success"] = $"Račun uspješno uređen.";
                return RedirectToAction("Details", new { id = racun.Id });
            }
            return View(racun);
        }

        /// <summary>
        /// Metoda koja provjerava da li račun postoji za navedeni "id"
        /// </summary>
        /// <param name="id">Id računa</param>
        /// <returns>Istinitosna vrijednost ukoliko postoji, suprotno ukoliko ne postoji</returns>
        private bool RacunExists(int id)
        {
            return context.DhmzAcc.Any(k => k.Id.Equals(id));
        }

        public bool racunPostoji(string KorisnickoIme, int Id)
        {

            if (context.DhmzAcc.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).Any())
            {
                var racun = context.DhmzAcc.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).First();
                if (racun.Id != Id)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Metoda koja vraća detaljni pogled sljedećeg Dhmz raćuna koji su sortirani po id-u
        /// </summary>
        /// <param name="id">Id trenutno prikazanog računa</param>
        /// <returns>Detaljni pogled sljedećeg računa</returns>
        public IActionResult Next(int id)
        {
            var racuni = context.DhmzAcc.ToList();
            bool found = false;
            int index = 0;
            for (int i = 0; i < racuni.Count; i = (i + 1) % racuni.Count)
            {
                if (found)
                {
                    index = racuni[i].Id;
                    break;
                }

                if (racuni[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        /// <summary>
        /// Metoda koja vraća detaljni pogled prethodnog Dhmz raćuna koji su sortirani po id-u
        /// </summary>
        /// <param name="id">Id trenutno prikazanog računa</param>
        /// <returns>Detaljni pogled prethodnog računa</returns>
        public IActionResult Previous(int id)
        {
            var racuni = context.DhmzAcc.ToList();
            bool found = false;
            int index = 0;
            for (int i = racuni.Count - 1; i > -1; i--)
            {
                if (found)
                {
                    index = racuni[i].Id;
                    break;
                }

                if (racuni[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = racuni.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }
    }
}
