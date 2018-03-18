using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using videozid.Models;
using videozid.ViewModels;
using OfficeOpenXml;
using videozid.ViewModels.Api;
using OfficeOpenXml.Style;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace videozid.Controllers
{
    public class ReportKorisniciController : Controller
    {
        private readonly RPPP15Context ctx;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public ReportKorisniciController(RPPP15Context ctx)
        {
            this.ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Korisnici()
        {
            string naslov = "Popis korisnika";

            var korisnici = ctx.Korisnik.ToList().OrderBy(k => k.Prezime);


            List<KorisnikDenorm> korisniciDenorm = new List<KorisnikDenorm>();
            var broj = 1;
            foreach (var k in korisnici)
            {
                var fer = ctx.FerWebAcc.Where(f => f.Id == k.FerId).FirstOrDefault();
                var dhmz = ctx.DhmzAcc.Where(d => d.Id == k.DhmzId).FirstOrDefault();
                var autori = ctx.Sadrzaj.Where(s => s.IdAutora == k.Id).ToList().OrderBy(s => s.Ime);
                var autor = autori.Select(s => s.Ime).ToList();
                var odobrioo = ctx.Sadrzaj.Where(s => s.IdOdobrenOd == k.Id).ToList().OrderBy(s => s.Ime);
                var odobrio = odobrioo.Select(s => s.Ime).ToList();


                korisniciDenorm.Add(Denormalize(k, fer, dhmz, autor, odobrio, broj));
                broj++;
            }

            PdfReport report = CreateReport(naslov);

            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion
            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(korisniciDenorm));
            report.MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Sum");
                
            });
            var admin = ctx.Administrator.Select(a => a.IdKorisnika).ToList();
            report.MainTableColumns(columns =>

            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width((float)0.5);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(KorisnikDenorm.Id));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width((float)0.5);
                    column.HeaderCell("ID", horizontalAlignment: HorizontalAlignment.Center);
                    column.AggregateFunction(aggregateFunction =>
                    {
                        aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<KorisnikDenorm>(k => k.PrezimeIme);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width((float)1.75);
                    column.HeaderCell("Ime i prezime ", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<KorisnikDenorm>(k => k.Email);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Email", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<KorisnikDenorm>(k => k.KorisnickoIme);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width((float)1.5);
                    column.HeaderCell("Korisničko ime", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<KorisnikDenorm>(k => k.Fer);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(1);
                    column.HeaderCell("FerWeb račun", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<KorisnikDenorm>(k => k.DHMZ);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(1);
                    column.HeaderCell("DHMZ račun", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(KorisnikDenorm.Autor));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width((float)1.5);
                    column.HeaderCell("Autor sadržaja", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(KorisnikDenorm.Odobrio));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(8);
                    column.Width((float)1.5);
                    column.HeaderCell("Odobrio sadržaje", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=korisnici.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
                return NotFound();
        }

        public async Task<IActionResult> KorisniciExcel()
        {
            var korisnici = ctx.Korisnik.ToList().OrderBy(k => k.Prezime);

            List<KorisnikExcelDenorm> korisniciDenorm = new List<KorisnikExcelDenorm>();
            var broj = 1;

            foreach (var k in korisnici)
            {
                var fer = ctx.FerWebAcc.Where(f => f.Id == k.FerId).FirstOrDefault();
                var dhmz = ctx.DhmzAcc.Where(d => d.Id == k.DhmzId).FirstOrDefault();
                var autori = ctx.Sadrzaj.Where(s => s.IdAutora == k.Id).ToList().OrderBy(s => s.Ime);
                var odobrio = ctx.Sadrzaj.Where(s => s.IdOdobrenOd == k.Id).ToList().OrderBy(s => s.Ime);
                var admin = ctx.Administrator.Where(a => a.IdKorisnika == k.Id).FirstOrDefault();
                var jeAdmin = true;

                if(admin == null)
                {
                    jeAdmin = false;
                }

                korisniciDenorm.Add(DenormalizeKorisnikExcel(k, fer, dhmz, jeAdmin, autori, odobrio, broj));
                broj++;
            }

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis korisnika";
                excel.Workbook.Properties.Author = "Robert Holovka";
                var worksheet = excel.Workbook.Worksheets.Add("korisnici");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Prezime";
                worksheet.Cells[1, 2].Value = "Ime";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Lozinka";
                worksheet.Cells[1, 5].Value = "Korisničko ime";
                worksheet.Cells[1, 6].Value = "Administrator";
                worksheet.Cells[1, 7].Value = "FerWeb račun";
                worksheet.Cells[1, 8].Value = "Dhmz račun";
                worksheet.Cells[1, 9].Value = "Sadržaj";
                worksheet.Cells[1, 10].Value = "Tip";
                worksheet.Cells[1, 11].Value = "Ime";
                worksheet.Cells[1, 12].Value = "Opis";
                worksheet.Cells[1, 13].Value = "URL:";




                //range.Style.Border.Top.Color.SetColor(Color.Red);
                var offset = 0;
                for (int i = 0; i < korisniciDenorm.Count(); i++)
                {
                    worksheet.Cells[offset + i + 2, 1].Value = korisniciDenorm[i].Prezime;
                    worksheet.Cells[offset + i + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[offset + i + 2, 1].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);

                    worksheet.Cells[offset + i + 2, 2].Value = korisniciDenorm[i].Ime;
                    worksheet.Cells[offset + i + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[offset + i + 2, 2].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);

                    worksheet.Cells[offset + i + 2, 3].Value = korisniciDenorm[i].Email;
                    worksheet.Cells[offset + i + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[offset + i + 2, 3].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);

                    worksheet.Cells[offset + i + 2, 4].Value = korisniciDenorm[i].Lozinka;
                    worksheet.Cells[offset + i + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[offset + i + 2, 4].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);

                    worksheet.Cells[offset + i + 2, 5].Value = korisniciDenorm[i].KorisnickoIme;
                    worksheet.Cells[offset + i + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[offset + i + 2, 5].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);

                    worksheet.Cells[offset + i + 2, 6].Value = korisniciDenorm[i].jeAdmin;
                    worksheet.Cells[offset + i + 2, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[offset + i + 2, 6].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);

                    var biloJe = false;
                    //FerWeb
                    if(korisniciDenorm[i].Fer != null)
                    {
                        biloJe = true;
                        worksheet.Cells[offset + i + 2, 7].Value = "Ime: " + korisniciDenorm[i].Fer.KorisnickoIme;
                     

                        worksheet.Cells[offset + i + 3, 7].Value = "Lozinka: " + korisniciDenorm[i].Fer.Lozinka;
                        worksheet.Cells[offset + i + 3, 7].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                        worksheet.Cells[offset + i + 3, 7].Style.Border.Left.Color.SetColor(System.Drawing.Color.DarkRed);

                        if (korisniciDenorm[i].Fer.DozvolaServer == null)
                        {
                            worksheet.Cells[offset + i + 4, 7].Value = "Ne posjeduje dozvolu";
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Left.Color.SetColor(System.Drawing.Color.DarkRed);
                        }
                        else
                        {
                            worksheet.Cells[offset + i + 4, 7].Value = "Dozvola: " + korisniciDenorm[i].Fer.DozvolaServer;
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[offset + i + 4, 7].Style.Border.Left.Color.SetColor(System.Drawing.Color.DarkRed);
                        }                     
                    }
                    else
                    {
                        worksheet.Cells[offset + i + 2, 7].Value = "-";
                        worksheet.Cells[offset + i + 2, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                        worksheet.Cells[offset + i + 2, 7].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);
                    }

                    //Dhmz
                    if (korisniciDenorm[i].DHMZ != null)
                    {
                        biloJe = true;
                        worksheet.Cells[offset + i + 2, 8].Value = "Ime: " + korisniciDenorm[i].DHMZ.KorisnickoIme;
                        worksheet.Cells[offset + i + 3, 8].Value = "Lozinka: " + korisniciDenorm[i].DHMZ.Lozinka;
                        if (korisniciDenorm[i].DHMZ.DozvolaServer == null)
                        {
                            worksheet.Cells[offset + i + 4, 8].Value = "Ne posjeduje dozvolu";
                            worksheet.Cells[offset + i + 4, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[offset + i + 4, 8].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);
                        }
                        else
                        {
                            worksheet.Cells[offset + i + 4, 8].Value = "Dozvola: " + korisniciDenorm[i].DHMZ.DozvolaServer;
                            worksheet.Cells[offset + i + 4, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[offset + i + 4, 8].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);
                        }
                    }
                    else
                    {
                        worksheet.Cells[offset + i + 2, 8].Value = "-";
                        worksheet.Cells[offset + i + 2, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                        worksheet.Cells[offset + i + 2, 8].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkRed);

                    }

                    //sadrzaji
                    var odobreni = korisniciDenorm[i].Odobrio;
                    var autorSadrzaja = korisniciDenorm[i].Autor;

                    var j = 0;

                    foreach (var temp in autorSadrzaja)
                    {
                        worksheet.Cells[offset + j + i + 2, 9].Value = "Autor:";
                        var tip = ctx.TipSadrzaja.Where(t => t.Id == temp.IdTipa).FirstOrDefault();
                        worksheet.Cells[offset + j + i + 2, 10].Value = tip.Ime;
                        worksheet.Cells[offset + j + i + 2, 11].Value = temp.Ime;
                        worksheet.Cells[offset + j + i + 2, 12].Value = temp.Opis;
                        worksheet.Cells[offset + j + i + 2, 13].Value = temp.Url;

                        j++;
                    }
                    foreach (var temp in odobreni)
                    {
                        worksheet.Cells[offset + j + i + 2, 9].Value = "Odobrio:";
                        var tip = ctx.TipSadrzaja.Where(t => t.Id == temp.IdTipa).FirstOrDefault();
                        worksheet.Cells[offset + j + i + 2, 10].Value = tip.Ime;
                        worksheet.Cells[offset + j + i + 2, 11].Value = temp.Ime;
                        worksheet.Cells[offset + j + i + 2, 12].Value = temp.Opis;
                        worksheet.Cells[offset + j + i + 2, 13].Value = temp.Url;
                        j++;
                    }
                    if ((j-1) > 2)
                    {
                        offset += j - 1;
                    }
                    else if(biloJe)
                    {
                        offset += 2;
                    }

                }

                worksheet.Cells[1, 1, korisniciDenorm.Count + 1, 13].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "korisnici.xlsx");
        }

        public async Task<IActionResult> FerWebAccExcel()
        {
            var racuni = await ctx.Korisnik.Where(k => k.Fer != null)
            .Select(r => new IndexAccApiModel
            {
                KorisnickoIme = r.Fer.KorisnickoIme,
                Lozinka = r.Fer.Lozinka,
                DozvolaServer = r.Fer.DozvolaServer,
                Vlasnik = r.KorisnickoIme
            })
            .ToListAsync();
            racuni.OrderBy(k => k.KorisnickoIme);

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis FerWeb računa";
                excel.Workbook.Properties.Author = "Robert Holovka";
                var worksheet = excel.Workbook.Worksheets.Add("racuni");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Korisničko ime";
                worksheet.Cells[1, 2].Value = "Lozinka";
                worksheet.Cells[1, 3].Value = "Dozvola server";
                worksheet.Cells[1, 4].Value = "Pripada";

                for (int i = 0; i < racuni.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = racuni[i].KorisnickoIme;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = racuni[i].Lozinka;
                    worksheet.Cells[i + 2, 3].Value = racuni[i].DozvolaServer;
                    worksheet.Cells[i + 2, 4].Value = racuni[i].Vlasnik;
                }

                worksheet.Cells[1, 1, racuni.Count + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "FerWebAcc.xlsx");
        }

        public async Task<IActionResult> DhmzAccExcel()
        {
            var racuni = await ctx.Korisnik.Where(k => k.Dhmz != null)
            .Select(r => new IndexAccApiModel
            {
                KorisnickoIme = r.Dhmz.KorisnickoIme,
                Lozinka = r.Dhmz.Lozinka,
                DozvolaServer = r.Dhmz.DozvolaServer,
                Vlasnik = r.KorisnickoIme
            })
            .ToListAsync();
            racuni.OrderBy(k => k.KorisnickoIme);

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis DHMZ računa";
                excel.Workbook.Properties.Author = "Robert Holovka";
                var worksheet = excel.Workbook.Worksheets.Add("racuni");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Korisničko ime";
                worksheet.Cells[1, 2].Value = "Lozinka";
                worksheet.Cells[1, 3].Value = "Dozvola server";
                worksheet.Cells[1, 4].Value = "Pripada";

                for (int i = 0; i < racuni.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = racuni[i].KorisnickoIme;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = racuni[i].Lozinka;
                    worksheet.Cells[i + 2, 3].Value = racuni[i].DozvolaServer;
                    worksheet.Cells[i + 2, 4].Value = racuni[i].Vlasnik;
                }

                worksheet.Cells[1, 1, racuni.Count + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "DhmzAcc.xlsx");
        }

        private PdfReport CreateReport(string naslov)
        {
            var pdf = new PdfReport();

            pdf.DocumentPreferences(doc =>
            {
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata
                {
                    Author = "Robert Holovka",
                    Application = "Videozid.MVC Core",
                    Title = naslov
                });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.AppleOrchardTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);

                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = true,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 150,
                    SpacingAfterAllGroupsSummary = 5f
                });
                table.SpacingAfter(4f);
            });

            return pdf;
        }
        private KorisnikDenorm Denormalize(Korisnik k, FerWebAcc f, DhmzAcc d, List<String> autor, List<String> odobrio, int broj)
        {
            KorisnikDenorm korisnik = new KorisnikDenorm();
            korisnik.Id = k.Id;
            korisnik.Email = k.Email;
            korisnik.KorisnickoIme = k.KorisnickoIme;
            if(f != null)
            {
                korisnik.Fer = f.KorisnickoIme;
            }
            if (d != null)
            {
                korisnik.DHMZ = d.KorisnickoIme;
            }
            korisnik.SetAutor(autor);
            korisnik.SetOdobrio(odobrio);
            korisnik.SetPrezimeIme(k.Ime, k.Prezime);
            korisnik.Id = k.Id;

            return korisnik;
        }
        private KorisnikExcelDenorm DenormalizeKorisnikExcel(Korisnik k, FerWebAcc f, DhmzAcc d, bool jeAdmin, IOrderedEnumerable<Sadrzaj> autor, IOrderedEnumerable<Sadrzaj> odobrio, int broj)
        {
            KorisnikExcelDenorm korisnik = new KorisnikExcelDenorm();

            korisnik.Id = k.Id;
            korisnik.Email = k.Email;
            korisnik.KorisnickoIme = k.KorisnickoIme;
            korisnik.Prezime = k.Prezime;
            korisnik.Ime = k.Ime;
            if (jeAdmin)
            {
                korisnik.jeAdmin = "DA";
            }
            else
            {
                korisnik.jeAdmin = "NE";
            }
            
            korisnik.Fer = f;
            korisnik.DHMZ = d;
            korisnik.Autor = autor;
            korisnik.Odobrio = odobrio;
            korisnik.Lozinka = k.Lozinka;

            return korisnik;
        }
    }
}
