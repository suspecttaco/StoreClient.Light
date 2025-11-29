namespace StoreClient.Light.Services;

// Tipos de mensaje
public enum ToastLevel
{
    Success, // Verde (Éxito)
    Error,   // Rojo (Fallo)
    Info,    // Azul (Info general)
    Warning  // Amarillo (Cuidado)
}

public class ToastService
{
    public event Action<string, ToastLevel>? OnShow;
    
    public void ShowSuccess(string message) => Show(message, ToastLevel.Success);
    public void ShowError(string message)   => Show(message, ToastLevel.Error);
    public void ShowInfo(string message)    => Show(message, ToastLevel.Info);
    public void ShowWarning(string message) => Show(message, ToastLevel.Warning);

    private void Show(string message, ToastLevel level)
    {
        OnShow?.Invoke(message, level);
    }
}