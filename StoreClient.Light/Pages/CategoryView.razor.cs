using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;

namespace StoreClient.Light.Pages;

public partial class CategoryView
{
    [Inject] public ApiService Api { get; set; }
    [Inject] public ToastService Toast { get; set; }
    [Inject] public ConfirmService Confirm { get; set; }

    private List<Category> allCategories = new();
    private List<Category> filteredCategories = new();
    
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
            allCategories = await Api.GetListAsync<Category>($"catalogs/categories{query}");
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
            filteredCategories = allCategories;
        }
        else
        {
            var q = searchQuery.ToLower();
            filteredCategories = allCategories
                .Where(c => c.Name.ToLower().Contains(q) || 
                           (c.Description != null && c.Description.ToLower().Contains(q)))
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
        selectedId = null;
        showModal = true;
    }

    private void OpenEdit(int id)
    {
        selectedId = id;
        showModal = true;
    }

    private async Task HandleDelete(Category c)
    {
        bool flag = await Confirm.Show("¿Eliminar Categoria?",
            "No podra registrar nuevos productos en esta categoria",
            "Si, Eliminar.",
            isDanger: true);

        if (flag)
        {
            bool success = await Api.DeleteAsync($"catalogs/categories/{c.Id}");
            if (success)
            {
                await LoadData();
                Toast.ShowSuccess($"Categoría '{c.Name}' eliminada.");
            }
            else
            {
                Toast.ShowError("Error al eliminar categoría.");
            }
        }
    }

    private async Task OnModalSaved()
    {
        await LoadData();
        Toast.ShowSuccess("Categoría guardada exitosamente.");
    }
}