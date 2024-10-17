using Bigview.Bot.Core.Dialogs.MenuDialog.Models;
using Bigview.Bot.Core.Dialogs.MenuDialog.Services;
using Bigview.Bot.Core.Properties;
using Bigview.Bot.Core.Utils;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bigview.Bot.Core.Dialogs.MenuDialog;

public class MenuDialog : ComponentDialog
{

    private readonly IBotTelemetryClient botTelemetryClient;
    private readonly StudentServiceAdapter studentServiceAdapter;

    public MenuDialog(IBotTelemetryClient botTelemetryClient, StudentServiceAdapter studentServiceAdapter)
        : base(nameof(MenuDialog))
    {

        this.botTelemetryClient = botTelemetryClient;
        this.studentServiceAdapter = studentServiceAdapter;
        AddDialog(new TextPrompt(nameof(TextPrompt)));

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
          {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
          }));

        InitialDialogId = nameof(WaterfallDialog);

        //registrar el dialogo actual
        BotStateHelper.CurrentDialog = this;
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var finishStep = Resources.TranscriptDialogEndMessage;
        var messageResponse = MessageFactory.Text(finishStep, finishStep, InputHints.IgnoringInput);

        await stepContext.Context.SendActivityAsync(messageResponse, cancellationToken);

        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        //TODO: Validar si el codigo del estudiante es valido
        if (stepContext.Result is null && string.IsNullOrEmpty(stepContext.Result.ToString()))
        {
            var errorMessageResponse = MessageFactory.Text(Resources.TranscriptDialogInvalidCode, Resources.TranscriptDialogInvalidCode, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(errorMessageResponse, cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        var studentCode = stepContext.Result.ToString(); // "000209733"

        StudentMenuModel student = await studentServiceAdapter.GetStudentAsync(studentCode);

        var getStudentInformation = string.Format(Resources.MenuDialogStudentFount, student.Nombre, student.NotaFinal);
        var messageResponse = MessageFactory.Text(getStudentInformation, getStudentInformation, InputHints.IgnoringInput);
        await stepContext.Context.SendActivityAsync(messageResponse, cancellationToken);

        return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var messageText = stepContext.Options?.ToString() ?? Resources.TranscriptDialogInitialMessage;
        var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);

    }
}