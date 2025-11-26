using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;

namespace StoreClient.Light.Pages;

public partial class InventoryView
{
    [Inject] public ApiService Api { get; set; }
    [Inject] public ToastService Toast { get; set; }

    private List<Product> allProducts = new();
    private List<Product> filteredProducts = new();
    
    private string searchQuery = "";
    private bool showInactive = false;
    private bool isLoading = true;

    // Modal
    private bool showModal = false;
    private int? selectedProductId = null;

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
            allProducts = await Api.GetListAsync<Product>($"products/{query}");
            FilterData();
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
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
            filteredProducts = allProducts;
        }
        else
        {
            var q = searchQuery.ToLower();
            filteredProducts = allProducts
                .Where(p => p.Name.ToLower().Contains(q) || (p.Code != null && p.Code.ToLower().Contains(q)))
                .ToList();
        }
    }

    private void ToggleInactive()
    {
        showInactive = !showInactive;
        _ = LoadData();
    }

    // CRUD
    private void OpenCreate()
    {
        selectedProductId = null;
        showModal = true;
    }

    private void OpenEdit(int id)
    {
        selectedProductId = id;
        showModal = true;
    }

    private async Task HandleDelete(Product p)
    {
        // Opcional: Agregar confirmación JS o Modal
        bool success = await Api.DeleteAsync($"products/{p.Id}");
        if (success)
        {
            await LoadData();
            Toast.ShowSuccess($"Producto '{p.Name}' eliminado (Inactivo).");
        }
        else
        {
            Toast.ShowError("No se puede eliminar el producto.");
        }
    }

    private async Task OnModalSaved()
    {
        await LoadData();
        Toast.ShowSuccess("Producto guardado correctamente.");
    }
}