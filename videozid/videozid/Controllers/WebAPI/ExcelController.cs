using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using videozid.ViewModels.Api;
using videozid.ViewModels;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace videozid.Controllers.WebAPI
{
    /// <summary>
    /// Kontroler koji se koristi za izradu excel tablica na temelju sadržaja u bazi podataka.
    /// </summary>
    [Route("api/[controller]")]
    public class ExcelController : Controller
    {
        private IHostingEnvironment _environment;
        private readonly RPPP15Context context;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private static string SHEET_NAME = "Serviseri";

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontekst za pristup bazi podataka</param>
        /// <param name="env">Okruženje </param>
        public ExcelController(RPPP15Context context, IHostingEnvironment env)
        {
            this.context = context;
            this._environment = env;
        }

        /// <summary>
        /// Procedura koja vraća pogled IndexServiseri.
        /// </summary>
        /// <returns>pogled IndexServiseri</returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View("IndexServiseri");
        }

        /// <summary>
        /// Metoda koja kreira excel tablicu svih dostupnih servisa.
        /// </summary>
        /// <returns>Excel dokumenst, xlsx</returns>
        [HttpGet("servisi")]
       // [SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(File), Description = "Vraća excel tablicu sa servisima.")]
        public async Task<IActionResult> ServisiExport()
        {
            var servisi = await context.Servis
                                .Select(d => new ServisApiModel
                                {
                                    Id = d.Id,
                                    Ime = d.Ime,
                                    Opis = d.Opis,
                                    Racun = d.ZiroRacun,
                                    TipServisa = context.TipServisa.Where(s => s.IdServis == d.Id).First().Tip,
                                    Serviseri = context.Serviser.Where(s => s.IdServis == d.Id).ToList()
                                })
                                .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis servisa";
                excel.Workbook.Properties.Author = "Nikola Gajski";
                var worksheet = excel.Workbook.Worksheets.Add("Servisi");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Ime";
                worksheet.Cells[1, 2].Value = "Opis";
                worksheet.Cells[1, 3].Value = "Žiro račun";
                worksheet.Cells[1, 4].Value = "Serviseri: Opis";
                worksheet.Cells[1, 5].Value = "Uređaji: Cijena ";


                for (int i = 0; i < servisi.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = servisi[i].Ime;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = servisi[i].Opis;
                    worksheet.Cells[i + 2, 3].Value = servisi[i].Racun;


                    #region Serviseri u excel
                    string cellString = "Nema zaposlenih";
                    if (servisi[i].Serviseri.Count > 0)
                    {
                        cellString = "";
                        foreach (var worker in servisi[i].Serviseri)
                        {
                            cellString += worker.Ime + " " + worker.Prezime + ": " + worker.Opis + " | ";
                        }
                        cellString = cellString.Substring(0, cellString.Length - 2);
                    }
                    worksheet.Cells[i + 2, 4].Value = cellString;
                    #endregion

                    #region Uređaji u excel
                    var uredajIdNaziv = (from ur in context.Uredaj
                                         join servisira in context.Servisira on ur.Id equals servisira.IdUredaj
                                         where (servisira.IdServis == servisi[i].Id)
                                         select new
                                         {
                                             id = ur.Id,
                                             naziv = ur.Naziv,
                                             cijena = ur.AktualnaCijena
                                         }).ToList();

                    List<Uredaj> uredaji = new List<Uredaj>();
                    for (int j = 0; j < uredajIdNaziv.Count; j++)
                    {
                        Uredaj uredaj = new Uredaj();
                        uredaj.Id = uredajIdNaziv[j].id;
                        uredaj.Naziv = uredajIdNaziv[j].naziv;
                        uredaj.AktualnaCijena = uredajIdNaziv[j].cijena;
                        uredaji.Add(uredaj);
                    }

                    cellString = "Servis ne servisira niti jedan uređaj";

                    if (uredaji.Count != 0)
                    {
                        cellString = "";
                        foreach (var uredaj in uredaji)
                        {
                            cellString += uredaj.Naziv + ": " + uredaj.AktualnaCijena + "kn | ";
                        }
                        cellString = cellString.Substring(0, cellString.Length - 2);
                    }
                    worksheet.Cells[i + 2, 5].Value = cellString;
                }
                #endregion

                worksheet.Cells[1, 1, servisi.Count + 1, 4].AutoFitColumns();
                worksheet.Cells[1, 1, servisi.Count + 1, 5].AutoFitColumns();
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "servisi.xlsx");
        }

        /// <summary>
        /// Metoda koja kreira excel tablicu svih dostupnih serviera.
        /// </summary>
        /// <returns>Excel dokument, xlsx</returns>
        [HttpGet("serviseri")]
        //[SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(File), Description = "Vraća excel tablicu sa svim serviserima.")]
        public async Task<IActionResult> ServiseriExport()
        {
            var serviseri = await context.Serviser
                                .Select(d => new ServiserApiModel
                                {
                                    Id = d.Id,
                                    Ime = d.Ime,
                                    Opis = d.Opis,
                                    Prezime = d.Prezime,
                                    Servis = d.IdServisNavigation.Ime
                                })
                                .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis servisera";
                excel.Workbook.Properties.Author = "Nikola Gajski";
                var worksheet = excel.Workbook.Worksheets.Add(SHEET_NAME);

                //First add the headers
                worksheet.Cells[1, 1].Value = "Ime";
                worksheet.Cells[1, 2].Value = "Prezime";
                worksheet.Cells[1, 3].Value = "Opis";
                worksheet.Cells[1, 4].Value = "Servis";

                for (int i = 0; i < serviseri.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = serviseri[i].Ime;
                    worksheet.Cells[i + 2, 2].Value = serviseri[i].Prezime;
                    worksheet.Cells[i + 2, 3].Value = serviseri[i].Opis;
                    worksheet.Cells[i + 2, 4].Value = serviseri[i].Servis;
                }

                worksheet.Cells[1, 1, serviseri.Count + 1, 4].AutoFitColumns();
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "serviseri.xlsx");
        }

        /// <summary>
        /// Metoda koja kreira excel predložak za dodavanje servisera.
        /// </summary>
        /// <returns>Excel predložak za dodavanje servisera</returns>
        [HttpGet("serviseri/predlozak")]
        //[SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IActionResult), Description = "Vraća excel predložak sa serviserima.")]
        public IActionResult ServiseriPredlozak()
        {
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis servisera";
                excel.Workbook.Properties.Author = "Nikola Gajski";
                var worksheet = excel.Workbook.Worksheets.Add(SHEET_NAME);

                //First add the headers
                worksheet.Cells[1, 1].Value = "Ime";
                worksheet.Cells[1, 2].Value = "Prezime";
                worksheet.Cells[1, 3].Value = "Opis";
                worksheet.Cells[1, 4].Value = "Servis";

                addRowToSheet("Ime10", "Ime10", "Excel 10", "ServisZaRačunala",worksheet,2);
                addRowToSheet("Ime11", "Ime11", "Excel 11", "ServisZaRačunalaa", worksheet, 3);
                addRowToSheet("Ime12", "Ime12", "Excel 12", "ServisZaMonitor", worksheet, 4);
                addRowToSheet("Ime13", "Ime13", "", "ServisZaMonitor", worksheet, 5);
                addRowToSheet("Ime14", "", "", "ServisZaMonitor", worksheet, 6);
                addRowToSheet("", "Prez15", "", "ServisZaMonitor", worksheet, 7);
                addRowToSheet("Ime16", "Prez16", "", "", worksheet, 8);

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "serviserPredložak.xlsx");
        }

        /// <summary>
        /// Precedura koja sprema servisere zapisane u excel tablici u bazu podataka i vreća excel tablicu u kojoj piše
        /// je li serviser uspiješno spremljen.
        /// </summary>
        /// <param name="files">Excel tablica sa serviserima</param>
        /// <returns>Excel tablica sa statusom je li pojedini serviser uspješno spremljen</returns>
        [HttpPost]
        //[SwaggerResponseAttribute((int)HttpStatusCode.OK, typeof(IActionResult), Description = "Spremanje servisera iz excel tablice")]
        public IActionResult ServiseriImport(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            List<ServiserApiModel> serviseri = new List<ServiserApiModel>();
            List<string> statusi = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    try
                    {
                        using (ExcelPackage package = new ExcelPackage(file.OpenReadStream()))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[SHEET_NAME];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;


                            for (int row = 2; row <= rowCount; row++)
                            {
                                ServiserApiModel serviser = new ServiserApiModel();
                                CultureInfo culture = CultureInfo.InvariantCulture;
                                string ime = null, prezime = null, opis = null, servis = null;

                                if (worksheet.Cells[row, 1].Value != null && !worksheet.Cells[row, 1].Value.ToString().Equals(""))
                                {
                                    ime = worksheet.Cells[row, 1].Value.ToString();
                                    serviser.Ime = ime;
                                }
                                else
                                {
                                    statusi.Add("NE (Ime prazno)");
                                }
                                if (worksheet.Cells[row, 2].Value != null && !worksheet.Cells[row, 2].Value.ToString().Equals(""))
                                {
                                    prezime = worksheet.Cells[row, 2].Value.ToString();
                                    serviser.Prezime = prezime;
                                }
                                else
                                {
                                    statusi.Add("NE (Prezime prazno)");
                                }
                                if (worksheet.Cells[row, 3].Value != null && !worksheet.Cells[row, 3].Value.ToString().Equals(""))
                                {
                                    opis = worksheet.Cells[row, 3].Value.ToString();
                                    serviser.Opis = opis;
                                }
                                else
                                {
                                    opis = null;
                                    serviser.Opis = null;
                                }
                                if (worksheet.Cells[row, 4].Value != null && !worksheet.Cells[row, 4].Value.ToString().Equals(""))
                                {
                                    servis = worksheet.Cells[row, 4].Value.ToString();
                                    serviser.Servis = servis;
                                }
                                else
                                {
                                    statusi.Add("NE (Servis prazan)");
                                }

                                if (ime == null || prezime == null || servis == null)
                                {
                                    serviseri.Add(serviser);
                                    continue;
                                }
                               

                                Servis servisBaza = context.Servis.Where(s => s.Ime == serviser.Servis).FirstOrDefault();

                                if (servisBaza == null)
                                {
                                    statusi.Add("NE (Servis ne postoji)");
                                    serviseri.Add(serviser);
                                    continue;
                                }

                                try
                                {

                                    Serviser serviserBaza = new Serviser
                                    {
                                        Ime = serviser.Ime,
                                        Prezime = serviser.Prezime,
                                        Opis = serviser.Opis,
                                        IdServis = servisBaza.Id
                                    };


                                    context.Serviser.Add(serviserBaza);
                                    context.SaveChanges();
                                    statusi.Add("DA");
                                }
                                catch (Exception e)
                                {
                                    statusi.Add("NE (" + e.Message + ")");
                                }
                                finally
                                {

                                    serviseri.Add(serviser);

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
                    excel.Workbook.Properties.Title = "Status unosa servisera";
                    excel.Workbook.Properties.Author = "Nikola Gajski";
                    var worksheet = excel.Workbook.Worksheets.Add("Statusi");

                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Ime";
                    worksheet.Cells[1, 2].Value = "Prezime";
                    worksheet.Cells[1, 3].Value = "Opis";
                    worksheet.Cells[1, 4].Value = "Servis";
                    worksheet.Cells[1, 5].Value = "Spremljen";

                    for (int i = 0; i < serviseri.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = serviseri[i].Ime;
                        worksheet.Cells[i + 2, 2].Value = serviseri[i].Prezime;
                        worksheet.Cells[i + 2, 3].Value = serviseri[i].Opis;
                        worksheet.Cells[i + 2, 4].Value = serviseri[i].Servis;
                        worksheet.Cells[i + 2, 5].Value = statusi[i];
                    }

                    worksheet.Cells[1, 1, serviseri.Count + 1, 6].AutoFitColumns();
                    content = excel.GetAsByteArray();
                }
                return File(content, ExcelContentType, "StatusUnosaServisera.xlsx");
            }
            return View();
        }

        private void addRowToSheet(string ime, string prezime, string opis, string servis, ExcelWorksheet worksheet, int row)
        {
            worksheet.Cells[row, 1].Value = ime;
            worksheet.Cells[row, 2].Value = prezime;
            worksheet.Cells[row, 3].Value = opis;
            worksheet.Cells[row, 4].Value = servis;
        }

    }
}
