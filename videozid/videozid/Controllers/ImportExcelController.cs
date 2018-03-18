using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using videozid.Models;
using videozid.ViewModels;
using videozid.ViewModels.Api;

namespace videozid.Controllers
{
    public class ImportExcelController : Controller
    {
        private IHostingEnvironment _environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly RPPP15Context context;

        public ImportExcelController(RPPP15Context context, IHostingEnvironment environment)
        {
            this.context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexDhmz()
        {
            return View();
        }

        public IActionResult IndexKorisnici()
        {
            return View();
        }

        public IActionResult IndexUredaji()
        {
            return View();
        }

        public IActionResult IndexVideozidovi()
        {
            return View();
        }

        public IActionResult IndexPrezentacije()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            List<ImportExcelViewModel> racuni = new List<ImportExcelViewModel>();
         
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    FileInfo fileExcel = new FileInfo(Path.Combine(uploads, file.FileName));

                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(fileExcel))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                ImportExcelViewModel racun = new ImportExcelViewModel();
                                string ime, lozinka, vlasnik;
                                int? dozvola;
                                if(worksheet.Cells[row, 1].Value != null)
                                {
                                     ime = worksheet.Cells[row, 1].Value.ToString();
                                    racun.KorisnickoIme = ime;
                                }
                                else
                                {
                                    ime = null;
                                    racun.KorisnickoIme = null;
                                }
                                if (worksheet.Cells[row, 2].Value != null)
                                {
                                    lozinka = worksheet.Cells[row, 2].Value.ToString();
                                    racun.Lozinka = lozinka;
                                }
                                else
                                {
                                    lozinka = null;
                                    racun.Lozinka = null;
                                }
                                if (worksheet.Cells[row, 3].Value != null)
                                {
                                    dozvola = Int32.Parse(worksheet.Cells[row, 3].Value.ToString());
                                    racun.DozvolaServer = dozvola;
                                }
                                else
                                {
                                    racun.DozvolaServer = null;
                                    dozvola = null;
                                }
                                if (worksheet.Cells[row, 4].Value != null)
                                {
                                    vlasnik = worksheet.Cells[row, 4].Value.ToString();
                                    racun.Vlasnik = vlasnik;
                                }
                                else
                                {
                                    vlasnik = null;
                                    racun.Vlasnik = null;
                                }

                                FerWebAcc racunFer = null;
                                var prosloKorisnika = false;
                                try
                                {
                                    Korisnik vlasnikRacuna = null;
                                    vlasnikRacuna = await context.Korisnik.AsNoTracking().Where(k => k.KorisnickoIme.Equals(vlasnik))
                                                                .FirstOrDefaultAsync();

                                    racunFer = new FerWebAcc
                                    {
                                        DozvolaServer = dozvola,
                                        KorisnickoIme = ime,
                                        Lozinka = lozinka
                                        
                                    };
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);

                                    

