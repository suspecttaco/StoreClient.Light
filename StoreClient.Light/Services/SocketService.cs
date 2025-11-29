
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

        var options = new SocketIOOptions
        {
            EIO = EngineIO.V4, // Forzar Engine.IO v4
            Transport = TransportProtocol.WebSocket, // Forzar WebSocket
            Reconnection = true,
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
            await _client.EmitAsync("join_inventory", new { user = "csharp_client" });
        };
        
        // Actualizacion de inventario
        _client.On("inventory_updated", response =>
        {
            OnInventoryUpdated?.Invoke(response.GetValue<object>());
        });
        
        // Alertas de stock bajo
        _client.On("low_stock_alert", response =>
        {
            try 
            {
                var data = response.GetValue<AlertPayload>();
                
                if (data != null && data.Products != null && data.Products.Count > 0)
                {
                    OnLowStockAlert?.Invoke(data);
                }
            }
            catch
            {
                // ignored
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