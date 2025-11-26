using StoreClient.Light.Models;

namespace StoreClient.Light.Utils;

public class SessionManager
{
    // Instancia estatica unica (porque si)
    private static SessionManager _instance;
    
    // Propiedad para acceder desde cualquier lado
    public static SessionManager Instance
    {
        get
        {
            if (_instance == null) _instance = new SessionManager();
            return _instance;
        }
    }
    
    // Guardar usuario loggeado
    public User User { get; private set; }
    public string Token { get; private set; }
    
    // Constructor privado (monopolio xd)
    public void Login(User user, string token)
    {
        User = user;
        Token = token;
    }
    
    // Cerrar sesion
    public void Logout()
    {
        User = null;
        Token = null;
    }
    
    // Checar si hay sesion activa
    public bool IsLoggedIn => User != null;
}