
using Bigview.Bot.Core.Properties;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Bigview.Bot.Core.Dialogs;
public class DialogBotBase<T> : ActivityHandler
    where T : Dialog
{
    protected readonly Dialog Dialog;
    protected readonly BotState ConversationState;
    protected readonly BotState UserState;
    protected readonly ILogger Logger;

    public DialogBotBase(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBotBase<T>> logger)
    {
        ConversationState = conversationState;
        UserState = userState;
        Dialog = dialog;
        Logger = logger;

    }

    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
    {
        // Espera a que se complete el procesamiento del turno en la conversación.
        await base.OnTurnAsync(turnContext, cancellationToken);

        // Guarda cualquier cambio en el estado de la conversación que pueda haber ocurrido durante el turno.
        await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

        // Guarda cualquier cambio en el estado del usuario que pueda haber ocurrido durante el turno.
        await UserState.SaveChangesAsync(turnContext, false, cancellationToken);

    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        // Registra información en el registro de la aplicación utilizando el objeto Logger. La información registrada es un mensaje de recurso.
        Logger.LogInformation(Resources.LoggerOnMessageActivityAsync);

        // Ejecuta el diálogo especificado utilizando el objeto Dialog y la actividad de mensaje recibida en el contexto actual.
        // ConversationState.CreateProperty crea un objeto que representa el estado del diálogo y lo almacena en la propiedad de estado de la conversación. 
        // El nombre de la propiedad se especifica con nameof(DialogState).
        await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(Resources.PropertyDialogState), cancellationToken);

    }
}
