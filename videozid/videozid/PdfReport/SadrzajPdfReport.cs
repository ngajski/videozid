using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.DataSources;
using videozid.Models;
using PdfRpt.Core.Helper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using videozid.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;


public class SadrzajPdfReport
{
    public static byte[] CreateInMemoryPdfReport(RPPP15Context _context) 
    {

        var sadrzaji = _context.Sadrzaj.ToList();
        List <SadrzajDenorm> sadrzajDenorm= new List<SadrzajDenorm>();
        foreach(var s in sadrzaji)
        {
            var tipSadrzaja = _context.TipSadrzaja.Where(tip => tip.Id == s.IdTipa).FirstOrDefault();
            var Autor = _context.Korisnik.Where(autor => autor.Id == s.IdAutora).FirstOrDefault();
            var OdobrenOd = _context.Korisnik.Where(od => od.Id == s.IdOdobrenOd).FirstOrDefault();

            sadrzajDenorm.Add(Denormalize(s, tipSadrzaja, Autor, OdobrenOd));
        }


        
        return new PdfReport().DocumentPreferences(doc =>
        {
            doc.RunDirection(PdfRunDirection.LeftToRight);
            doc.Orientation(PageOrientation.Portrait);
            doc.PageSize(PdfPageSize.A4);
            doc.DocumentMetadata(new DocumentMetadata { Author = "Mislav Brusac", Application = "PdfRpt", Subject = "Sadrzaj", Title = "Sadrzaj" });
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
                    defaultHeader.Message("Sadrzaj");
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
                dataSource.StronglyTypedList(sadrzajDenorm);
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
                    column.PropertyName(nameof(SadrzajDenorm.Ime));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("Naziv");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(SadrzajDenorm.Opis));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Opis");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(SadrzajDenorm.Url));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("URL");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(SadrzajDenorm.Autora));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("Ime autora");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(SadrzajDenorm.OdobrenOd));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(3);
                    column.HeaderCell("Odobren od");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(SadrzajDenorm.Tip));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(3);
                    column.HeaderCell("Tip Sadrzaja");
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

    private static SadrzajDenorm Denormalize(Sadrzaj s, TipSadrzaja tipSadrzaja, Korisnik autor, Korisnik odobren)
    {
        SadrzajDenorm sadrzaj = new SadrzajDenorm();
        sadrzaj.Id = s.Id;
        sadrzaj.Ime = s.Ime;
        sadrzaj.Opis = s.Opis;
        sadrzaj.Url = s.Url;


        sadrzaj.SetAutor(autor.KorisnickoIme);
        sadrzaj.SetOdobrenOd(odobren.KorisnickoIme);
        sadrzaj.SetTip(tipSadrzaja.Ime);
        return sadrzaj;
       
    }
}
