using Bigview.Bot.Core.Utils;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bigview.Bot.Core.Dialogs.StudentDialog;

public class DocumentStudentDialog : ComponentDialog
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IBotTelemetryClient botTelemetryClient;

    public DocumentStudentDialog(IHttpClientFactory httpClientFactory, IBotTelemetryClient botTelemetryClient)
        : base(nameof(DocumentStudentDialog))
    {
        this.httpClientFactory = httpClientFactory;
        this.botTelemetryClient = botTelemetryClient;

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
        var finishStep = "Un gusto haberte logrado apoyar";
        var messageResponse = MessageFactory.Text(finishStep, finishStep, InputHints.IgnoringInput);

        await stepContext.Context.SendActivityAsync(messageResponse, cancellationToken);

        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var getStudentInformation = "TODO: get api students";
        var messageResponse = MessageFactory.Text(getStudentInformation, getStudentInformation, InputHints.IgnoringInput);
        await stepContext.Context.SendActivityAsync(messageResponse, cancellationToken);

        return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var messageText = stepContext.Options?.ToString() ?? $"por supuesto, requiero tu numero de documento";
        var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

        return await stepContext.BeginDialogAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
    }
}