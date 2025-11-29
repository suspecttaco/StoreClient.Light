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
    
    private LoginModel model = new LoginModel();
    private string errorMessage;
    private bool isLoading = false;

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = "";

        try
        {
            await Task.Delay(300);

            var response = await Api.LoginAsync(model.Username, model.Password);

            if (response.User != null)
            {
                SessionManager.Instance.Login(response.User, response.Token);

                Nav.NavigateTo("/app", forceLoad: true); 
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

    public class LoginModel
    {
        [Required(ErrorMessage = "Ingresa tu usuario.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Ingresa tu contraseña.")]
        public string Password { get; set; }
    }
}