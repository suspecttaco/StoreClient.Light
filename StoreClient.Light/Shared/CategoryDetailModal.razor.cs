using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using System.ComponentModel.DataAnnotations;

namespace StoreClient.Light.Shared;

public partial class CategoryDetailModal
{
    [Inject] public ApiService Api { get; set; }

    // Parámetros
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public int? CategoryId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    private CategoryModel model = new();
    private string errorMessage;
    private bool isLoading = false;
    private string title = "Nueva Categoría";

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible)
        {
            errorMessage = null;
            isLoading = false;

            if (CategoryId.HasValue && CategoryId > 0)
            {
                title = "Editar Categoría";
                await CargarDatos(CategoryId.Value);
            }
            else
            {
                title = "Nueva Categoría";
                model = new CategoryModel { Active = true };
            }
        }
    }

    private async Task CargarDatos(int id)
    {
        isLoading = true;
        try
        {
            var cat = await Api.GetByIdAsync<Category>($"catalogs/categories/{id}");
            if (cat != null)
            {
                model = new CategoryModel
                {
                    Name = cat.Name,
                    Description = cat.Description,
                    Active = cat.Active
                };
            }
        }
        catch (Exception ex) { errorMessage = $"Error: {ex.Message}"; }
        finally { isLoading = false; }
    }

    private async Task HandleSave()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var data = new Category
            {
                Name = model.Name,
                Description = model.Description,
                Active = model.Active
            };

            bool success;
            if (CategoryId.HasValue && CategoryId > 0)
                success = await Api.PutAsync($"catalogs/categories/{CategoryId}", data);
            else
                success = await Api.PostAsync("catalogs/categories", data);

            if (success)
            {
                await Close();
                await OnSaved.InvokeAsync();
            }
            else
            {
                errorMessage = "No se pudo guardar la categoría.";
            }
        }
        catch (Exception ex) { errorMessage = $"Error: {ex.Message}"; }
        finally { isLoading = false; }
    }

    private async Task Close()
    {
        IsVisible = false;
        await IsVisibleChanged.InvokeAsync(false);
    }

    public class CategoryModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}