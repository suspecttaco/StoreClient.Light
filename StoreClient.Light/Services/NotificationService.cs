using StoreClient.Light.Models;

namespace StoreClient.Light.Services;

public class NotificationService
{
    // Lista en memoria (Historial)
    public List<AppNotification> Notifications { get; private set; } = new();
    
    // Evento para actualizar la UI (La campana)
    public event Action? OnChange;

    public void AddNotification(string title, string message, string type = "info")
    {
        var note = new AppNotification
        {
            Title = title,
            Message = message,
            Type = type,
            Timestamp = DateTime.Now
        };

        // Agregamos al inicio de la lista (LIFO)
        Notifications.Insert(0, note);
        
        // Limitamos el historial a las últimas 50 para no saturar memoria
        if (Notifications.Count > 50) Notifications.RemoveAt(Notifications.Count - 1);

        NotifyStateChanged();
    }

    public void MarkAllAsRead()
    {
        foreach (var n in Notifications) n.IsRead = true;
        NotifyStateChanged();
    }

    public int UnreadCount => Notifications.Count(n => !n.IsRead);

    private void NotifyStateChanged() => OnChange?.Invoke();
}