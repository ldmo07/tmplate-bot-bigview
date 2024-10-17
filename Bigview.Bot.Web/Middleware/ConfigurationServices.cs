using Bigview.Bot.Core.Exceptions;
using Bigview.Bot.Core.Middleware;

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bigview.Bot.Web.Middleware;

public static class ConfigurationServices
{

    public static IServiceCollection AddBasicBotServices(this IServiceCollection services)
    {
        // Construye un objeto ServiceProvider para manejar las dependencias.
        ServiceProvider sp = services.BuildServiceProvider();

        // Obtiene la configuración de la aplicación.
        var configuration = sp.GetRequiredService<IConfiguration>();

        // Agrega soporte para acceso a los servicios web.
        services.AddHttpServicesSupport();

        // Agrega un cliente HTTP, controladores y opciones de serialización JSON a los servicios.
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            // Establece la profundidad máxima de serialización para las respuestas HTTP de los bots.
            options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth;
        });

        // Agrega una autenticación de Bot Framework que se utilizará con el adaptador de Bot.
        services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

        // Crea el adaptador de Bot con manejo de errores habilitado.
        services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

        // Agrega soporte para almacenamiento y telemetría.
        services.AddTelemetryAndStorage();

        // Agrega los diálogos a los servicios.
        services.AddDialogs();

        return services;
    }

}