                                    if (vlasnikRacuna.FerId != null)
                                    {
                                        //korisnik već ima račun
                                        throw new System.ArgumentException("Ma samo baci iznimku");

                                    }
                                    else
                                    {
                                        prosloKorisnika = true;
                                        context.FerWebAcc.Add(racunFer);
                                        context.SaveChanges();

                                        //spojiti korisnika s FerWeb računom
                                        var korisnik = context.Korisnik.Where(k => k.Id.Equals(vlasnikRacuna.Id)).First();


                                        korisnik.FerId = racunFer.Id;
                                        context.Update(korisnik);
                                        context.Update(racunFer);

                                        context.SaveChanges();

                                        //uspješno spremljen
                                        racun.Status = "DA";

                                    }

                                }
                                catch(Exception e)
                                {
                                    //nije uspješno spremljen, status spremiti u izlazi excel file
                                    racun.Status = "NE";
                                    if (prosloKorisnika)
                                    {
                                        context.FerWebAcc.Remove(racunFer);

                                    }


                                }
                                finally
                                {
                                  
                                        racuni.Add(racun);
                                    
                                    
                                }
                            }
                           
                        }
                    }
                    catch (Exception e)
                    {

                    }

                }

                byte[] content;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Properties.Title = "Status unosa FerWeb računa";
                    excel.Workbook.Properties.Author = "Autor";
                    var worksheet = excel.Workbook.Worksheets.Add("FerWeb računi");

                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Uneseno:";
                    worksheet.Cells[1, 2].Value = "Korisničko ime";
                    worksheet.Cells[1, 3].Value = "Lozinka";
                    worksheet.Cells[1, 4].Value = "Dozvola server";
                    worksheet.Cells[1, 5].Value = "Pripada";

                    for (int i = 0; i < racuni.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = racuni[i].Status;
                        worksheet.Cells[i + 2, 2].Value = racuni[i].KorisnickoIme;
                        worksheet.Cells[i + 2, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[i + 2, 3].Value = racuni[i].Lozinka;
                        worksheet.Cells[i + 2, 4].Value = racuni[i].DozvolaServer;
                        worksheet.Cells[i + 2, 5].Value = racuni[i].Vlasnik;
                    }

                    worksheet.Cells[1, 1, racuni.Count + 1, 4].AutoFitColumns();

                    content = excel.GetAsByteArray();
                }
                return File(content, ExcelContentType, "FerWebAcc.xlsx");
            }
            return View();
        }

        public async Task<IActionResult> PredlozakPrezentacija()
        {
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Predlozak prezentacija";
                excel.Workbook.Properties.Author = "Mislav Brusač";
                var worksheet = excel.Workbook.Worksheets.Add("Predlozak prezentacija");

                //First add the headers
                worksheet.Cells[1, 1].Value = "X koordinata";
                worksheet.Cells[1, 2].Value = "Y koordinata";
                worksheet.Cells[1, 3].Value = "Sirina";
                worksheet.Cells[1, 4].Value = "Visina";
                worksheet.Cells[1, 5].Value = "Sadrzaj";
                worksheet.Cells[1, 6].Value = "Kategorija";

                worksheet.Cells[1, 1, 6 + 1, 7].AutoFitColumns();
                content = excel.GetAsByteArray();
            }

            return File(content, ExcelContentType, "PredlozakPrezentacija.xlsx");
        }

            public async Task<IActionResult> PredlozakFerWebAcc()
        {

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Predložak za unos FerWeb računa";
                excel.Workbook.Properties.Author = "Autor";
                var worksheet = excel.Workbook.Worksheets.Add("FerWeb predložak");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Korisničko ime";
                worksheet.Cells[1, 2].Value = "Lozinka";
                worksheet.Cells[1, 3].Value = "Dozvola server";
                worksheet.Cells[1, 4].Value = "Pripada";

                worksheet.Cells[1, 1, 4 + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "PredlozakFerWebAcc.xlsx");
        }

        public async Task<IActionResult> PredlozakDhmzAcc()
        {

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Predložak za unos Dhmz računa";
                excel.Workbook.Properties.Author = "Autor";
                var worksheet = excel.Workbook.Worksheets.Add("Dhmz predložak");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Korisničko ime";
                worksheet.Cells[1, 2].Value = "Lozinka";
                worksheet.Cells[1, 3].Value = "Dozvola server";
                worksheet.Cells[1, 4].Value = "Pripada";

                worksheet.Cells[1, 1, 4 + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "PredlozakDhmzAcc.xlsx");
        }
        public async Task<IActionResult> TestAcc()
        {

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Test datoteka za unos računa";
                excel.Workbook.Properties.Author = "Autor";
                var worksheet = excel.Workbook.Worksheets.Add("Test");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Korisničko ime";
                worksheet.Cells[1, 2].Value = "Lozinka";
                worksheet.Cells[1, 3].Value = "Dozvola server";
                worksheet.Cells[1, 4].Value = "Pripada";

                //data 
                worksheet.Cells[2, 1].Value = "test555";
                worksheet.Cells[2, 2].Value = null;
                worksheet.Cells[2, 3].Value = null;
                worksheet.Cells[2, 4].Value = "mahi55";

                worksheet.Cells[3, 1].Value = "q2";
                worksheet.Cells[3, 2].Value = "w";
                worksheet.Cells[3, 3].Value = 12;
                worksheet.Cells[3, 4].Value = "mahi55";

                worksheet.Cells[4, 1].Value = "testno232";
                worksheet.Cells[4, 2].Value = "svasta4542";
                worksheet.Cells[4, 3].Value = null;
                worksheet.Cells[4, 4].Value = "mahiNepostojeci";

                worksheet.Cells[5, 1].Value = "test555";
                worksheet.Cells[5, 2].Value = "nesto2343";
                worksheet.Cells[5, 3].Value = null;
                worksheet.Cells[5, 4].Value = "mahi55";

                worksheet.Cells[6, 1].Value = "test555";
                worksheet.Cells[6, 2].Value = null;
                worksheet.Cells[6, 3].Value = null;
                worksheet.Cells[6, 4].Value = "mahi55";

                worksheet.Cells[7, 1].Value = "racun223";
                worksheet.Cells[7, 2].Value = "nepitaj99";
                worksheet.Cells[7, 3].Value = 12;
                worksheet.Cells[7, 4].Value = "medo22";


                worksheet.Cells[1, 1, 32 + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "TestAcc.xlsx");
        }

        public async Task<IActionResult> PredlozakKorisnici()
        {

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Predložak za unos korisnika";
                excel.Workbook.Properties.Author = "Autor";
                var worksheet = excel.Workbook.Worksheets.Add("Korisnici predložak");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Prezime";
                worksheet.Cells[1, 2].Value = "Ime";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Lozinka";
                worksheet.Cells[1, 5].Value = "Korisničko ime";
                worksheet.Cells[1, 6].Value = "Administrator(da/ne)";

                worksheet.Cells[1, 1, 6 + 1, 6].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "PredlozakKorisnici.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> IndexDhmz(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            List<ImportExcelViewModel> racuni = new List<ImportExcelViewModel>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    FileInfo fileExcel = new FileInfo(Path.Combine(uploads, file.FileName));

                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(fileExcel))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                ImportExcelViewModel racun = new ImportExcelViewModel();
                                string ime, lozinka, vlasnik;
                                int? dozvola;
                                if (worksheet.Cells[row, 1].Value != null)
                                {
                                    ime = worksheet.Cells[row, 1].Value.ToString();
                                    racun.KorisnickoIme = ime;
                                }
                                else
                                {
                                    ime = null;
                                    racun.KorisnickoIme = null;
                                }
                                if (worksheet.Cells[row, 2].Value != null)
                                {
                                    lozinka = worksheet.Cells[row, 2].Value.ToString();
                                    racun.Lozinka = lozinka;
                                }
                                else
                                {
                                    lozinka = null;
                                    racun.Lozinka = null;
                                }
                                if (worksheet.Cells[row, 3].Value != null)
                                {
                                    dozvola = Int32.Parse(worksheet.Cells[row, 3].Value.ToString());
                                    racun.DozvolaServer = dozvola;
                                }
                                else
                                {
                                    racun.DozvolaServer = null;
                                    dozvola = null;
                                }
                                if (worksheet.Cells[row, 4].Value != null)
                                {
                                    vlasnik = worksheet.Cells[row, 4].Value.ToString();
                                    racun.Vlasnik = vlasnik;
                                }
                                else
                                {
                                    vlasnik = null;
                                    racun.Vlasnik = null;
                                }

                                DhmzAcc racunDhmz = null;
                                var prosloKorisnika = false;
                                try
                                {
                                    Korisnik vlasnikRacuna = null;
                                    vlasnikRacuna = await context.Korisnik.AsNoTracking().Where(k => k.KorisnickoIme.Equals(vlasnik))
                                                                .FirstOrDefaultAsync();

                                    racunDhmz = new DhmzAcc
                                    {
                                        DozvolaServer = dozvola,
                                        KorisnickoIme = ime,
                                        Lozinka = lozinka

                                    };
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);
                                    Debug.WriteLine(lozinka);



                                    if (vlasnikRacuna.DhmzId != null)
                                    {
                                        //korisnik već ima račun
                                        throw new System.ArgumentException("Ma samo baci iznimku");

                                    }
                                    else
                                    {
                                        prosloKorisnika = true;
                                        context.DhmzAcc.Add(racunDhmz);
                                        context.SaveChanges();

                                        //spojiti korisnika s Dhmz računom
                                        var korisnik = context.Korisnik.Where(k => k.Id.Equals(vlasnikRacuna.Id)).First();


                                        korisnik.DhmzId = racunDhmz.Id;
                                        context.Update(korisnik);
                                        context.Update(racunDhmz);

                                        context.SaveChanges();

                                        //uspješno spremljen
                                        racun.Status = "DA";

                                    }

                                }
                                catch (Exception e)
                                {
                                    //nije uspješno spremljen, status spremiti u izlazi excel file
                                    racun.Status = "NE";
                                    if (prosloKorisnika)
                                    {
                                        context.DhmzAcc.Remove(racunDhmz);

                                    }


                                }
                                finally
                                {

                                    racuni.Add(racun);


                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {

                    }

                }

                byte[] content;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Properties.Title = "Status unosa Dhmz računa";
                    excel.Workbook.Properties.Author = "Autor";
                    var worksheet = excel.Workbook.Worksheets.Add("Dhmz računi");

                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Uneseno:";
                    worksheet.Cells[1, 2].Value = "Korisničko ime";
                    worksheet.Cells[1, 3].Value = "Lozinka";
                    worksheet.Cells[1, 4].Value = "Dozvola server";
                    worksheet.Cells[1, 5].Value = "Pripada";

                    for (int i = 0; i < racuni.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = racuni[i].Status;
                        worksheet.Cells[i + 2, 2].Value = racuni[i].KorisnickoIme;
                        worksheet.Cells[i + 2, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[i + 2, 3].Value = racuni[i].Lozinka;
                        worksheet.Cells[i + 2, 4].Value = racuni[i].DozvolaServer;
                        worksheet.Cells[i + 2, 5].Value = racuni[i].Vlasnik;
                    }

                    worksheet.Cells[1, 1, racuni.Count + 1, 4].AutoFitColumns();

                    content = excel.GetAsByteArray();
                }
                return File(content, ExcelContentType, "DhmzAcc.xlsx");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexKorisnici(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            List<ImportExcelKorisnikViewModel> korisnici = new List<ImportExcelKorisnikViewModel>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    FileInfo fileExcel = new FileInfo(Path.Combine(uploads, file.FileName));

                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(fileExcel))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                ImportExcelKorisnikViewModel korisnik = new ImportExcelKorisnikViewModel();
                                CultureInfo culture = CultureInfo.InvariantCulture;
                                string prezime, ime, email, lozinka, korisnickoIme;
                                bool admin;
                                if (worksheet.Cells[row, 1].Value != null)
                                {
                                    prezime = worksheet.Cells[row, 1].Value.ToString();
                                    korisnik.Prezime = prezime;
                                }
                                else
                                {
                                    prezime = null;
                                    korisnik.Prezime = null;
                                }
                                if (worksheet.Cells[row, 2].Value != null)
                                {
                                    ime = worksheet.Cells[row, 2].Value.ToString();
                                    korisnik.Ime = ime;
                                }
                                else
                                {
                                    ime = null;
                                    korisnik.Ime = null;
                                }
                                if (worksheet.Cells[row, 3].Value != null)
                                {
                                    email = worksheet.Cells[row, 3].Value.ToString();
                                    korisnik.Email = email;
                                }
                                else
                                {
                                    email = null;
                                    korisnik.Email = null;
                                }
                                if (worksheet.Cells[row, 4].Value != null)
                                {
                                    lozinka = worksheet.Cells[row, 4].Value.ToString();
                                    korisnik.Lozinka = lozinka;
                                }
                                else
                                {
                                    lozinka = null;
                                    korisnik.Lozinka = null;
                                }
                                if (worksheet.Cells[row, 5].Value != null)
                                {
                                    korisnickoIme = worksheet.Cells[row, 5].Value.ToString();
                                    korisnik.KorisnickoIme = korisnickoIme;
                                }
                                else
                                {
                                    korisnickoIme = null;
                                    korisnik.KorisnickoIme = null;
                                }
                                if(worksheet.Cells[row, 6].Value != null)
                                {
                                    string temp = worksheet.Cells[row, 6].Value.ToString();
                                    if(culture.CompareInfo.IndexOf(temp, "da", CompareOptions.IgnoreCase) >= 0)
                                    {
                                        korisnik.Admin = true;
                                        admin = true;
                                    }
                                    else
                                    {
                                        korisnik.Admin = false;
                                        admin = false;
                                    }
                                }
                                else
                                {
                                    korisnik.Admin = false;
                                    admin = false;
                                }

                                Korisnik korisnikBaza = null;
                                var prosloKorisnika = false;
                                try
                                {

                                    korisnikBaza = new Korisnik
                                    {
                                        KorisnickoIme = korisnickoIme,
                                        Lozinka = lozinka,
                                        Email = email,
                                        Ime = ime,
                                        Prezime = prezime
                                    };
                                         
                                    
                                    context.Korisnik.Add(korisnikBaza);
                                    context.SaveChanges();

                                    if (admin)
                                    {
                                        Administrator administrator = new Administrator { IdKorisnika = korisnikBaza.Id };

                                        context.Administrator.Add(administrator);
                                        context.SaveChanges();
                                    }

                                    //uspješno spremljen
                                    korisnik.Status = "DA";
                                }
                                catch (Exception e)
                                {
                                    //nije uspješno spremljen, status spremiti u izlazi excel file
                                    korisnik.Status = "NE";
                                    context.Korisnik.Remove(korisnikBaza);
                                }
                                finally
                                {

                                    korisnici.Add(korisnik);

                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {

                    }

                }

                byte[] content;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Properties.Title = "Status unosa korisnika";
                    excel.Workbook.Properties.Author = "Autor";
                    var worksheet = excel.Workbook.Worksheets.Add("Korisnici unos");

                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Uneseno";
                    worksheet.Cells[1, 2].Value = "Prezime";
                    worksheet.Cells[1, 3].Value = "Ime";
                    worksheet.Cells[1, 4].Value = "Email";
                    worksheet.Cells[1, 5].Value = "Lozinka";
                    worksheet.Cells[1, 6].Value = "Korisničko ime";
                    worksheet.Cells[1, 7].Value = "Administrator";


                    for (int i = 0; i < korisnici.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = korisnici[i].Status;
                        worksheet.Cells[i + 2, 2].Value = korisnici[i].Prezime;
                        worksheet.Cells[i + 2, 3].Value = korisnici[i].Ime;
                        worksheet.Cells[i + 2, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[i + 2, 4].Value = korisnici[i].Email;
                        worksheet.Cells[i + 2, 5].Value = korisnici[i].Lozinka;
                        worksheet.Cells[i + 2, 6].Value = korisnici[i].KorisnickoIme;
                        if (korisnici[i].Admin)
                        {
                            worksheet.Cells[i + 2, 7].Value = "Da";
                        }
                        else
                        {
                            worksheet.Cells[i + 2, 7].Value = "Ne";
                        }
                    }

                    worksheet.Cells[1, 1, korisnici.Count + 1, 6].AutoFitColumns();

                    content = excel.GetAsByteArray();
                }
                return File(content, ExcelContentType, "KorisniciUnosStatus.xlsx");
            }
            return View();
        }


        public async Task<IActionResult> PredlozakUredaji()
        {

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Predložak za unos uređaja";
                excel.Workbook.Properties.Author = "Autor";
                var worksheet = excel.Workbook.Worksheets.Add("Uređaji predložak");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Ime uređaja";
                worksheet.Cells[1, 2].Value = "Nabavna cijena";
                worksheet.Cells[1, 3].Value = "Aktualna cijena";
                worksheet.Cells[1, 4].Value = "Datum nabavke";
                worksheet.Cells[1, 5].Value = "Nadređena komponenta";
                worksheet.Cells[1, 6].Value = "Pripadnost videozidu";
                worksheet.Cells[1, 7].Value = "Status (aktivan/zamjenski)";

                worksheet.Cells[1, 1, 7 + 1, 7].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "PredlozakUredaji.xlsx");
        }

        /// <summary>
        /// Precedura koja sprema prezentacije u bazu podataka i vraća excel tablicu u kojoj piše
        /// je li prezentacija uspješno spremljena.
        /// </summary>
        /// <param name="files">Excel tablica prezentacija</param>
        /// <returns>Excel tablica sa statusom je li pojedina prezentacija uspješno spremljena</returns>
        [HttpPost]
        public IActionResult IndexPrezentacije(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            List<PrezentacijaApiModel> prezentacije = new List<PrezentacijaApiModel>();
            List<string> statusi = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    //FileInfo fileExcel = new FileInfo("C:/Users/nikol/Downloads/serviserPredložak.xlsx");
                    FileInfo fileExcel = new FileInfo(Path.Combine(uploads, file.FileName));

                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(fileExcel))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;


                            for (int row = 2; row <= rowCount; row++)
                            {
                                bool flag = false;
                                PrezentacijaApiModel prezentacija = new PrezentacijaApiModel();
                                CultureInfo culture = CultureInfo.InvariantCulture;
                                int xkoord, ykoord, sirina, visina;
                                string sadrzaj, kategorija;


                                if (worksheet.Cells[row, 1].Value != null)
                                {
                                    xkoord = Int32.Parse(worksheet.Cells[row, 1].Value.ToString());
                                    prezentacija.XKoord = xkoord;
                                }
                                else
                                {
                                    flag = true;
                                }
                                if (worksheet.Cells[row, 2].Value != null)
                                {
                                    ykoord = Int32.Parse(worksheet.Cells[row, 2].Value.ToString());
                                    prezentacija.YKoord = ykoord;
                                }
                                else
                                {
                                    flag = true;
                                }
                                if (worksheet.Cells[row, 3].Value != null)
                                {
                                    sirina = Int32.Parse(worksheet.Cells[row, 3].Value.ToString());
                                    prezentacija.Sirina = sirina;
                                }
                                else
                                {
                                    flag = true;
                                }
                                if (worksheet.Cells[row, 4].Value != null)
                                {
                                    visina = Int32.Parse(worksheet.Cells[row, 4].Value.ToString());
                                    prezentacija.Visina = visina;
                                }
                                else
                                {
                                    flag = true;
                                }
                                if (worksheet.Cells[row, 5].Value != null)
                                {
                                    sadrzaj = worksheet.Cells[row, 5].Value.ToString();
                                    prezentacija.Sadrzaj = sadrzaj;
                                }
                                else
                                {
                                    flag = true;
                                }
                                if (worksheet.Cells[row, 6].Value != null)
                                {
                                    kategorija = worksheet.Cells[row, 6].Value.ToString();
                                    prezentacija.Kategorija = kategorija;
                                }
                                else
                                {
                                    flag = true;
                                }
                                Kategorija kategorijaBaza = context.Kategorija.Where(k => k.Vrsta.Equals(prezentacija.Kategorija)).FirstOrDefault();
                                Sadrzaj sadrzajBaza = context.Sadrzaj.Where(s => s.Ime.Equals(prezentacija.Sadrzaj)).FirstOrDefault();


                                if (kategorijaBaza == null)
                                {
                                    statusi.Add("NE (Kategorija ne postoji)");
                                }

                                if (sadrzajBaza == null)
                                {
                                    statusi.Add("NE (Sadržaj ne postoji)");
                                }

                                if (flag == true)
                                {
                                    statusi.Add("NE (Neispravan unos)");
                                }

                                try
                                {
                                    Prezentacija prezentacijaBaza = new Prezentacija
                                    {
                                        XKoord = prezentacija.XKoord,
                                        YKoord = prezentacija.YKoord,
                                        Sirina = prezentacija.Sirina,
                                        Visina = prezentacija.Visina,
                                        IdSadrzaja = sadrzajBaza.Id,
                                        IdKategorije = kategorijaBaza.Id

                                    };
                                    context.Prezentacija.Add(prezentacijaBaza);
                                    context.SaveChanges();
                                    statusi.Add("DA");
                                }
                                catch (Exception e)
                                {
                                    statusi.Add("NE (" + e.Message + ")");
                                }
                                finally
                                {
                                    prezentacije.Add(prezentacija);
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        return NotFound(e.Message);
                    }

                }

                byte[] content;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Properties.Title = "Status unosa prezentacija";
                    excel.Workbook.Properties.Author = "Mislav Brusač";
                    var worksheet = excel.Workbook.Worksheets.Add("Status prezentacija");

                    //First add the headers
                    worksheet.Cells[1, 1].Value = "X koordinata";
                    worksheet.Cells[1, 2].Value = "Y koordinata";
                    worksheet.Cells[1, 3].Value = "Sirina";
                    worksheet.Cells[1, 4].Value = "Visina";
                    worksheet.Cells[1, 5].Value = "Sadrzaj";
                    worksheet.Cells[1, 6].Value = "Kategorija";
                    worksheet.Cells[1, 7].Value = "Status";

                    for (int i = 0; i < prezentacije.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = prezentacije[i].XKoord;
                        worksheet.Cells[i + 2, 2].Value = prezentacije[i].YKoord;
                        worksheet.Cells[i + 2, 3].Value = prezentacije[i].Sirina;
                        worksheet.Cells[i + 2, 4].Value = prezentacije[i].Visina;
                        worksheet.Cells[i + 2, 5].Value = prezentacije[i].Sadrzaj;
                        worksheet.Cells[i + 2, 6].Value = prezentacije[i].Kategorija;
                        worksheet.Cells[i + 2, 7].Value = statusi[i];
                    }

                    worksheet.Cells[1, 1, prezentacije.Count + 1, 7].AutoFitColumns();
                    content = excel.GetAsByteArray();
                }
                return File(content, ExcelContentType, "StatusUnosaPrezentacija.xlsx");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexUredaji(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var uredaji = new List<ImportUredajViewModel>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(file.OpenReadStream()))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Uređaji predložak"];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var uredaj = new ImportUredajViewModel();
                                CultureInfo culture = CultureInfo.InvariantCulture;
                                string naziv, nabavna, aktualna, datum, status;
                                int? nadredena, zid;

                                if (worksheet.Cells[row, 1].Value != null)
                                {
                                    naziv = worksheet.Cells[row, 1].Value.ToString();
                                }
                                else
                                {
                                    naziv = null;
                                }
                                uredaj.Naziv = naziv;

                                if (worksheet.Cells[row, 2].Value != null)
                                {
                                    nabavna = worksheet.Cells[row, 2].Value.ToString();
                                }
                                else
                                {
                                    nabavna = null;
                                }
                                uredaj.NabavnaCijena = nabavna;

                                if (worksheet.Cells[row, 3].Value != null)
                                {
                                    aktualna = worksheet.Cells[row, 3].Value.ToString();

                                }
                                else
                                {
                                    aktualna = null;
                                }
                                uredaj.AktualnaCijena = aktualna;

                                if (worksheet.Cells[row, 4].Value != null)
                                {
                                    datum = worksheet.Cells[row, 4].Value.ToString();
                                }
                                else
                                {
                                    datum = System.DateTime.Now.ToString("dd.MM.yyyy");
                                }
                                uredaj.DatumNabavke = datum;
                                //novo
                                if (worksheet.Cells[row, 5].Value != null)
                                {
                                    var name = worksheet.Cells[row, 5].Value.ToString();
                                    uredaj.NadredenaKomponenta = name;

                                    if(context.Uredaj.Where(u => u.Naziv.Equals(name)).ToList().Count > 0)
                                        nadredena = context.Uredaj.Where(u => u.Naziv.Equals(name)).First().Id;
                                    else nadredena = null;
                                }
                                else
                                {
                                    nadredena = null;
                                    uredaj.NadredenaKomponenta = null;
                                }

                                if (worksheet.Cells[row, 6].Value != null)
                                {
                                     var name = worksheet.Cells[row, 6].Value.ToString();
                                     uredaj.Videozid = name;
                                     if (context.Videozid.Where(z => z.Naziv.Equals(name)).ToList().Count > 0)
                                         zid = context.Videozid.Where(z => z.Naziv.Equals(name)).First().Id;
                                     else zid = null;
                                }
                                else
                                {
                                    zid = null;
                                    uredaj.Videozid = null;
                                }

                                // kraj
                                if (worksheet.Cells[row, 7].Value != null)
                                {
                                    status = worksheet.Cells[row, 7].Value.ToString();
                                    uredaj.StatusUredaja = status;
                                    if (!status.Equals("aktivan") && !status.Equals("zamjenski"))
                                        status = null;
                                }
                                else
                                {
                                    status = null;
                                    uredaj.StatusUredaja = null;
                                }

                                Uredaj uredajBaza = null;
                                var prosloUredaja = false;
                                try
                                {

                                    uredajBaza = new Uredaj
                                    {
                                        Naziv = naziv,
                                        NabavnaCijena = int.Parse(nabavna),
                                        AktualnaCijena = int.Parse(aktualna),
                                        DatumNabavke = DateTime.Parse(datum),
                                        IdStatusa = status.Equals("aktivan") ? 1 : (status.Equals("zamjenski") ? 2 : 0),
                                        IdZida = zid,
                                        IdNadredeneKomponente = nadredena,
                                    };


                                    context.Add(uredajBaza);
                                    context.SaveChanges();

                                    //uspješno spremljen
                                    uredaj.Status = "DA";
                                }
                                catch (Exception e)
                                {
                                    //nije uspješno spremljen, status spremiti u izlazi excel file
                                    uredaj.Status = "NE";
                                    //context.Remove(uredajBaza);
                                }
                                finally
                                {

                                    uredaji.Add(uredaj);

                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {

                    }

                }

                byte[] content;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Properties.Title = "Status unosa uređaja";
                    excel.Workbook.Properties.Author = "Autor";
                    var worksheet = excel.Workbook.Worksheets.Add("uređaji unos");

                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Status";
                    worksheet.Cells[1, 2].Value = "Ime uređaja";
                    worksheet.Cells[1, 3].Value = "Nabavna cijena";
                    worksheet.Cells[1, 4].Value = "Aktualna cijena";
                    worksheet.Cells[1, 5].Value = "Datum nabavke";
                    worksheet.Cells[1, 6].Value = "Nadređena komponenta";
                    worksheet.Cells[1, 7].Value = "Pripadnost videozidu";
                    worksheet.Cells[1, 8].Value = "Status (aktivan/zamjenski)";

                    for (int i = 0; i < uredaji.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = uredaji[i].Status;
                        worksheet.Cells[i + 2, 2].Value = uredaji[i].Naziv;
                        worksheet.Cells[i + 2, 3].Value = uredaji[i].AktualnaCijena;
                        worksheet.Cells[i + 2, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[i + 2, 4].Value = uredaji[i].NabavnaCijena;
                        worksheet.Cells[i + 2, 5].Value = uredaji[i].DatumNabavke;
                        worksheet.Cells[i + 2, 6].Value = uredaji[i].NadredenaKomponenta;
                        worksheet.Cells[i + 2, 7].Value = uredaji[i].Videozid;
                        worksheet.Cells[i + 2, 8].Value = uredaji[i].StatusUredaja;
                    }

                    worksheet.Cells[1, 1, uredaji.Count + 1, 8].AutoFitColumns();

                    content = excel.GetAsByteArray();
                }
                return File(content, ExcelContentType, "UredajiUnosStatus.xlsx");
            }
            return View();
        }


        public async Task<IActionResult> PredlozakVideozidovi()
        {

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Predložak za unos videozida";
                excel.Workbook.Properties.Author = "Autor";
                var worksheet = excel.Workbook.Worksheets.Add("Videozidovi predložak");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Ime zida";
                worksheet.Cells[1, 2].Value = "Lokacija";
                worksheet.Cells[1, 3].Value = "Širina";
                worksheet.Cells[1, 4].Value = "Visina";

                worksheet.Cells[1, 1, 4 + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "PredlozakVideozidovi.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> IndexVideozidovi(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var videozidovi = new List<ImportVideozidoviViewModel>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    FileInfo fileExcel = new FileInfo(Path.Combine(uploads, file.FileName));

                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(file.OpenReadStream()))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Videozidovi predložak"];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var videozid = new ImportVideozidoviViewModel();
                                CultureInfo culture = CultureInfo.InvariantCulture;
                                string naziv, lokacija, sirina, visina;

                                if (worksheet.Cells[row, 1].Value != null)
                                {
                                    naziv = worksheet.Cells[row, 1].Value.ToString();
                                }
                                else
                                {
                                    naziv = null;
                                }
                                videozid.Naziv = naziv;

                                if (worksheet.Cells[row, 2].Value != null)
                                {
                                    lokacija = worksheet.Cells[row, 2].Value.ToString();
                                }
                                else
                                {
                                    lokacija = null;
                                }
                                videozid.Lokacija = lokacija;

                                if (worksheet.Cells[row, 3].Value != null)
                                {
                                    sirina = worksheet.Cells[row, 3].Value.ToString();

                                }
                                else
                                {
                                    sirina = null;
                                }
                                videozid.Sirina = sirina;

                                if (worksheet.Cells[row, 4].Value != null)
                                {
                                    visina = worksheet.Cells[row, 4].Value.ToString();
                                }
                                else
                                {
                                    visina = null;
                                }
                                videozid.Visina = visina;

                                Videozid videozidBaza = null;
                                var prosloZidova = false;
                                try
                                {

                                    videozidBaza = new Videozid
                                    {
                                        Naziv = naziv,
                                        Lokacija = lokacija,
                                        Sirina = int.Parse(sirina),
                                        Visina = int.Parse(visina),
                                    };


                                    context.Add(videozidBaza);
                                    context.SaveChanges();

                                    //uspješno spremljen
                                    videozid.Status = "DA";
                                }
                                catch (Exception e)
                                {
                                    //nije uspješno spremljen, status spremiti u izlazi excel file
                                    videozid.Status = "NE";
                                    //context.Remove(uredajBaza);
                                }
                                finally
                                {

                                    videozidovi.Add(videozid);

                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {

                    }

                }

                byte[] content;
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Properties.Title = "Status unosa videozida";
                    excel.Workbook.Properties.Author = "Autor";
                    var worksheet = excel.Workbook.Worksheets.Add("Videozidovi unos");

                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Status";
                    worksheet.Cells[1, 2].Value = "Ime zida";
                    worksheet.Cells[1, 3].Value = "Lokacija";
                    worksheet.Cells[1, 4].Value = "Širina";
                    worksheet.Cells[1, 5].Value = "Visina";

                    for (int i = 0; i < videozidovi.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = videozidovi[i].Status;
                        worksheet.Cells[i + 2, 2].Value = videozidovi[i].Naziv;
                        worksheet.Cells[i + 2, 3].Value = videozidovi[i].Lokacija;
                        worksheet.Cells[i + 2, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[i + 2, 4].Value = videozidovi[i].Sirina;
                        worksheet.Cells[i + 2, 5].Value = videozidovi[i].Visina;
                    }

                    worksheet.Cells[1, 1, videozidovi.Count + 1, 5].AutoFitColumns();

                    content = excel.GetAsByteArray();
                }
                return File(content, ExcelContentType, "VideozidoviUnosStatus.xlsx");
            }
            return View();
        }
    }
}
