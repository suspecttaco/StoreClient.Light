
using SocketIO.Core;
using SocketIOClient;
using SocketIOClient.Transport;
using SocketIOClient.Newtonsoft.Json;
using StoreClient.Light.Models;
using StoreClient.Light.Utils;


namespace StoreClient.Light.Services;

public class SocketService
{
    private SocketIOClient.SocketIO _client;
    
    // Eventos
    public event Action<object> OnInventoryUpdated;
    public event Action<AlertPayload> OnLowStockAlert;
    public event Action<ResolvedAlertPayload> OnStockResolved;

    public SocketService()
    {
        string url = AppConfig.BaseUrl.Replace("/api/", "");

        // CONFIGURACIÓN ROBUSTA
        var options = new SocketIOOptions
        {
            EIO = EngineIO.V4, // Forzar Engine.IO v4 (Vital para Flask moderno)
            Transport = TransportProtocol.WebSocket, // Forzar WebSocket (evita Polling)
            Reconnection = true, // Reintentar si se cae
            ReconnectionDelay = 1000,
            ReconnectionAttempts = 5
        };
        
        _client = new SocketIOClient.SocketIO(url, options);
        
        SetEvents();
    }
    
    private void SetEvents()
    {
        // Conexion
        _client.OnConnected += async (sender, e) =>
        {
            // WIP
            await _client.EmitAsync("join_inventory", new { user = "csharp_client" });
        };
        
        // Actualizacion de inventario (Ventas)
        _client.On("inventory_updated", response =>
        {
            OnInventoryUpdated?.Invoke(response.GetValue<object>());
        });
        
        // Alertas de stock bajo
        _client.On("low_stock_alert", response =>
        {
            try 
            {
                // Deserialización automática al modelo
                var data = response.GetValue<AlertPayload>();
                
                if (data != null && data.Products != null && data.Products.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Alerta recibida: {data.Products.Count} productos");
                    
                    // Disparamos el evento con los datos limpios
                    OnLowStockAlert?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔥 Error leyendo alerta: {ex.Message}");
            }
        });
        
        _client.On("stock_alert_resolved", response =>
        {
            try 
            {
                var data = response.GetValue<ResolvedAlertPayload>();
                if (data != null && data.Products.Count > 0)
                {
                    OnStockResolved?.Invoke(data);
                }
            }
            catch (Exception ex) { /* Log */ }
        });
    }

    public async Task ConnectAsync()
    {
        if (_client.Connected) return;
        
        try
        {
            await _client.ConnectAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error conectando socket: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        await _client.DisconnectAsync();
    }
}