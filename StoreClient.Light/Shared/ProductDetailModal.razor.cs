using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using System.ComponentModel.DataAnnotations;

namespace StoreClient.Light.Shared;

public partial class ProductDetailModal
{
    [Inject] public ApiService Api { get; set; }

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public int? ProductId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    private ProductModel model = new();
    private List<Category> categories = new();
    
    private string errorMessage;
    private bool isLoading = false;
    private string title = "Nuevo Producto";

    private List<SelectorOption> categoryOptions = new();

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible)
        {
            errorMessage = null;
            isLoading = false;
            await CargarCategorias();

            if (ProductId.HasValue && ProductId > 0)
            {
                title = "Editar Producto";
                await CargarProducto(ProductId.Value);
            }
            else
            {
                title = "Nuevo Producto";
                model = new ProductModel { Active = true, Stock = 0, MinStock = 5 };
            }
        }
    }

    private async Task CargarCategorias()
    {
        var cats = await Api.GetListAsync<Category>("catalogs/categories");
        
        categoryOptions = cats.Select(c => new SelectorOption 
        {
            Id = c.Id,
            Text = c.Name,
            Subtitle = c.Description
        }).ToList();
    }

    private async Task CargarProducto(int id)
    {
        isLoading = true;
        try
        {
            var p = await Api.GetByIdAsync<Product>($"products/{id}");
            if (p != null)
            {
                model = new ProductModel
                {
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    BuyPrice = p.BuyPrice,
                    SellPrice = p.SellPrice,
                    Stock = p.Stock,
                    MinStock = p.MinStock,
                    Active = p.Active
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
            var productData = new Product
            {
                Code = model.Code,
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                BuyPrice = model.BuyPrice,
                SellPrice = model.SellPrice,
                Stock = model.Stock,
                MinStock = model.MinStock,
                Active = model.Active
            };

            bool success;
            if (ProductId.HasValue && ProductId > 0)
                success = await Api.PutAsync($"products/{ProductId}", productData);
            else
                success = await Api.PostAsync("products/", productData);

            if (success)
            {
                await Close();
                await OnSaved.InvokeAsync();
            }
            else
            {
                errorMessage = "No se pudo guardar el producto.";
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

    // Modelo con validaciones
    public class ProductModel
    {
        public string Code { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public decimal BuyPrice { get; set; }
        
        [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal SellPrice { get; set; }
        
        public decimal Stock { get; set; }
        public decimal MinStock { get; set; }
        public bool Active { get; set; }
    }
}