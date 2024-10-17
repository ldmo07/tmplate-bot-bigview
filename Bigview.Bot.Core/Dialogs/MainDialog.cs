using Bigview.Bot.Core.Components.Storage;
using Bigview.Bot.Core.Dialogs.StudentDialog;
using Bigview.Bot.Core.Properties;
using Bigview.Bot.Core.Utils;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Bigview.Bot.Core.Dialogs;

public class MainDialog : ComponentDialog
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IBotTelemetryClient botTelemetryClient;
    private readonly IStorageServices storageServices;
    private readonly UserState userState;
    private readonly StudentConversationalDialogs studentConversationalDialogs;

    public MainDialog(IHttpClientFactory httpClientFactory,
                      IServiceProvider serviceProvider,
                      IBotTelemetryClient botTelemetryClient,
                      IStorageServices storageServices,
                      UserState userState,
                      StudentConversationalDialogs studentConversationalDialogs)
        : base(nameof(MainDialog))
    {
        this.httpClientFactory = httpClientFactory;
        this.serviceProvider = serviceProvider;
        this.botTelemetryClient = botTelemetryClient;
        this.storageServices = storageServices;
        this.userState = userState;
        this.studentConversationalDialogs = studentConversationalDialogs;
    }


    public override async Task<DialogTurnResult> ContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default)
    {
        //Valida si el dialogo a ejecutar sí esta registrado en el objeto DialogSet del dialogo principal
        (bool, DialogTurnResult) result = await dc.ValidateContinueDialogsAsync(serviceProvider);

        // Se verifica si se encontró un diálogo correspondiente.
        if (result.Item1)
            // Se continua con el diálogo correspondiente.
            return result.Item2;

        return await base.ContinueDialogAsync(dc, cancellationToken);
    }

    public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
    {
        // permite almacenar la conversacion 
        await storageServices.SendConverationToStorageAsync(dc.Context.Activity);

        // Se verifica si la actividad actual es un mensaje.
        if (dc.Context.Activity.Type == ActivityTypes.Message)
        {
            //Valida eventos de Menu 
            if (dc.Context.Activity.Value != null)
            {
                string activityValue = dc.Context.Activity.Value.ToString();

                if (activityValue.Contains("firstcard"))
                {
                    await ShowMenuCard(Resources.CardsFirtsCard, dc.Context, cancellationToken);
                    //return new DialogTurnResult(DialogTurnStatus.Waiting);//este codigo tenia un bug con la interraccion de los botones
                    return await dc.ContinueDialogAsync();//este codigo resuelve el bug de los botones
                }
                else if (activityValue.Contains("secondcard"))
                {
                    await ShowMenuCard(Resources.CardsSecondCard, dc.Context, cancellationToken);
                    //return new DialogTurnResult(DialogTurnStatus.Waiting);//este codigo tenia un bug con la interraccion de los botones
                    return await dc.ContinueDialogAsync();//este codigo resuelve el bug de los botones
                }
                else if (activityValue.Contains("MenuDialog"))
                {
                    var transcriptDialogAsembly = serviceProvider.GetDialog<MenuDialog.MenuDialog>();
                    dc.Dialogs.Add(transcriptDialogAsembly);
                    return await transcriptDialogAsembly.BeginDialogAsync(dc, cancellationToken: cancellationToken);
                }
            }

            // Se llama a un método que evalúa las intenciones en el mensaje y actualiza el diálogo correspondiente en el objeto de conversación.
            await studentConversationalDialogs.EvaluateIntentsAsync(dc, cancellationToken);

            if (studentConversationalDialogs.DialogId.Item1 != null)
            {
                //Verifica si se encontró un diálogo correspondiente.
                var currendDialogId = studentConversationalDialogs.DialogId.Item1;

                //Se agrega el diálogo al DialogSet del dialogo principal
                Dialog dialog = await serviceProvider.RegisterDialogAsync(currendDialogId, dc);

                return await dialog.BeginDialogAsync(dc, cancellationToken: cancellationToken);
            }
        }

        // Se finaliza el diálogo actual.
        return await dc.EndDialogAsync(cancellationToken: cancellationToken);
    }


    // Método auxiliar para mostrar una tarjeta de menú
    async Task ShowMenuCard(string cardResource, ITurnContext context, CancellationToken token)
    {
        var menuCard = GetType().CreateAdaptiveCardAttachment(cardResource);
        var activeCardMenu = MessageFactory.Attachment(menuCard);
        await context.SendActivityAsync(activeCardMenu, token);
    }
}