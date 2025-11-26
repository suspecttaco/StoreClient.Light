using Microsoft.AspNetCore.Components;
using StoreClient.Light.Models;
using StoreClient.Light.Services;

namespace StoreClient.Light.Pages;

public partial class DashboardView
{
    [Inject] public ApiService Api { get; set; }

    private DashboardStats stats;
    private bool isLoading = true;
    private string errorMessage = null;

    protected override async Task OnInitializedAsync()
    {
        await CargarDatos();
    }

    private async Task CargarDatos()
    {
        isLoading = true;
        errorMessage = null;
        StateHasChanged(); // Forzar renderizado de loader

        try
        {
            // Usamos el endpoint que ya probamos en el backend
            stats = await Api.GetByIdAsync<DashboardStats>("dashboard/stats");

            if (stats == null)
            {
                errorMessage = "No se pudieron cargar los datos del servidor.";
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
}