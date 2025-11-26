using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;

namespace StoreClient.Light.Pages;

public partial class SupplierView
{
    [Inject] public ApiService Api { get; set; }
    [Inject] public ToastService Toast { get; set; }

    private List<Supplier> allSuppliers = new();
    private List<Supplier> filteredSuppliers = new();
    
    private string searchQuery = "";
    private bool showInactive = false;
    private bool isLoading = true;

    // Modal
    private bool showModal = false;
    private int? selectedId = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            string query = showInactive ? "?all=true" : "";
            allSuppliers = await Api.GetListAsync<Supplier>($"catalogs/suppliers{query}");
            FilterData();
        }
        finally { isLoading = false; StateHasChanged(); }
    }

    private void HandleSearch(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? "";
        FilterData();
    }

    private void FilterData()
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            filteredSuppliers = allSuppliers;
        }
        else
        {
            var q = searchQuery.ToLower();
            filteredSuppliers = allSuppliers
                .Where(s => s.Name.ToLower().Contains(q) || 
                           (s.ContactName != null && s.ContactName.ToLower().Contains(q)))
                .ToList();
        }
    }

    private void ToggleInactive() { showInactive = !showInactive; _ = LoadData(); }
    
    // CRUD
    private void OpenCreate() { selectedId = null; showModal = true; }
    private void OpenEdit(int id) { selectedId = id; showModal = true; }

    private async Task HandleDelete(Supplier s)
    {
        bool success = await Api.DeleteAsync($"catalogs/suppliers/{s.Id}");
        
        if (success) 
        {
            await LoadData();
            Toast.ShowSuccess($"Proveedor '{s.Name}' eliminado.");
        }
        else 
        {
            Toast.ShowError("Error al eliminar proveedor.");
        }
    }

    private async Task OnModalSaved()
    {
        await LoadData();
        Toast.ShowSuccess("Proveedor guardado correctamente.");
    }
}