using Microsoft.Extensions.Configuration;

namespace StoreClient.Light.Utils;

public static class AppConfig
{
    private static IConfiguration _configuration;

    static AppConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();
    }

    public static string BaseUrl
    {
        get
        {
            // Local o Remoto
            var userRemote = bool.Parse(_configuration["ApiSettings:UseRemote"] ?? "false");

            if (userRemote)
            {
                return _configuration["RemoteSettings:BaseUrl"];
            }

            return _configuration["ApiSettings:BaseUrl"];
        }
    }
}