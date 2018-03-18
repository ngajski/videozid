using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using videozid.ViewModels;
using Microsoft.EntityFrameworkCore;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using OfficeOpenXml;
using videozid.ViewModels.Api;
using videozid.Extensions;

namespace videozid.Controllers
{
    public class ReportController : Controller
    {

        private readonly RPPP15Context _context;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public ReportController(RPPP15Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UredajiExcel()
        {
            var list = new List<UredajDenorm>();
            var uredaji = _context.Uredaj
                .Include(u => u.IdNadredeneKomponenteNavigation)
                .Include(u => u.IdStatusaNavigation)
                .Include(u => u.IdZidaNavigation)
                .ToList();

            if (uredaji.Count <= 0)
                return NotFound();

            foreach (var u in uredaji)
            {
                var ur = LoadDetails(u);
                list.Add(ur.GetInfo());
            }
            list.Sort((a, b) => a.Naziv.CompareTo(b.Naziv));

            byte[] content;
            using (ExcelPackage excel = list.CreateExcel("Ureðaji"))
            {
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "uredaji.xlsx");
        }

        public async Task<IActionResult> VideozidoviExcel()
        {
            var list = new List<VideozidDenorm>();
            var zidovi = _context.Videozid
                .ToList();

            if (zidovi.Count <= 0)
                return NotFound();

            foreach (var v in zidovi)
            {
                var ekrani = _context.EkranZida.Where(u => u.IdZida == v.Id).Include(u => u.IdUredajaNavigation).ToList();
                var vi = new VideozidDetailsViewModel(v, ekrani).GetInfo();
                list.Add(vi);
            }
            list.Sort((a, b) => a.Naziv.CompareTo(b.Naziv));

            byte[] content;
            using (ExcelPackage excel = list.CreateExcel("Videozidovi"))
            {
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "videozidovi.xlsx");
        }

        public async Task<IActionResult> Uredaji()
        {
            string naslov = "Popis uredaja";
            var list = new List<UredajDenorm>();
            var uredaji =  _context.Uredaj
                .Include(u => u.IdNadredeneKomponenteNavigation)
                .Include(u => u.IdStatusaNavigation)
                .Include(u => u.IdZidaNavigation)
                .ToList();

            if (uredaji.Count <= 0)
                return NotFound();

            foreach (var u in uredaji)
            {
                var ur = LoadDetails(u);
                list.Add(ur.GetInfo());
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(list));


            report.MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Ukupno");
            });

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv ureðaja");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.DatumNabavke);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("Datum Nabavke", horizontalAlignment: HorizontalAlignment.Center);
                });

                /*columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.NabavnaCijena);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Nabavna Cijena", horizontalAlignment: HorizontalAlignment.Center);
                });
                */
                /*columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.AktualnaCijena);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Aktualna Cijena", horizontalAlignment: HorizontalAlignment.Center);
                });
                
                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.Zida);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(1);
                    column.HeaderCell("Videozid", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.Status);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(1);
                    column.HeaderCell("Status", horizontalAlignment: HorizontalAlignment.Center);
                });
                */
                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.Zamjena);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Zamjenski ureðaji", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.ZamjenaZa);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("Zamjena Za", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.Servisi);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(3);
                    column.HeaderCell("Servisi", horizontalAlignment: HorizontalAlignment.Center);
                });


                columns.AddColumn(column =>
                {
                    column.PropertyName("Nabavna Cijena");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("Nabavna Cijena", horizontalAlignment: HorizontalAlignment.Center);
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:C2}", obj));
                    });
                    column.AggregateFunction(aggregateFunction =>
                    {
                        aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                        aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:C2}", obj));
                    });
                    column.CalculatedField(
                                  l =>
                                  {
                                      if (l == null) return string.Empty;
                                      var iznos = l.GetValueOf(nameof(UredajDenorm.NabavnaCijena));
                                      return iznos;
                                  });
                });

            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=drzave.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
                return NotFound();
        }


        private UredajDetailsViewModel LoadDetails(Uredaj ur)
        {

            var zamjenaZa = _context.ZamjenskiUredaj.Where(u => u.IdUredaja == ur.Id).Include(u => u.IdZamjenaZaNavigation).ToList();
            var zamjena = _context.ZamjenskiUredaj.Where(u => u.IdZamjenaZa == ur.Id).Include(u => u.IdUredajaNavigation).ToList();
            var servisira = _context.Servisira.Where(s => s.IdUredaj == ur.Id).Include(s => s.IdServisNavigation);

            UredajDetailsViewModel ure = new UredajDetailsViewModel(ur, zamjenaZa, zamjena, servisira);
            return ure;
        }

        public async Task<IActionResult> Videozidovi()
        {
            string naslov = "Popis videozidova";
            var list = new List<VideozidDenorm>();
            var zidovi = _context.Videozid
                .ToList();

            if (zidovi.Count <= 0)
                return NotFound();

            foreach (var v in zidovi)
            {
                var ekrani = _context.EkranZida.Where(u => u.IdZida == v.Id).Include(u => u.IdUredajaNavigation).ToList();
                var vi = new VideozidDetailsViewModel(v,ekrani).GetInfo();
                list.Add(vi);
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(list));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VideozidDenorm>(x => x.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv Videozida");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VideozidDenorm>(x => x.Lokacija);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("Lokacija", horizontalAlignment: HorizontalAlignment.Center);
                });
                
                columns.AddColumn(column =>
                {
                    column.PropertyName<VideozidDenorm>(x => x.Sirina);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Sirina", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VideozidDenorm>(x => x.Visina);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Visina", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<VideozidDenorm>(x => x.Ekrani);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(5);
                    column.HeaderCell("Ekrani zida", horizontalAlignment: HorizontalAlignment.Center);
                });

            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=drzave.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
                return NotFound();
        }

        /* #region Master-detail header
         public class MasterDetailsHeaders : IPageHeader
         {
             private string naslov;
             public MasterDetailsHeaders(string naslov)
             {
                 this.naslov = naslov;
             }
             public IPdfFont PdfRptFont { set; get; }

             public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
             {
                 var idDokumenta = newGroupInfo.GetSafeStringValueOf(nameof(StavkaDenorm.IdDokumenta));
                 var urlDokumenta = newGroupInfo.GetSafeStringValueOf(nameof(StavkaDenorm.UrlDokumenta));
                 var nazPartnera = newGroupInfo.GetSafeStringValueOf(nameof(StavkaDenorm.NazPartnera));
                 var datDokumenta = (DateTime)newGroupInfo.GetValueOf(nameof(StavkaDenorm.DatDokumenta));
                 var iznosDokumenta = (decimal)newGroupInfo.GetValueOf(nameof(StavkaDenorm.IznosDokumenta));

                 var table = new PdfGrid(relativeWidths: new[] { 2f, 5f, 2f, 3f }) { WidthPercentage = 100 };

                 table.AddSimpleRow(
                     (cellData, cellProperties) =>
                     {
                         cellData.Value = "Id dokumenta:";
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                     },
                     (cellData, cellProperties) =>
                     {
                         cellData.TableRowData = newGroupInfo; //postavi podatke retka za æeliju
                 var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                         {
                             TextPropertyName = nameof(StavkaDenorm.IdDokumenta),
                             NavigationUrlPropertyName = nameof(StavkaDenorm.UrlDokumenta),
                             BasicProperties = new CellBasicProperties
                             {
                                 HorizontalAlignment = HorizontalAlignment.Left,
                                 PdfFontStyle = DocumentFontStyle.Bold,
                                 PdfFont = PdfRptFont
                             }
                         };

                         cellData.CellTemplate = cellTemplate;
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                     },
                     (cellData, cellProperties) =>
                     {
                         cellData.Value = "Datum dokumenta:";
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                     },
                     (cellData, cellProperties) =>
                     {
                         cellData.Value = datDokumenta;
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                         cellProperties.DisplayFormatFormula = obj => ((DateTime)obj).ToString("dd.MM.yyyy");
                     });

                 table.AddSimpleRow(
                     (cellData, cellProperties) =>
                     {
                         cellData.Value = "Partner:";
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                     },
                     (cellData, cellProperties) =>
                     {
                         cellData.Value = nazPartnera;
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                     },
                     (cellData, cellProperties) =>
                     {
                         cellData.Value = "Iznos dokumenta:";
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                     },
                     (cellData, cellProperties) =>
                     {
                         cellData.Value = iznosDokumenta;
                         cellProperties.DisplayFormatFormula = obj => ((decimal)obj).ToString("C2");
                         cellProperties.PdfFont = PdfRptFont;
                         cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                     });
                 return table.AddBorderToTable(borderColor: BaseColor.LightGray, spacingBefore: 5f);
             }

             public PdfGrid RenderingReportHeader(Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
             {
                 var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
                 table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = naslov;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                 return table.AddBorderToTable();
             }
         }
         #endregion
         */
        private PdfReport CreateReport(string naslov)
        {
            var pdf = new PdfReport();

            pdf.DocumentPreferences(doc =>
            {
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata
                {
                    Author = "RPPP15",
                    Application = "videozid",
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
                template.BasicTemplate(BasicTemplate.ProfessionalTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
          //table.NumberOfDataRowsPerPage(20);
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
    }
}