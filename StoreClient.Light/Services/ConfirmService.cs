namespace StoreClient.Light.Services;

public class ConfirmService
{
    // Evento
    public event Action<string, string, string, string, bool>? OnShow;
    // Promesa
    private TaskCompletionSource<bool>? _taskCompletionSource;

    public Task<bool> Show(
        string title,
        string message,
        string btnComfirmText = "Confirmar",
        string btnCancelText = "Cancelar",
        bool isDanger = false)
    {
        _taskCompletionSource = new TaskCompletionSource<bool>();
        
        // Disparador
        OnShow?.Invoke(title, message, btnComfirmText, btnCancelText, isDanger);

        return _taskCompletionSource.Task;
    }
    
    // Llamada al evento
    public void Respond(bool confirmed)
    {
        _taskCompletionSource?.TrySetResult(confirmed);
    }
}