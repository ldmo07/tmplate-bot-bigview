#nullable disable

using Bigview.Bot.Core.Components.Storage;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using System.Text;
using System.Text.Json;

namespace Bigview.Bot.Core.Utils;

public static class Extensions
{

    // Load attachment from embedded resource.
    public static Attachment CreateAdaptiveCardAttachment(this Type _, string resourceName)
    {
        var cardResourcePath = _.Assembly.GetManifestResourceNames().First(name => name.EndsWith(resourceName));

        using var stream = _.Assembly.GetManifestResourceStream(cardResourcePath);

        using var reader = new StreamReader(stream);
        var adaptiveCard = reader.ReadToEnd();

        return new Attachment()
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = JsonConvert.DeserializeObject(adaptiveCard, new JsonSerializerSettings { MaxDepth = null }),
        };
    }


    public static ComponentDialog GetDialog<T>(this IServiceProvider sp) where T : ComponentDialog
        => (ComponentDialog)sp.GetRequiredService<T>() ?? throw new InvalidOperationException("No se pudo obtener el ServiceProvider de la instancia.");

    public static ComponentDialog GetDialog(this IServiceProvider sp, Dialog dialog)
        => (ComponentDialog)sp.GetRequiredService(dialog.GetType()) ?? throw new InvalidOperationException("No se pudo obtener el ServiceProvider de la instancia.");

    public static async Task SendConverationToStorageAsync(this IStorageServices storageServices, Activity activity)
    {
        string filename = $"{activity.Id}.json";

        // Convert the JSON string to a byte array.
        byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(activity));

        using MemoryStream stream = new(byteArray);
        await storageServices.UploadFile("biubot", filename, stream, "application/json");
    }


    public static Task<T> FromJsonAsync<T>(this string json)
        => Task.FromResult(JsonConvert.DeserializeObject<T>(json));
    public static Task<T> FromJsonAsync<T>(this JsonElement json)
        => FromJsonAsync<T>(json.ToString());


    public static Task<Dialog> RegisterDialogAsync<T>(this IServiceProvider sp, DialogContext dc) where T : ComponentDialog
    {
        var transcriptDialogAsembly = sp.GetDialog<T>();
        dc.Dialogs.Add(transcriptDialogAsembly);

        return Task.FromResult(dc.Dialogs.Find(transcriptDialogAsembly.Id));
    }

    public static Task<Dialog> RegisterDialogAsync(this IServiceProvider sp, Dialog dialog, DialogContext dc)
    {
        var transcriptDialogAsembly = sp.GetDialog(dialog);
        dc.Dialogs.Add(transcriptDialogAsembly);

        return Task.FromResult(dc.Dialogs.Find(transcriptDialogAsembly.Id));
    }

    public static async Task<(bool, DialogTurnResult)> ValidateContinueDialogsAsync(this DialogContext dc, IServiceProvider sp)
    {
        if (BotStateHelper.CurrentDialog != null)
        {
            var _currentDialog = await sp.RegisterDialogAsync(BotStateHelper.CurrentDialog, dc);

            if (_currentDialog != null)
                return (_currentDialog != null, await _currentDialog.ContinueDialogAsync(dc));
            else
                return (true, await _currentDialog.BeginDialogAsync(dc));
        }

        return (false, new DialogTurnResult(DialogTurnStatus.Empty));
    }
}
