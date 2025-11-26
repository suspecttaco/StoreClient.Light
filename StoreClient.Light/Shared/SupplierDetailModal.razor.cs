using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using System.ComponentModel.DataAnnotations;

namespace StoreClient.Light.Shared;

public partial class SupplierDetailModal
{
    [Inject] public ApiService Api { get; set; }

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public int? SupplierId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    private SupplierModel model = new();
    private string errorMessage;
    private bool isLoading = false;
    private string title = "Nuevo Proveedor";

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible)
        {
            errorMessage = null;
            isLoading = false;

            if (SupplierId.HasValue && SupplierId > 0)
            {
                title = "Editar Proveedor";
                await CargarDatos(SupplierId.Value);
            }
            else
            {
                title = "Nuevo Proveedor";
                model = new SupplierModel { Active = true };
            }
        }
    }

    private async Task CargarDatos(int id)
    {
        isLoading = true;
        try
        {
            var sup = await Api.GetByIdAsync<Supplier>($"catalogs/suppliers/{id}");
            if (sup != null)
            {
                model = new SupplierModel
                {
                    Name = sup.Name,
                    ContactName = sup.ContactName,
                    Phone = sup.Phone,
                    Email = sup.Email,
                    Address = sup.Address,
                    Active = sup.Active
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
            var data = new Supplier
            {
                Name = model.Name,
                ContactName = model.ContactName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Active = model.Active
            };

            bool success;
            if (SupplierId.HasValue && SupplierId > 0)
                success = await Api.PutAsync($"catalogs/suppliers/{SupplierId}", data);
            else
                success = await Api.PostAsync("catalogs/suppliers", data);

            if (success)
            {
                await Close();
                await OnSaved.InvokeAsync();
            }
            else
            {
                errorMessage = "No se pudo guardar el proveedor.";
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

    public class SupplierModel
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        public string Name { get; set; }
        
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
    }
}