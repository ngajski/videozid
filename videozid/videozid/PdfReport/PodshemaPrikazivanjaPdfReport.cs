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


public class PodshemaPrikazivanjaPdfReport
{
    public static byte[] CreateInMemoryPdfReport(RPPP15Context _context) 
    {

        var podsheme = _context.PodshemaPrikazivanja.ToList();
        List <PodshemaPrikazivanjaDenorm> podshemaDenorm= new List<PodshemaPrikazivanjaDenorm>();
        foreach(var s in podsheme)
        {
            var pripada = _context.Sadrzi.Where(u => u.IdPodsheme == s.Id).FirstOrDefault();
            var shema = _context.ShemaPrikazivanja.Where(u => u.Id == pripada.IdSheme).FirstOrDefault();
            
            podshemaDenorm.Add(Denormalize(s, pripada, shema));
        }


        
        return new PdfReport().DocumentPreferences(doc =>
        {
            doc.RunDirection(PdfRunDirection.LeftToRight);
            doc.Orientation(PageOrientation.Portrait);
            doc.PageSize(PdfPageSize.A4);
            doc.DocumentMetadata(new DocumentMetadata { Author = "Tin Ceraj", Application = "PdfRpt", Subject = "PodshemaPrikazivanja", Title = "Podshema" });
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
                    defaultHeader.Message("PodshemaPrikazivanja");
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
                dataSource.StronglyTypedList(podshemaDenorm);
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
                    column.PropertyName(nameof(PodshemaPrikazivanjaDenorm.Naziv));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("Naziv");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PodshemaPrikazivanjaDenorm.Opis));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Opis");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PodshemaPrikazivanjaDenorm.Sheme));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Pripada shemi");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PodshemaPrikazivanjaDenorm.Od));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("Od");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(PodshemaPrikazivanjaDenorm.Do));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(3);
                    column.HeaderCell("Do");
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

    private static PodshemaPrikazivanjaDenorm Denormalize(PodshemaPrikazivanja s, Sadrzi sadrzi, ShemaPrikazivanja shema)
    {
        PodshemaPrikazivanjaDenorm podshema = new PodshemaPrikazivanjaDenorm();
        podshema.Id = s.Id;
        podshema.Od = sadrzi.Od.ToString();
        podshema.Do = sadrzi.Do.ToString();

        podshema.SetNaziv(s.Naziv);
        podshema.SetOpis(s.Opis);
        podshema.SetSheme(shema.Naziv);
        return podshema;
       
    }
}
