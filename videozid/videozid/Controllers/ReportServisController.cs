using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using Microsoft.EntityFrameworkCore;
using videozid.ViewModels;

namespace videozid.Controllers
{
    public class ReportServisController : Controller
    {
        private readonly RPPP15Context ctx;

        public ReportServisController(RPPP15Context context)
        {
            ctx = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Servisi()
        {
            string naslov = "Popis servisa";
         
            var servisi = ctx.Servis.ToList();

            PdfReport report = CreateReport(naslov);

            List<ServisDenorm> servisiDenorm = new List<ServisDenorm>();
            foreach (var s in servisi)
            {
                var tipServis = ctx.TipServisa.Where(tip => tip.IdServis == s.Id).FirstOrDefault();
                var serviseri = ctx.Serviser.Where(serviser => serviser.IdServis == s.Id).ToList();
                var uredajIdNaziv = (from ur in ctx.Uredaj
                                     join servisira in ctx.Servisira on ur.Id equals servisira.IdUredaj
                                     where (servisira.IdServis == s.Id)
                                     select ur.Naziv).ToList();

                servisiDenorm.Add(Denormalize(s,tipServis,serviseri,uredajIdNaziv));
            }

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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(servisiDenorm));
            report.MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Suma");
            });

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(2);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ServisDenorm.Id));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell("Id ");
                    //column.ColumnItemsTemplate(template =>
                    //{
                    //    template.TextBlock();
                    //    template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                    //                                        ? string.Empty : string.Format("{0:2}", obj));
                    //});
                    column.AggregateFunction(aggregateFunction =>
                    {
                        aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                        //aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                        //                                    ? string.Empty : string.Format("{0:C2}", obj));
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ServisDenorm.ImeServisa));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Ime servisa");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ServisDenorm.OpisServisa));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Opis servisa", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ServisDenorm.Tip));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Tip servisa", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ServisDenorm.Serviseri));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(5);
                    column.HeaderCell("Serviseri", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ServisDenorm.Uredaji));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(6);
                    column.HeaderCell("Uređaji", horizontalAlignment: HorizontalAlignment.Center);
                });


            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=servisi.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
                return NotFound();
        }

        #region Private methods
        private PdfReport CreateReport(string imeServisa)
        {
            var pdf = new PdfReport();

            pdf.DocumentPreferences(doc =>
            {
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata
                {
                    Author = "Nikola Gajski",
                    Application = "Videozid",
                    Title = imeServisa
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

        private ServisDenorm Denormalize(Servis s, TipServisa tipServis, List<Serviser> serviseri, List<string> uredaji)
        {
            ServisDenorm servis = new ServisDenorm();
            servis.Id = s.Id;
            servis.ImeServisa = s.Ime;
            servis.SetOpis(s.Opis);
            if (tipServis != null)
            {
                servis.Tip = tipServis.Tip;
            }
            
            servis.SetServiseri(s.Serviser);
            servis.SetUredaji(uredaji);
            return servis;
        }

        #endregion
    }
}