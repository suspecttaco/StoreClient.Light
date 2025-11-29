using StoreClient.Light.Models;

namespace StoreClient.Light.Services;

public class NotificationService
{
    // Lista en memoria
    public List<AppNotification> Notifications { get; private set; } = new();
    
    // Evento para actualizar la UI
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

        Notifications.Insert(0, note);

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