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

    // --- DATOS ---
    private List<Product> allProducts = new();
    private List<Product> filteredProducts = new();
    private List<Customer> customers = new();
    private List<SaleDetail> cart = new();

    // --- ESTADO DE UI ---
    private string searchQuery = "";
    private int selectedCustomerId = 0; // 0 representa "Público General"
    private bool isLoading = true;
    private bool isPaying = false;

    // --- ESTADO DEL MODAL DE TICKET ---
    private bool showReceipt = false;
    private bool isErrorModal = false;
    private string modalTitle = "";
    private string modalMessage = "";

    // --- PROPIEDADES CALCULADAS ---
    private decimal TotalToPay => cart.Sum(x => x.Subtotal);
    
    // Variables de estado
    private bool showCustomerModal = false;

    // =================================================
    // 1. CARGA INICIAL
    // =================================================
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            // Carga paralela para mayor velocidad
            var t1 = Api.GetListAsync<Product>("products/");
            var t2 = Api.GetListAsync<Customer>("catalogs/customers");

            await Task.WhenAll(t1, t2);

            allProducts = await t1;
            customers = await t2;

            // Insertar opción por defecto
            customers.Insert(0, new Customer { Id = 0, Name = "Público General" });

            // Inicializar filtro
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

    // =================================================
    // 2. BÚSQUEDA Y FILTRADO
    // =================================================
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

    // =================================================
    // 3. GESTIÓN DEL CARRITO
    // =================================================
    private void AddToCart(Product product)
    {
        var existingItem = cart.FirstOrDefault(x => x.ProductId == product.Id);

        if (existingItem != null)
        {
            // Si ya existe, sumamos 1
            existingItem.Amount++;
            existingItem.Subtotal = existingItem.Amount * existingItem.UnitPrice;
        }
        else
        {
            // Si no, nuevo renglón
            cart.Add(new SaleDetail
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.SellPrice,
                Amount = 1,
                Subtotal = product.SellPrice
            });
        }

        // Limpiar búsqueda para agilizar el siguiente escaneo
        searchQuery = "";
        FilterProducts();
    }

    private void UpdateQuantity(SaleDetail item, ChangeEventArgs e)
    {
        // Validar que sea número positivo
        if (decimal.TryParse(e.Value?.ToString(), out decimal qty) && qty > 0)
        {
            item.Amount = qty;
            item.Subtotal = item.Amount * item.UnitPrice;
        }
        else
        {
            // Si pone 0 o texto inválido, revertir visualmente o borrar?
            // Por ahora forzamos 1 si es inválido para no romper
            // item.Amount = 1; 
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

    // =================================================
    // 4. COBRO Y GENERACIÓN DE TICKET
    // =================================================
    private async Task HandleCheckout()
    {
        if (cart.Count == 0) return;

        isPaying = true;
        StateHasChanged();

        try
        {
            // Preparar el objeto de venta
            int? customerToSend = (selectedCustomerId > 0) ? selectedCustomerId : null;

            var sale = new Sale
            {
                UserId = SessionManager.Instance.User.Id,
                CustomerId = customerToSend,
                Total = TotalToPay,
                PaymentMethod = "cash",
                Details = cart
            };

            // 1. ENVIAR AL BACKEND (POST)
            var response = await Api.PostWithResponseAsync<SaleResponse, Sale>("sales/", sale);

            if (response != null && response.SaleId > 0)
            {
                // 2. ÉXITO: Obtener datos completos para el ticket (con fechas y folios reales)
                var fullSale = await Api.GetByIdAsync<Sale>($"sales/{response.SaleId}");

                if (fullSale != null)
                {
                    Toast.ShowSuccess("¡Venta registrada correctamente!");
                    // Generar texto del ticket
                    modalTitle = "Ticket de Venta";
                    modalMessage = TicketGenerator.GenerateTicketString(fullSale);
                    isErrorModal = false;
                    
                    // MOSTRAR EL MODAL
                    showReceipt = true;
                }
                
                // Recargar productos en segundo plano (para actualizar stock visualmente)
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

    // =================================================
    // 5. CIERRE DEL MODAL (Limpieza)
    // =================================================
    private void OnModalClosed()
    {
        // Solo limpiamos el carrito si fue una venta exitosa
        if (!isErrorModal)
        {
            cart.Clear();
            searchQuery = "";
            selectedCustomerId = 0; // Resetear cliente a Público General
            FilterProducts();
            StateHasChanged();
        }
        
        // Cerrar modal
        showReceipt = false;
    }
    
    // Callback cuando se guarda exitosamente
    private async Task OnCustomerSaved()
    {
        // Recargar la lista de clientes para que aparezca el nuevo
        var list = await Api.GetListAsync<Customer>("catalogs/customers");

        // Mantener el "Público General" al inicio
        list.Insert(0, new Customer { Id = 0, Name = "Público General" });

        customers = list;

        // Opcional: Seleccionar el último creado (lógica simple: el de mayor ID)
        var nuevo = customers.MaxBy(c => c.Id);
        if (nuevo != null) selectedCustomerId = nuevo.Id;

        Toast.ShowSuccess("Cliente nuevo agregado y seleccionado.");
        StateHasChanged();
    }
}