using Photino.Blazor;
using Microsoft.Extensions.DependencyInjection;
using StoreClient.Light.Services;

namespace StoreClient.Light;
class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        
        // Inyectar dependencias
        appBuilder.Services.AddLogging();
        
        // Registrar servicios migrados
        appBuilder.Services.AddSingleton<ApiService>();
        appBuilder.Services.AddSingleton<SocketService>();
        appBuilder.Services.AddSingleton<ToastService>();
        appBuilder.Services.AddSingleton<NotificationService>();
        
        // Registrar pagina maestra 
        appBuilder.RootComponents.Add<App>("#app");
        var app = appBuilder.Build();
        
        // Configurar ventana nativa
        app.MainWindow
            .SetTitle("Sistema de Punto de Venta para Abarrotes pequeños")
            .SetSize(1024, 768)
            .Center().SetMaximized(true);
        
        // Correr
        app.Run();
    }
}