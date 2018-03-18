using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using Microsoft.EntityFrameworkCore;
using videozid.ViewModels;
using System.Diagnostics;
using videozid.Extensions;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace videozid.Controllers
{
    
    public class KorisnikController : Controller
    {
        private readonly RPPP15Context context;

        public KorisnikController(RPPP15Context context)
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

            Korisnik korisnik = context.Korisnik.Where(k => k.Id.Equals(id)).First();
            string korisnickoIme = korisnik.KorisnickoIme;

            ViewBag.id = id;

            return View("Api/Details", korisnickoIme);
        }

        public IActionResult ApiEdit(int id)
        {
            Korisnik korisnik = context.Korisnik.Where(k => k.Id.Equals(id)).First();
            string korisnickoIme = korisnik.KorisnickoIme;

            ViewBag.id = id;

            return View("Api/Edit", korisnickoIme);
        }

        //Index 
        public async Task<IActionResult> Index()
        {
            var rPPP15Context = context.Korisnik.Include(fk => fk.Dhmz).Include(fk => fk.Fer).Include(fk => fk.Administrator);
            ViewData["autor"] = context.Sadrzaj.Select(s => s.Ime).ToList();

            return View(await rPPP15Context.ToListAsync());
        }

        //Index2
        public async Task<IActionResult> ObicniKorisnici()
        {
            var rPPP15Context = context.Korisnik.Include(fk => fk.Dhmz).Include(fk => fk.Fer).Include(fk => fk.Administrator);
            return View(await rPPP15Context.ToListAsync());
        }

        // GET: Details/?id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var user = await context.Korisnik.Include(fk => fk.Dhmz).Include(fk => fk.Fer).Include(fk => fk.Administrator)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var sadrzaji = context.Sadrzaj.Where(s => s.IdAutora.Equals(user.Id)).ToList();
            var odobrio = context.Sadrzaj.Where(s => s.IdOdobrenOd.Equals(user.Id)).ToList();

            KorisnikDetailsViewModel view = new KorisnikDetailsViewModel(user,sadrzaji,odobrio);

            return View(view);
        }
        // GET: Korisnik/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Korisnik/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,Prezime,Email,Lozinka,KorisnickoIme")] Korisnik korisnik, bool checkAdmin = false)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Add(korisnik);

                    if (checkAdmin == true)
                    {
                        Administrator admin = new Administrator();
                        admin.IdKorisnika = korisnik.Id;
                        context.Add(admin);
                    }
                    

                    await context.SaveChangesAsync();
                    TempData["Success"] = $"Korisnik {korisnik.KorisnickoIme} uspješno dodan.";
                    return RedirectToAction("Details", new { id = korisnik.Id });
                }
            }
            catch (Exception e)
            {
                String poruka = "";
                var korisnicii = context.Korisnik.ToList();
                foreach(var kor in korisnicii)
                {
                    if (kor.Email.Equals(korisnik.Email))
                    {
                        poruka += " Navedeni e-mail već postoji!";
                    }
                    if (kor.KorisnickoIme.Equals(korisnik.KorisnickoIme))
                    {
                        poruka += " Navedeno korisničko ime već postoji!";
                    }
                }
                TempData["Error"] = poruka;
                return View(korisnik);


            }
            return View();
        }

            // GET: Korisnik/Delete/?id
            public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await context.Korisnik.Include(fk => fk.Dhmz).Include(fk => fk.Fer).Include(fk => fk.Administrator)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var sadrzaji = context.Sadrzaj.Where(s => s.IdAutora.Equals(user.Id)).ToList();
            var odobrio = context.Sadrzaj.Where(s => s.IdOdobrenOd.Equals(user.Id)).ToList();

            KorisnikDetailsViewModel view = new KorisnikDetailsViewModel(user, sadrzaji, odobrio);

            return View(view);
        }

        // POST: Korisnik/Delete/?id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var korisnik = await context.Korisnik.SingleOrDefaultAsync(m => m.Id == id);
                var sadrzaji = context.Sadrzaj.Where(s => s.IdAutora.Equals(korisnik.Id)).ToList();
                var odobrio = context.Sadrzaj.Where(s => s.IdOdobrenOd.Equals(korisnik.Id)).ToList();
                var defaultniKorisnik = context.Korisnik.Where(k => k.KorisnickoIme.Equals("sadrzaj")).ToList().First();
                //dodijeli default korisniku sadrzaje
                if (sadrzaji.Count != 0)
                {
                    foreach(var sadrzaj in sadrzaji)
                    {
                        sadrzaj.IdAutora = defaultniKorisnik.Id;
                        context.Update(sadrzaj);
                    }
                }
                if (odobrio.Count != 0)
                {
                    foreach (var sadrzaj in odobrio)
                    {
                        sadrzaj.IdOdobrenOd = defaultniKorisnik.Id;
                        context.Update(sadrzaj);
                    }
                }

                //brisi ga iz admin tablice ako ga ima
                if (context.Administrator.Where(a => a.IdKorisnika.Equals(korisnik.Id)).Any())
                {
                    var admin = context.Administrator.Where(a => a.IdKorisnika.Equals(korisnik.Id)).First();
                    context.Administrator.Remove(admin);
                    await context.SaveChangesAsync();
                }

                //brisi ferWeb racun ako ima          
                if (context.FerWebAcc.Where(f => f.Id.Equals(korisnik.FerId)).Any())
                {
                    var ferRacun = context.FerWebAcc.Where(f => f.Id.Equals(korisnik.FerId)).First();
                    context.FerWebAcc.Remove(ferRacun);
                    await context.SaveChangesAsync();
                }

                //brisi DHMZ racun ako ima          
                if (context.DhmzAcc.Where(f => f.Id.Equals(korisnik.DhmzId)).Any())
                {
                    var dhmzRacun = context.DhmzAcc.Where(f => f.Id.Equals(korisnik.DhmzId)).First();
                    context.DhmzAcc.Remove(dhmzRacun);
                    await context.SaveChangesAsync();
                }


                context.Korisnik.Remove(korisnik);
                await context.SaveChangesAsync();
                TempData["Success"] = $"Korisnik {korisnik.KorisnickoIme} uspješno izbrisan.";
                
            }
            catch(Exception e)

            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        // GET: Korisnik/Edit/?id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await context.Korisnik.Include(a => a.Administrator).Include(a=> a.Fer).Include(a=>a.Dhmz).SingleOrDefaultAsync(m => m.Id == id);
            if (korisnik == null)
            {
                return NotFound();
            }        
            return View(korisnik);
        }

        // POST: Korisnik/Edit/?id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,Prezime,Email,Lozinka,KorisnickoIme,DhmzId,FerId")] Korisnik korisnik, bool checkAdmin = false)
        {
            if (id != korisnik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    bool adminJe = context.Administrator.Where(a => a.IdKorisnika.Equals(korisnik.Id)).Any();

                    //za administriranje
                    if ((checkAdmin==true) && !adminJe)
                    {                       
                        Administrator admin = new Administrator();
                        admin.IdKorisnika = korisnik.Id;
                        context.Add(admin);
                    }
                    if ((checkAdmin==false) && adminJe)
                    {
                        var admin = context.Administrator.Where(a => a.IdKorisnika.Equals(korisnik.Id)).First();
                        context.Administrator.Remove(admin);
                        await context.SaveChangesAsync();
                    }
                    context.Update(korisnik);

                    await context.SaveChangesAsync();
                    TempData["Success"] = $"Korisnik {korisnik.KorisnickoIme} uspješno uređen.";
                }
                catch(Exception e)
                  {
                    String poruka = "";
                    var korisnicii = context.Korisnik.ToList();
                    foreach (var kor in korisnicii)
                    {
                        if (kor.Email.Equals(korisnik.Email) && kor.Id != korisnik.Id)
                        {
                            poruka += " Navedeni e-mail već postoji!";
                        }
                        if (kor.KorisnickoIme.Equals(korisnik.KorisnickoIme) && kor.Id != korisnik.Id)
                        {
                            poruka += " Navedeno korisničko ime već postoji!";
                        }
                    }
                    TempData["Error"] = poruka;
                    return View(korisnik);


                }
                
                return RedirectToAction("Index");
            }
            return View(korisnik);
        }
        private bool KorisnikExists(int id)
        {
            return context.Korisnik.Any(k => k.Id.Equals(id));
        }

        public bool korisnikPostoji(string KorisnickoIme, int Id )
        {
            
            if (context.Korisnik.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).Any())
            {
                var korisnik = context.Korisnik.Where(k => k.KorisnickoIme.Equals(KorisnickoIme)).First();
                if(korisnik.Id != Id)
                {
                    return false;
                }
            }
            return true;
        }
        public bool emailPostoji(string Email, int Id)
        {
            if (context.Korisnik.Where(k => k.Email.Equals(Email)).Any())
            {
                var korisnik = context.Korisnik.Where(k => k.Email.Equals(Email)).First();
                if (korisnik.Id != Id)
                {
                    return false;
                }
            }
            return true;
        }
        public IActionResult Next(int id)
        {
            var korisnici = context.Korisnik.ToList();
            bool found = false;
            int index = 0;
            for (int i = 0; i < korisnici.Count; i = (i + 1) % korisnici.Count)
            {
                if (found)
                {
                    index =korisnici[i].Id;
                    break;
                }

                if (korisnici[i].Id == id)
                {
                    found = true;
                }
            }

            return RedirectToAction("Details", new { id = index });
        }

        public IActionResult Previous(int id)
        {
            var korisnici = context.Korisnik.ToList();
            bool found = false;
            int index = 0;
            for (int i = korisnici.Count - 1; i > -1; i--)
            {
                if (found)
                {
                    index = korisnici[i].Id;
                    break;
                }

                if (korisnici[i].Id == id)
                {
                    found = true;
                }

                if (i == 0)
                    i = korisnici.Count;
            }

            return RedirectToAction("Details", new { id = index });
        }
    }
}
