using ClosedXML.Excel;
using StoreClient.Light.Models;
using System.Diagnostics;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace StoreClient.Light.Services;

public class ReportService
{
    // Ruta base: Mis Documentos / POS_Reports
    private string BasePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "POS_Reports");

    public ReportService()
    {
        if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);
    }

    public async Task<string> GenerateExcelAsync(List<Sale> sales, string fileName)
    {
        string fullPath = Path.Combine(BasePath, $"{fileName}.xlsx");

        await Task.Run(() =>
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Ventas");

                // Estilos sencillos
                ws.Cell(1, 1).Value = "Reporte de Ventas";
                ws.Range("A1:E1").Merge().Style.Font.Bold = true;
                
                // Headers
                ws.Cell(2, 1).Value = "Folio";
                ws.Cell(2, 2).Value = "Fecha";
                ws.Cell(2, 3).Value = "Cliente";
                ws.Cell(2, 4).Value = "Total";
                
                int row = 3;
                foreach (var s in sales)
                {
                    ws.Cell(row, 1).Value = s.Id;
                    ws.Cell(row, 2).Value = s.Date;
                    ws.Cell(row, 3).Value = s.CustomerName ?? "General";
                    ws.Cell(row, 4).Value = s.Total;
                    row++;
                }
                
                ws.Columns().AdjustToContents();
                workbook.SaveAs(fullPath);
            }
        });

        AbrirArchivo(fullPath);
        return fullPath;
    }

    public async Task<string> GeneratePdfAsync(List<Sale> sales, string fileName)
    {
        string fullPath = Path.Combine(BasePath, $"{fileName}.pdf");

        await Task.Run(() =>
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("Reporte de Ventas").FontSize(20).SemiBold();
                    
                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(c => 
                        { 
                            c.ConstantColumn(50); 
                            c.RelativeColumn(); 
                            c.RelativeColumn(); 
                            c.ConstantColumn(80); 
                        });

                        table.Header(h => 
                        { 
                            h.Cell().Text("Folio").Bold(); 
                            h.Cell().Text("Fecha").Bold();
                            h.Cell().Text("Cliente").Bold();
                            h.Cell().AlignRight().Text("Total").Bold(); 
                        });

                        foreach (var s in sales)
                        {
                            table.Cell().Text($"#{s.Id}");
                            table.Cell().Text($"{s.Date:dd/MM/yy}");
                            table.Cell().Text(s.CustomerName ?? "General");
                            table.Cell().AlignRight().Text($"{s.Total:C2}");
                        }
                    });
                });
            });

            doc.GeneratePdf(fullPath);
        });

        AbrirArchivo(fullPath);
        return fullPath;
    }

    private void AbrirArchivo(string path)
    {
        try { Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true }); }
        catch { /* Ignorar */ }
    }
}