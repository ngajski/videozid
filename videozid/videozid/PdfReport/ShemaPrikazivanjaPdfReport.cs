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


public static class ShemaPrikazivanjaPdfReport
{
    public static byte[] CreateInMemoryPdfReport(RPPP15Context _context)
    {
        var sheme = _context.ShemaPrikazivanja.ToList();
        List<ShemaPrikazivanjaDenorm> shemDenorm = new List<ShemaPrikazivanjaDenorm>();
        foreach(var p in sheme)
        {
            //var Sadrzaj = _context.Sadrzaj.Where(x => x.Id == p.IdSadrzaja).FirstOrDefault();
            //var Kategorija = _context.Kategorija.Where(x => x.Id == p.IdKategorije).FirstOrDefault();
            shemDenorm.Add(Denormalize(p));
        }

        return new PdfReport().DocumentPreferences(doc =>
        {
            doc.RunDirection(PdfRunDirection.LeftToRight);
            doc.Orientation(PageOrientation.Portrait);
            doc.PageSize(PdfPageSize.A4);
            doc.DocumentMetadata(new DocumentMetadata { Author = "Tin Ceraj", Application = "PdfRpt", Subject = "ShemaPrikazivanja", Title = "ShemaPrikazivanja" });
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
                    defaultHeader.Message("ShemaPrikazivanja");
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
                dataSource.StronglyTypedList(shemDenorm);
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
                    column.PropertyName(nameof(ShemaPrikazivanjaDenorm.Naziv));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("Naziv");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ShemaPrikazivanjaDenorm.Opis));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Opis");
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

    private static ShemaPrikazivanjaDenorm Denormalize(ShemaPrikazivanja p)
    {
        ShemaPrikazivanjaDenorm shemica = new ShemaPrikazivanjaDenorm();
        shemica.Id = p.Id;
        shemica.SetNaziv(p.Naziv);
        shemica.SetOpis(p.Opis);

        return shemica;

    }
}
