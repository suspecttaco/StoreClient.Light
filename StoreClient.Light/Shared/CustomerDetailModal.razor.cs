using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using System.ComponentModel.DataAnnotations;

namespace StoreClient.Light.Shared;

public partial class CustomerDetailModal
{
    [Inject] public ApiService Api { get; set; }

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public int? CustomerId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    private CustomerModel model = new();
    private string errorMessage;
    private bool isLoading = false;
    private string title = "Nuevo Cliente";

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible)
        {
            errorMessage = null;
            isLoading = false;

            if (CustomerId.HasValue && CustomerId > 0)
            {
                title = "Editar Cliente";
                await CargarDatos(CustomerId.Value);
            }
            else
            {
                title = "Nuevo Cliente";
                // Por defecto, uno nuevo nace activo
                model = new CustomerModel { Active = true };
            }
        }
    }

    private async Task CargarDatos(int id)
    {
        isLoading = true;
        try
        {
            var customer = await Api.GetByIdAsync<Customer>($"catalogs/customers/{id}");
            if (customer != null)
            {
                model = new CustomerModel
                {
                    Name = customer.Name,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    CreditLimit = customer.CreditLimit,
                    Active = customer.Active // <--- CARGAMOS EL ESTADO REAL
                };
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error cargando: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task HandleSave()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var customerData = new Customer
            {
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                CreditLimit = model.CreditLimit,
                Active = model.Active // <--- GUARDAMOS LO QUE ELIJA EL USUARIO
            };

            bool success;
            if (CustomerId.HasValue && CustomerId > 0)
            {
                success = await Api.PutAsync($"catalogs/customers/{CustomerId}", customerData);
            }
            else
            {
                success = await Api.PostAsync("catalogs/customers", customerData);
            }

            if (success)
            {
                await Close();
                await OnSaved.InvokeAsync();
            }
            else
            {
                errorMessage = "No se pudo guardar.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task Close()
    {
        IsVisible = false;
        await IsVisibleChanged.InvokeAsync(false);
    }

    public class CustomerModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }
        
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal CreditLimit { get; set; }
        
        // Agregamos la propiedad al modelo del formulario
        public bool Active { get; set; } 
    }
}