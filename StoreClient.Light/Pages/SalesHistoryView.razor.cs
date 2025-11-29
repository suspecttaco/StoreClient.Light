using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using StoreClient.Light.Utils;

namespace StoreClient.Light.Pages;

public partial class SalesHistoryView
{
    [Inject] public ApiService Api { get; set; }
    [Inject] public ToastService Toast { get; set; }
    [Inject] public ConfirmService Confirm { get; set; }
    [Inject] public ReportService Report { get; set; }

    // Datos
    private List<Sale> salesList = new();
    private Sale? selectedSale; // Venta seleccionada actualmente

    // Filtros
    private DateTime startDate = DateTime.Today;
    private DateTime endDate = DateTime.Today;
    private bool isLoading = false;

    // Estado para el Modal de Ticket
    private bool showTicketModal = false;
    private string ticketContent = "";
    private string ticketTitle = "";
    
    // Reportes
    private bool showExportModal = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadSales();
    }

    private async Task LoadSales()
    {
        isLoading = true;
        selectedSale = null;
        try
        {
            string start = startDate.ToString("yyyy-MM-dd");
            string end = endDate.ToString("yyyy-MM-dd");

            salesList = await Api.GetListAsync<Sale>($"sales/?start={start}&end={end}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task SelectSale(Sale sale)
    {
        try
        {
            var fullSale = await Api.GetByIdAsync<Sale>($"sales/{sale.Id}");
            if (fullSale != null)
            {
                selectedSale = fullSale;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando detalles: {ex.Message}");
        }
    }

    // Acciones
    private void SeeTicket()
    {
        if (selectedSale == null) return;
        
        ticketContent = TicketGenerator.GenerateTicketString(selectedSale);
        ticketTitle = $"Ticket de Venta #{selectedSale.Id}";
        showTicketModal = true;
    }

    private async Task CancelSale()
    {
        if (selectedSale == null) return;

        bool flag = await Confirm.Show(
            "¿Cancelar Venta?",
            $"Se reembolsara el total de ${selectedSale.Total:C2} y se restaurara el stock.",
            "Cancelar Venta",
            isDanger: true);

        if (flag)
        {
            bool success = await Api.DeleteAsync($"sales/{selectedSale.Id}");
        
            if (success)
            {
                int id = selectedSale.Id;
                selectedSale = null;      
            
                await LoadSales();
                
                Toast.ShowSuccess($"Venta #{id} cancelada y stock restaurado.");
            }
            else
            {
                Toast.ShowError("Error al intentar cancelar la venta.");
            }
        }
    }

    private void OnTicketClosed()
    {
        showTicketModal = false;
    }

    private async Task HandleExport((string Format, string Name) args)
    {
        if (salesList.Count == 0) 
        {
            Toast.ShowWarning("No hay datos para exportar.");
            return;
        }

        Toast.ShowInfo($"Generando {args.Format.ToUpper()}...");

        try
        {
            string path = "";
            if (args.Format == "excel")
                path = await Report.GenerateExcelAsync(salesList, args.Name);
            else
                path = await Report.GeneratePdfAsync(salesList, args.Name);

            Toast.ShowSuccess($"Archivo guardado en Documentos.");
        }
        catch (Exception ex)
        {
            Toast.ShowError("Error al exportar.");
            Console.WriteLine(ex);
        }
    }
}