using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;
using StoreClient.Light.Utils; // Para SessionManager

namespace StoreClient.Light.Pages;

public partial class Login
{
    [Inject] public ApiService Api { get; set; }
    [Inject] public NavigationManager Nav { get; set; }

    // Modelo exclusivo para el formulario
    private LoginModel model = new LoginModel();
    private string errorMessage;
    private bool isLoading = false;

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = "";

        try
        {
            // Simulamos carga mínima para UX
            await Task.Delay(300);

            var response = await Api.LoginAsync(model.Username, model.Password);

            if (response.User != null)
            {
                // 1. Guardar Sesión (Si tienes SessionManager singleton)
                SessionManager.Instance.Login(response.User, response.Token);
                
                // 2. Redirigir a la Raíz (Home)
                // El 'true' fuerza recarga completa, útil para limpiar errores de Photino
                Nav.NavigateTo("/", forceLoad: true); 
            }
            else
            {
                errorMessage = response.Error ?? "Credenciales incorrectas.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error de conexión: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    // Clase interna para validación
    public class LoginModel
    {
        [Required(ErrorMessage = "Ingresa tu usuario.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Ingresa tu contraseña.")]
        public string Password { get; set; }
    }
}