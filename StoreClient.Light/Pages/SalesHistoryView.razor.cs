using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using StoreClient.Light.Utils;

namespace StoreClient.Light.Pages;

public partial class SalesHistoryView
{
    [Inject] public ApiService Api { get; set; }
    [Inject] public ToastService Toast { get; set; }

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

    protected override async Task OnInitializedAsync()
    {
        await CargarVentas();
    }

    private async Task CargarVentas()
    {
        isLoading = true;
        selectedSale = null; // Limpiar selección al recargar
        try
        {
            string start = startDate.ToString("yyyy-MM-dd");
            string end = endDate.ToString("yyyy-MM-dd");
            
            // Llamada al endpoint con filtros
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
        // Cargar detalles completos (productos) de la venta seleccionada
        try
        {
            // Pedimos al backend la venta completa por ID
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

    // --- ACCIONES ---

    private void VerTicket()
    {
        if (selectedSale == null) return;

        // Generar texto usando tu utilidad existente
        ticketContent = TicketGenerator.GenerateTicketString(selectedSale);
        ticketTitle = $"Ticket de Venta #{selectedSale.Id}";
        showTicketModal = true;
    }

    private async Task CancelarVenta()
    {
        if (selectedSale == null) return;

        bool success = await Api.DeleteAsync($"sales/{selectedSale.Id}");
        
        if (success)
        {
            int id = selectedSale.Id; // Guardar ID para el mensaje
            selectedSale = null;      // Limpiar selección
            
            await CargarVentas();
            
            // USAR EL SERVICIO GLOBAL
            Toast.ShowSuccess($"Venta #{id} cancelada y stock restaurado.");
        }
        else
        {
            Toast.ShowError("Error al intentar cancelar la venta.");
        }
    }
    
    // Cerrar modal de ticket
    private void OnTicketClosed()
    {
        showTicketModal = false;
    }
}