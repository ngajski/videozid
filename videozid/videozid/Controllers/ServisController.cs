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
    public class ServisController : Controller
    {
        private readonly RPPP15Context _context;
        private readonly AppSettings appData;

        public ServisController(RPPP15Context context)
        {
            _context = context;
            this.appData = new AppSettings();
        }

        public IActionResult Index(string filter,int page = 1, int sort = 1, bool ascending = true)
        {

            int pagesize = appData.PageSize;

            var query = _context.Servis.AsNoTracking().FromSql(Constants.SqlViewServisi);

            ServisFilter sFilter = new ServisFilter();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                sFilter = ServisFilter.FromString(filter);
                if (!sFilter.IsEmpty())
                {
                    query = sFilter.Apply(query);
                }
            }


            int count = query.Count();

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };

            if (page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = pagingInfo.TotalPages, sort = sort, ascending = ascending });
            }

            query = ApplySort(sort, ascending, query);

            var servisi = ServisiToList(page, query, pagesize);

            List<ServisDetailsViewModel> modeliServisa = new List<ServisDetailsViewModel>();
            foreach (var servis in servisi)
            {
                var tipServis = _context.TipServisa.Where(tip => tip.IdServis == servis.Id).FirstOrDefault();
                var serviseri = _context.Serviser.Where(serviser => serviser.IdServis == servis.Id).ToList();
                modeliServisa.Add(new ServisDetailsViewModel(servis, serviseri, null, tipServis));
            }

            for (int i = 0; i < servisi.Count; i++)
            {
                servisi[i].Position = (page - 1) * pagesize + i;
            }

            ServisiDetailsModel model = new ServisiDetailsModel(modeliServisa, pagingInfo);

            return View(model);
        }

        // GET: Servis/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var servis = _context.Servis.Where(s => s.Id == id).FirstOrDefault();
            if (servis == null)
            {
                TempData[Constants.Error] = "Servis sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            var tipServis = _context.TipServisa.Where(tip => tip.IdServis == id).FirstOrDefault();
            var serviseri = _context.Serviser.Where(serviser => serviser.IdServis == id).ToList();
            var uredajIdNaziv = (from ur in _context.Uredaj
                                 join servisira in _context.Servisira on ur.Id equals servisira.IdUredaj
                                 where (servisira.IdServis == id)
                                 select new
                                 {
                                     id = ur.Id,
                                     naziv = ur.Naziv
                                 }).ToList();

            List<Uredaj> uredaji = new List<Uredaj>();
            for (int i = 0; i < uredajIdNaziv.Count; i++)
            {
                Uredaj uredaj = new Uredaj();
                uredaj.Id = uredajIdNaziv[i].id;
                uredaj.Naziv = uredajIdNaziv[i].naziv;
                uredaji.Add(uredaj);
            }

            ServisDetailsViewModel modelView = new ServisDetailsViewModel(servis, serviseri, uredaji, tipServis);
            return View(modelView);
        }

        // GET: Servis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,ZiroRacun,Opis")] Servis servis, [Bind("TipServisa")] String tipServisa)
        {
            if (!CheckIBAN(servis.ZiroRacun))
            {
                TempData[Constants.Error] = "Broj žiro računa mora biti u formatu HR*******************";
                return View(servis);

            } else if (IBANExists(servis.ZiroRacun))
            {
                TempData[Constants.Error] = "Broj žiro računa " + servis.ZiroRacun + "se koristi.";
                return View(servis);
            }

            if (ModelState.IsValid)
            {

                _context.Add(servis);
                _context.SaveChanges();

                if (tipServisa != null)
                {
                    TipServisa tip = new TipServisa();
                    tip.IdServis = servis.Id;
                    tip.Tip = tipServisa;
                    _context.Add(tip);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Servis \"" + servis.Ime + "\" uspješno dodan.";
                return RedirectToAction("Index");
            }

            return View(servis);
        }

        // GET: Servis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var servis = await _context.Servis.SingleOrDefaultAsync(m => m.Id == id);
            if (servis == null)
            {
                TempData[Constants.Error] = "Servis sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }
            return View(servis);
        }

        // POST: Servis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,ZiroRacun,Opis")] Servis servis)
        {
            if (id != servis.Id)
            {
                TempData[Constants.Error] = "ID-evi se ne poklapaju: " + id + "!=" + servis.Id;
                return RedirectToAction("Index");
            }

            if (!CheckIBAN(servis.ZiroRacun))
            {
                TempData[Constants.Error] = "Broj žiro računa mora biti u formatu HR*******************";
            }
            else if (ModelState.IsValid)
            {

                try
                {
                    _context.Update(servis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServisExists(servis.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(servis);
        }

        // GET: Servis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData[Constants.Error] = "Vrijednost ID ne može biti null.";
                return RedirectToAction("Index");
            }

            var servis = await _context.Servis
                .SingleOrDefaultAsync(m => m.Id == id);
            if (servis == null)
            {
                TempData[Constants.Error] = "Servis sa id=" + id + " ne postoji.";
                return RedirectToAction("Index");
            }

            return View(servis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var servis = await _context.Servis
                             .AsNoTracking()
                             .Where(d => d.Id == id)
                             .SingleOrDefaultAsync();
            if (servis != null)
            {

                foreach (var servisira in getServisiraForServis(servis.Id))
                {
                    _context.Remove(servisira);
                }

                try
                {
                    _context.Remove(servis);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Success] = $"Servis {servis.Ime} uspješno obrisan.";
                }
                catch (Exception exc)
                {
                    TempData[Constants.Error] = $"Pogreška prilikom brisanja servisa {servis.Ime}.";
                }
            }
            else
            {
                TempData[Constants.Error] = "Ne postoji servis s id-om: " + id;
            }
            return RedirectToAction(nameof(Index), new {page = page, sort = sort, ascending = ascending });
        }

        private bool ServisExists(int id)
        {
            return _context.Servis.Any(e => e.Id == id);
        }

        public void DeleteAsync(int? id)
        {

            removeServisiraForServis(id);

            Servis servis = _context.Servis.Find(id);

            if (servis != null)
            {
                _context.Servis.Remove(servis);
                _context.SaveChanges();
            }
        }

        public IActionResult NextServis(int id)
        {
            var servisi = _context.Servis.ToList();

            foreach (var servis in servisi)
            {
                if (servis.Id > id)
                {
                    return RedirectToAction("Details", new { id = servis.Id });
                }
            }

            return RedirectToAction("Details", new { id = servisi[0].Id });
        }

        public IActionResult PreviousServis(int id)
        {
            var servisi = _context.Servis.ToList();

            for (int i = 0; i < servisi.Count; i++)
            {
                if (servisi[i].Id == id && i > 0)
                {
                    return RedirectToAction("Details", new { id = servisi[i-1].Id });
                }
            }

            return RedirectToAction("Details", new { id = servisi[servisi.Count-1].Id });
        }

        [HttpPost]
        public IActionResult Filter(ServisFilter filter)
        {
            return RedirectToAction(nameof(Index), new { filter = filter.ToString() });
        }

        // GET: Uredaj
        public IActionResult IndexAPI()
        {
            return View("IndexAPI");
        }

        public IActionResult EditAPI(int id)
        {
            return View("EditAPI",id);
        }

        public IActionResult DetailsAPI(int id)
        {
            return View("DetailsAPI", id);
        }

        private Boolean CheckIBAN(string iban)
        {
            if (iban.Length != 21)
            {
                return false;
            } else if (!iban.ToUpper().StartsWith("HR")) {
                return false;
            }
            return true;
        }

        private void removeServisiraForServis(int? id)
        {
            var servisiraServis = _context.Servisira.Where(s => s.IdServis == id).ToList();
            foreach (var servisira in servisiraServis)
            {
                _context.Servisira.Remove(servisira);
            }
        }

        private static IQueryable<Servis> ApplySort(int sort, bool ascending, IQueryable<Servis> query)
        {
            System.Linq.Expressions.Expression<Func<Servis, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = d => d.Id;
                    break;
                case 2:
                    orderSelector = d => d.Ime;
                    break;
                case 3:
                    orderSelector = d => d.ZiroRacun;
                    break;
                case 4:
                    orderSelector = d => d.Opis;
                    break;
                case 5:
                    orderSelector = d => d.TipServisa;
                    break;
            }
            if (orderSelector != null)
            {
                query = ascending ?
                       query.OrderBy(orderSelector) :
                       query.OrderByDescending(orderSelector);
            }

            return query;
        }

        private List<Servis> ServisiToList(int page, IQueryable<Servis> query, int pagesize)
        {
            if (page > 0)
            {
                return query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToList();
            }
            else
            {
                return query
                         .Skip((page) * pagesize)
                         .Take(pagesize)
                         .ToList();
            }
        }

        private bool IBANExists(string ziroRacun)
        {
            var servis = _context.Servis.Where(s => s.ZiroRacun == ziroRacun).FirstOrDefault();
            if (servis == null)
            {
                return false;
            }
            return true;
        }

        private List<Servisira> getServisiraForServis(int id)
        {
            return _context.Servisira.Where(s => s.IdServis == id).ToList();
        }
    }
}
