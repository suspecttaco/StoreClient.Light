using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;

namespace StoreClient.Light.Pages;

public partial class CustomerView
{
    [Inject] public ApiService Api { get; set; }
    [Inject] public ToastService Toast { get; set; }
    [Inject] public ConfirmService Confirm { get; set; }

    // Datos
    private List<Customer> allCustomers = new();
    private List<Customer> filteredCustomers = new();

    // Estado UI
    private string searchQuery = "";
    private bool showInactive = false;
    private bool isLoading = true;

    // Estado del Modal
    private bool showModal = false;
    private int? selectedCustomerId = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            // Pedir datos a api
            string query = showInactive ? "?all=true" : "";
            allCustomers = await Api.GetListAsync<Customer>($"catalogs/customers{query}");
            
            FilterData();
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

    // Busqueda y filtros
    private void HandleSearch(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? "";
        FilterData();
    }

    private void ToggleInactive()
    {
        showInactive = !showInactive;
        _ = LoadData(); // Recargar desde API
    }

    private void FilterData()
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            filteredCustomers = allCustomers;
        }
        else
        {
            var q = searchQuery.ToLower();
            filteredCustomers = allCustomers
                .Where(c => c.Name.ToLower().Contains(q) || 
                            (c.Phone != null && c.Phone.Contains(q)))
                .ToList();
        }
    }

    // CRUD
    private void OpenCreateModal()
    {
        selectedCustomerId = null; // Modo Crear
        showModal = true;
    }

    private void OpenEditModal(int id)
    {
        selectedCustomerId = id; // Modo Editar
        showModal = true;
    }

    private async Task HandleDelete(Customer customer)
    {
        bool flag = await Confirm.Show(
            "¿Eliminar Cliente?",
            "No podra registrar ventas a este cliente en el futuro.",
            "Si, Eliminar",
            isDanger: true);
        
        if (flag)
        {
            bool success = await Api.DeleteAsync($"catalogs/customers/{customer.Id}");
            if (success)
            {
                await LoadData();
                Toast.ShowSuccess($"Cliente '{customer.Name}' eliminado.");
            }
            else
            {
                Toast.ShowError("No se pudo eliminar al cliente.");
            }
        }
    }

    private async Task OnModalSaved()
    {
        // Se llama cuando el modal guarda exitosamente
        await LoadData();
        Toast.ShowSuccess("Información del cliente guardada.");
    }
}