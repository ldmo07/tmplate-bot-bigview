using Bigview.Bot.Core.Components.Languages.Models;
using Bigview.Bot.Core.Components.Storage;
using Bigview.Bot.Core.Components.Storage.Models;
using Bigview.Bot.Core.Dialogs;
using Bigview.Bot.Core.Dialogs.MenuDialog;
using Bigview.Bot.Core.Dialogs.MenuDialog.Services;
using Bigview.Bot.Core.Dialogs.StudentDialog;
using Bigview.Bot.Core.Dialogs.WelcomeDialog;

using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bigview.Bot.Core.Middleware;

public static class ConfigurationServices
{

    public static IServiceCollection AddDialogs(this IServiceCollection services)
    {
        // Agrega el diálogo de conversación de estudiantes a los servicios como un objeto Singleton.
        services.AddSingleton<StudentConversationalDialogs>();

        // Agrega el diálogo de documentos de estudiantes a los servicios como un objeto Transient.
        services.AddSingleton<DocumentStudentDialog>();

        services.AddSingleton<MenuDialog>();

        // Agrega el diálogo principal (MainDialog) que será ejecutado por el bot como un objeto Singleton.
        services.AddSingleton<MainDialog>();

        // Crea el bot como un objeto Transient (en este caso el controlador ASP espera un objeto IBot).
        services.AddTransient<IBot, WelcomeBot<MainDialog>>();

        return services;
    }

    public static IServiceCollection AddHttpServicesSupport(this IServiceCollection services)
    {
        // Agrega un cliente HTTP a los servicios.
        services.AddHttpClient();

        services.AddHttpClient("student", (http) =>
        {
            http.BaseAddress = new Uri("https://comunidad.uniminuto.edu/");
        });

        //se registra el servicio de consumo de estudiantes via api
        services.AddSingleton<StudentServiceAdapter>();
        return services;
    }

    public static IServiceCollection AddTelemetryAndStorage(this IServiceCollection services)
    {
        // Construye un objeto ServiceProvider para manejar las dependencias.
        ServiceProvider sp = services.BuildServiceProvider();

        // Obtiene la configuración de la aplicación.
        var configuration = sp.GetRequiredService<IConfiguration>();

        // Crea el almacenamiento que se usará para el estado de usuario y conversación (en este caso se utiliza la memoria para propósitos de prueba).
        services.AddSingleton<IStorage, MemoryStorage>();

        // Crea el estado de usuario (usado en la implementación del diálogo de este bot).
        services.AddSingleton<UserState>();

        // Crea el estado de conversación (usado por el sistema de diálogo en sí).
        services.AddSingleton<ConversationState>();

        // Agrega el cliente de Telemetría de Application Insights.
        services.AddSingleton<TelemetryClient>();

        // Agrega el cliente de Telemetría de Bot.
        services.AddSingleton<IBotTelemetryClient, BotTelemetryClient>();

        StorageAccountParameters storageAccountParameters = new();
        configuration.Bind(nameof(StorageAccountParameters), storageAccountParameters);
        services.AddSingleton(x => storageAccountParameters);

        services.AddSingleton<IStorageServices, StorageServices>();

        // Crea un objeto ConversationalParameters y lo vincula con la sección correspondiente de la configuración de la aplicación.
        ConversationalParameters conversationalParameters = new();
        configuration.Bind(nameof(ConversationalParameters), conversationalParameters);

        // Agrega el objeto ConversationalParameters a los servicios como un objeto Singleton.
        services.AddSingleton(x => conversationalParameters);

        return services;
    }
}
