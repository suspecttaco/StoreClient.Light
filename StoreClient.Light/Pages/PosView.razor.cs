using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using StoreClient.Light.Utils;
using System.Globalization;

namespace StoreClient.Light.Pages;

public partial class PosView
{
    // Inyección de Dependencias
    [Inject] public ApiService Api { get; set; }
    [Inject] public SocketService Socket { get; set; }
    [Inject] public NavigationManager Nav { get; set; }
    [Inject] public ToastService Toast { get; set; }

    // Datos
    private List<Product> allProducts = new();
    private List<Product> filteredProducts = new();
    private List<Customer> customers = new();
    private List<SaleDetail> cart = new();

    // UI
    private string searchQuery = "";
    private int selectedCustomerId = 0;
    private bool isLoading = true;
    private bool isPaying = false;

    // Ticket
    private bool showReceipt = false;
    private bool isErrorModal = false;
    private string modalTitle = "";
    private string modalMessage = "";

    // Calculos
    private decimal TotalToPay => cart.Sum(x => x.Subtotal);
    
    // Variables de estado
    private bool showCustomerModal = false;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            var t1 = Api.GetListAsync<Product>("products/");
            var t2 = Api.GetListAsync<Customer>("catalogs/customers");

            await Task.WhenAll(t1, t2);

            allProducts = await t1;
            customers = await t2;
            
            customers.Insert(0, new Customer { Id = 0, Name = "Público General" });

            filteredProducts = allProducts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando datos: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    // Busqueda
    private void HandleSearch(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? "";
        FilterProducts();
    }

    private void FilterProducts()
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

    // Carrito
    private void AddToCart(Product product)
    {
        var existingItem = cart.FirstOrDefault(x => x.ProductId == product.Id);

        if (existingItem != null)
        {
            existingItem.Amount++;
            existingItem.Subtotal = existingItem.Amount * existingItem.UnitPrice;
        }
        else
        {
            cart.Add(new SaleDetail
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.SellPrice,
                Amount = 1,
                Subtotal = product.SellPrice
            });
        }
        
        searchQuery = "";
        FilterProducts();
    }

    private void UpdateQuantity(SaleDetail item, ChangeEventArgs e)
    {
        if (decimal.TryParse(e.Value?.ToString(), out decimal qty) && qty > 0)
        {
            item.Amount = qty;
            item.Subtotal = item.Amount * item.UnitPrice;
        }
        else
        {
            item.Amount = 1; 
        }
    }

    private void RemoveFromCart(SaleDetail item)
    {
        cart.Remove(item);
    }

    private void CrearClienteRapido()
    {
        showCustomerModal = true;
    }
    
    // Cobro
    private async Task HandleCheckout()
    {
        if (cart.Count == 0) return;

        isPaying = true;
        StateHasChanged();

        try
        {
            int? customerToSend = (selectedCustomerId > 0) ? selectedCustomerId : null;

            var sale = new Sale
            {
                UserId = SessionManager.Instance.User.Id,
                CustomerId = customerToSend,
                Total = TotalToPay,
                PaymentMethod = "cash",
                Details = cart
            };

            var response = await Api.PostWithResponseAsync<SaleResponse, Sale>("sales/", sale);

            if (response != null && response.SaleId > 0)
            {

                var fullSale = await Api.GetByIdAsync<Sale>($"sales/{response.SaleId}");

                if (fullSale != null)
                {
                    Toast.ShowSuccess("¡Venta registrada correctamente!");

                    modalTitle = "Ticket de Venta";
                    modalMessage = TicketGenerator.GenerateTicketString(fullSale);
                    isErrorModal = false;

                    showReceipt = true;
                }
                
                allProducts = await Api.GetListAsync<Product>("products/");
                FilterProducts();
            }
            else
            {
                // Error controlado
                Toast.ShowError("Error al registrar la venta.");
                modalTitle = "Error";
                modalMessage = "No se pudo registrar la venta. Verifique conexión.";
                isErrorModal = true;
                showReceipt = true;
            }
        }
        catch (Exception ex)
        {
            modalTitle = "Excepción";
            modalMessage = $"Error crítico: {ex.Message}";
            isErrorModal = true;
            showReceipt = true;
        }
        finally
        {
            isPaying = false;
            StateHasChanged();
        }
    }
    
    // Limpieza
    private void OnModalClosed()
    {
        if (!isErrorModal)
        {
            cart.Clear();
            searchQuery = "";
            selectedCustomerId = 0;
            FilterProducts();
            StateHasChanged();
        }

        showReceipt = false;
    }

    private async Task OnCustomerSaved()
    {

        var list = await Api.GetListAsync<Customer>("catalogs/customers");

        list.Insert(0, new Customer { Id = 0, Name = "Público General" });

        customers = list;

        var nuevo = customers.MaxBy(c => c.Id);
        if (nuevo != null) selectedCustomerId = nuevo.Id;

        Toast.ShowSuccess("Cliente nuevo agregado y seleccionado.");
        StateHasChanged();
    }
}