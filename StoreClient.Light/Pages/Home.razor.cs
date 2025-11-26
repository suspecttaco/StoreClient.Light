using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Utils;
using StoreClient.Light.Services;

// Asegúrate de importar donde tengas tus vistas (DashboardView, etc)
// Si están en el mismo namespace (Pages), no es necesario el using extra.

namespace StoreClient.Light.Pages;

public partial class Home : IDisposable
{
    // Inyección de dependencias
    [Inject] public NavigationManager Nav { get; set; }
    [Inject] public SocketService Socket { get; set; }
    [Inject] public NotificationService NotifService { get; set; }
    [Inject] public ToastService Toast { get; set; }

    // Estado de la Navegación Principal
    private Type activeComponent = typeof(DashboardView);
    private string currentTitle = "Dashboard General";

    // Estado de los Menús Colapsables (Acordeones)
    private bool expandOperations = false;
    private bool expandAdmin = false;
    
    private string userName = "Cargando...";
    private string userRole = "Usuario";
    private string userInitial = "U";

    // Al iniciar, verificamos seguridad
    protected override async Task OnInitializedAsync()
    {
        if (!SessionManager.Instance.IsLoggedIn)
        {
            Nav.NavigateTo("/login");
            return; // IMPORTANTE: Detener ejecución si no hay login
        }
        
        var u = SessionManager.Instance.User;
        if (u != null)
        {
            userName = u.FullName ?? u.Username;
            userRole = u.Role;
            if (!string.IsNullOrEmpty(userName)) 
                userInitial = userName.Substring(0, 1).ToUpper();
        }
        
        // 1. Limpieza preventiva (Aunque es difícil que funcione entre instancias, es buena práctica)
        Socket.OnLowStockAlert -= HandleLowStock;
        Socket.OnStockResolved -= HandleStockResolved;
        
        await Socket.ConnectAsync();

        // 2. Suscripción
        Socket.OnLowStockAlert += HandleLowStock;
        Socket.OnStockResolved += HandleStockResolved;
    }
    
    private void HandleStockResolved(ResolvedAlertPayload data)
    {
        int count = data.Products.Count;
        string titulo = "Stock Recuperado";
        string mensaje = "";

        if (count == 1)
            mensaje = $"El producto '{data.Products[0]}' ha recuperado su nivel de stock.";
        else
            mensaje = $"{count} productos han recuperado su nivel de stock.";

        InvokeAsync(() => 
        {
            // Notificación Verde (Success)
            NotifService.AddNotification(titulo, mensaje, "success");
            Toast.ShowSuccess(mensaje);
            StateHasChanged();
        });
    }
    
    private void HandleLowStock(AlertPayload data)
    {
        // 1. Construir el mensaje
        int count = data.Products.Count;
        string titulo = "Stock Crítico";
        string mensaje = "";

        if (count == 1)
        {
            var p = data.Products[0];
            mensaje = $"{p.DisplayName} tiene stock bajo ({p.CurrentStock})";
        }
        else
        {
            mensaje = $"{count} productos requieren reabastecimiento urgente.";
        }

        // 2. Actualizar UI en el hilo principal
        InvokeAsync(() => 
        {
            // A) Agregar al historial de la campana
            NotifService.AddNotification(titulo, mensaje, "warning");
        
            // B) Mostrar el Toast flotante
            Toast.ShowWarning(mensaje);
        
            // C) Refrescar para que aparezca el puntito rojo
            StateHasChanged();
        });
    }
    
    // Cambiar la pantalla central
    private void ShowComponent(Type componentType)
    {
        activeComponent = componentType;

        // Actualizar título del Header
        if (componentType == typeof(DashboardView)) currentTitle = "Dashboard General";
        else if (componentType == typeof(PosView)) currentTitle = "Punto de Venta";
        else if (componentType == typeof(SalesHistoryView)) currentTitle = "Historial de Transacciones";
        else if (componentType == typeof(InventoryView)) currentTitle = "Gestión de Inventario";
        else if (componentType == typeof(CategoryView)) currentTitle = "Catálogo de Categorías";
        else if (componentType == typeof(CustomerView)) currentTitle = "Directorio de Clientes";
        else if (componentType == typeof(SupplierView)) currentTitle = "Directorio de Proveedores";

        StateHasChanged();
    }

    // Estilo visual para saber en qué pantalla estamos
    private string GetActiveClass(Type type)
    {
        return activeComponent == type ? "active" : "";
    }

    // Lógica de los menús desplegables
    private void ToggleMenu(string menu)
    {
        if (menu == "ops") expandOperations = !expandOperations;
        if (menu == "admin") expandAdmin = !expandAdmin;
    }

    private string GetArrowIcon(bool isExpanded) =>
        isExpanded ? "bi-chevron-down" : "bi-chevron-right";

    private void Logout()
    {
        SessionManager.Instance.Logout();
        Nav.NavigateTo("/login");
    }
    
    public void Dispose()
    {
        // Cuando este componente se destruya (ej. al cerrar sesión), dejamos de escuchar
        if (Socket != null)
        {
            Socket.OnLowStockAlert -= HandleLowStock;
        }
    }
}