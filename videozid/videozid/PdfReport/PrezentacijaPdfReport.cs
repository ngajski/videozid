using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;
using videozid.Models;
using videozid.ViewModels;
using PdfRpt.Core.Helper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;


public static class PrezentacijaPdfReport
{
    public static byte[] CreateInMemoryPdfReport(RPPP15Context _context)
    {
        var prezentacije = _context.Prezentacija.ToList();
        List<PrezentacijaDenorm> prezDenorm = new List<PrezentacijaDenorm>();
        foreach(var p in prezentacije)
        {
            var Sadrzaj = _context.Sadrzaj.Where(x => x.Id == p.IdSadrzaja).FirstOrDefault();
            var Kategorija = _context.Kategorija.Where(x => x.Id == p.IdKategorije).FirstOrDefault();

            prezDenorm.Add(Denormalize(p, Sadrzaj, Kategorija));
        }

        return new PdfReport().DocumentPreferences(doc =>
        {
            doc.RunDirection(PdfRunDirection.LeftToRight);
            doc.Orientation(PageOrientation.Portrait);
            doc.PageSize(PdfPageSize.A4);
            doc.DocumentMetadata(new DocumentMetadata { Author = "Mislav Brusac", Application = "PdfRpt", Subject = "Prezentacija", Title = "Prezentacija" });
            doc.Compression(new CompressionSettings
            {
                EnableCompression = true,
                EnableFullCompression = true
            });
        })
            /*.DefaultFonts(fonts =>
            {
                fonts.Path(Path.Combine(wwwroot, "fonts", "verdana.ttf"),
                    Path.Combine(wwwroot, "fonts", "tahoma.ttf"));
                fonts.Size(9);
                fonts.Color(System.Drawing.Color.Black);
            })*/
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd/MM/yyyy"));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    //defaultHeader.ImagePath(Path.Combine(wwwroot, "images", "01.png"));
                    defaultHeader.Message("Prezentacija");
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ClassicTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(prezDenorm);
            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Sazetak");
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("Redni broj");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PrezentacijaDenorm.XKoord));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("X koordinata");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PrezentacijaDenorm.YKoord));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Y koordinata");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PrezentacijaDenorm.Sirina));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Sirina");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PrezentacijaDenorm.Visina));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("Visina");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PrezentacijaDenorm.Kategorije));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(3);
                    column.HeaderCell("Kategorija");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PrezentacijaDenorm.Sadrzaja));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(3);
                    column.HeaderCell("Sadrzaj");
                });
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Export(export =>
            {
                export.ToExcel();
            })
            .GenerateAsByteArray(); // creating an in-memory PDF file
    }

    private static PrezentacijaDenorm Denormalize(Prezentacija p, Sadrzaj sadrzaj, Kategorija kategorija)
    {
        PrezentacijaDenorm prez = new PrezentacijaDenorm();
        prez.Id = p.Id;
        prez.Sirina = p.Sirina;
        prez.Visina = p.Visina;
        prez.XKoord = p.XKoord;
        prez.YKoord = p.YKoord;

        prez.SetKategorija(kategorija.Vrsta);
        prez.SetSadrzaj(sadrzaj.Ime);

        return prez;

    }
}
