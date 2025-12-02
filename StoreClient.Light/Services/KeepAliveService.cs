namespace StoreClient.Light.Services;

public class KeepAliveService : IDisposable
{
    private readonly ApiService _api;
    private readonly SocketService _socket;
    
    // Timer para el bucle infinito
    private PeriodicTimer? _timer;
    private Task? _timerTask;
    private readonly CancellationTokenSource _cts = new();
    
    // Permite que solo 1 hilo entre a la zona crítica a la vez
    private readonly SemaphoreSlim _syncLock = new(1, 1);

    public KeepAliveService(ApiService api, SocketService socket)
    {
        _api = api;
        _socket = socket;
    }

    public void Start()
    {
        if (_timer != null) return; // Ya está corriendo

        // Configurar intervalo: 5 minutos
        _timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        
        // Iniciar el hilo en segundo plano
        _timerTask = RunKeepAliveLoop();
        
        Console.WriteLine("KeepAlive Service: INICIADO");
    }

    private async Task RunKeepAliveLoop()
    {
        try
        {
            // Bucle infinito mientras el timer exista y no se cancele
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                await PerformHealthCheck();
            }
        }
        catch (OperationCanceledException)
        {
            // Apagado normal
        }
    }

    // ZONA CRÍTICA SINCRONIZADA
    private async Task PerformHealthCheck()
    {
        // Semaforo
        await _syncLock.WaitAsync();

        try
        {
            Console.WriteLine("KeepAlive: Verificando signos vitales...");
            
            try 
            {

                await _api.GetListAsync<object>("health"); 
                Console.WriteLine("HTTP Ping: OK");
            }
            catch 
            {
                Console.WriteLine("⚠️ HTTP Ping: Falló (El servidor podría estar despertando)");
            }

            // Verificacion de Socket
            if (!_socket.IsConnected)
            {
                Console.WriteLine("Socket desconectado. Intentando reconexión forzada...");
                await _socket.ConnectAsync();
            }
            else
            {
                Console.WriteLine("Socket: OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en KeepAlive: {ex.Message}");
        }
        finally
        {
            _syncLock.Release();
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer?.Dispose();
        _syncLock.Dispose();
    }
}