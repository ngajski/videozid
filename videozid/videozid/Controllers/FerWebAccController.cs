using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using Microsoft.EntityFrameworkCore;
using videozid.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace videozid.Controllers
{
    public class FerWebAccController : Controller
    {
        private readonly RPPP15Context context;
       
        public FerWebAccController(RPPP15Context context)
        {
            this.context = context;
        }

        public IActionResult ApiIndex()
        {
            return View("Api/Index");
        }

        public IActionResult ApiDetails(int id)
        {

            FerWebAcc racun = context.FerWebAcc.Where(r => r.Id.Equals(id)).First();
            string korisnickoIme = racun.KorisnickoIme;

            ViewBag.id = id;

            return View("Api/Details", korisnickoIme);
        }

        public IActionResult ApiEdit(int id)
        {
            FerWebAcc racun = context.FerWebAcc.Where(r => r.Id.Equals(id)).First();
            string korisnickoIme = racun.KorisnickoIme;

            ViewBag.id = id;

            return View("Api/Edit", korisnickoIme);
        }

        //Index 
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = context.FerWebAcc.Include(fk => fk.Korisnik);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Details/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var racun = await context.FerWebAcc
                .SingleOrDefaultAsync(m => m.Id == id);
            if (racun == null)
            {
                return NotFound();
            }
            var korisnik = context.Korisnik.Where(k => k.FerId.Equals(id)).First();

            FerWebAccDetailsViewModel view = new FerWebAccDetailsViewModel(racun, korisnik);

            return View(view);
        }

        // GET:FerWebAcc/Create
        public IActionResult Create(int? id)
        {    
            ViewData["id"] = id;
            return View();
        }

        // POST: FerWebAcc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Lozinka,KorisnickoIme,DozvolaServer")] FerWebAcc racun,int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Add(racun);
                    await context.SaveChangesAsync();

                    //spojiti korisnika s ferWeb računom
                    var korisnik = context.Korisnik.Where(k => k.Id.Equals(id)).First();


                    korisnik.FerId = racun.Id;
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
            var listaKorisnika = context.Korisnik.Where(k => k.FerId == null && !k.KorisnickoIme.Equals("sadrzaj"));
            var lista = new SelectList(listaKorisnika, "Id", "KorisnickoIme");

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
        public async Task<IActionResult> Create2([Bind("Lozinka,KorisnickoIme,DozvolaServer")] FerWebAcc racun, int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Id == 0)
                    {
                        var listaKorisnika = context.Korisnik.Where(k => k.FerId == null && !k.KorisnickoIme.Equals("sadrzaj"));
                        var lista = new SelectList(listaKorisnika, "Id", "KorisnickoIme");

                        ViewData["Id"] = lista;

                        return View(racun);
                    }
                    context.Add(racun);
                    await context.SaveChangesAsync();

                    //spojiti korisnika s FerWeb računom
                    var korisnik = context.Korisnik.Where(k => k.Id.Equals(Id)).First();


                    korisnik.FerId = racun.Id;
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

        // GET:FerWebAcc/Delete/?id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var racun = await context.FerWebAcc.SingleOrDefaultAsync(m => m.Id == id);
            if (racun == null)
            {
                return NotFound();
            }

            var korisnik = context.Korisnik.Where(k => k.FerId.Equals(id)).First();
            FerWebAccDetailsViewModel view = new FerWebAccDetailsViewModel(racun, korisnik);

            return View(view);
        }

        // POST: FerWebAcc/Delete/?id
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                var racun = await context.FerWebAcc.SingleOrDefaultAsync(m => m.Id == id);

                var korisnik = context.Korisnik.Where(k => k.FerId.Equals(racun.Id)).First();
                korisnik.FerId = null;
                context.Korisnik.Update(korisnik);
                await context.SaveChangesAsync();

                context.FerWebAcc.Remove(racun);
                await context.SaveChangesAsync();

                TempData["Success"] = $"Račun uspješno obrisan.";

                return RedirectToAction("Index", "FerWebAcc");
            }catch(Exception e)
            {
                return RedirectToAction("Index", "FerWebAcc");
            }
            
        }
        // GET: FerWebAcc/Edit/?id
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var racun = await context.FerWebAcc.SingleOrDefaultAsync(m => m.Id == id);
            if (racun == null)
            {
                return NotFound();
            }
            return View(racun);
        }

        // POST: FerWebAcc/Edit/?id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Lozinka,KorisnickoIme,DozvolaServer")] FerWebAcc racun)
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
        private bool RacunExists(int id)
        {
            return context.FerWebAcc.Any(k => k.Id.Equals(id));
        }

        public bool racunPostoji(string KorisnickoIme, int Id)
        {

            if (context.FerWebAcc.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).Any())
            {
                var racun = context.FerWebAcc.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).First();
                if (racun.Id != Id)
                {
                    return false;
                }
            }
            return true;
        }
        public IActionResult Next(int id)
        {
            var racuni = context.FerWebAcc.ToList();
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

        public IActionResult Previous(int id)
        {
            var racuni = context.FerWebAcc.ToList();
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
