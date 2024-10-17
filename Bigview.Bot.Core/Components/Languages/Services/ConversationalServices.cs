using Azure.AI.TextAnalytics;

using Bigview.Bot.Core.Components.Languages.Models;
using Bigview.Bot.Core.Properties;

using Microsoft.Bot.Builder.Dialogs;

namespace Bigview.Bot.Core.Components.Languages.Services;

public abstract class ConversationalServices
{
    private readonly TextAnalyticsClient textAnalyticsClient;
    private readonly ConversationsProject conversationsProject;
    internal (Dialog, string) DialogId { get; set; }

    public ConversationalServices(ConversationalParameters conversationalParameters)
    {
        var azureKeyCredentials = new Azure.AzureKeyCredential(conversationalParameters.AzureKeyCredential);
        textAnalyticsClient = new TextAnalyticsClient(conversationalParameters.Endpoint, azureKeyCredentials);
        conversationsProject = new ConversationsProject(conversationalParameters);

    }

    public async virtual Task<Entity> ConversationalHandlerAsync(DialogContext dc, ConversationPrediction basePrediction, CancellationToken cancellationToken)
    {
        var entities = basePrediction.Entities.SingleOrDefault();

        if (!IsIntentConfigured(entities?.Category))
        {
            await dc.Context.SendActivityAsync(Resources.ConversationalHandlerAsyncNotUnderstand, cancellationToken: cancellationToken);
            return default;
        }

        return entities;
    }

    private bool IsIntentConfigured(string entities)
    {
        var validEntities = new[] {
            Resources.DialogsEntityCalificaciones,
            Resources.DialogsEntityCertificaciones
        };

        return !string.IsNullOrWhiteSpace(entities) && validEntities.Contains(entities, StringComparer.OrdinalIgnoreCase);
    }

    public virtual async Task WorkflowHandlerAsync(DialogContext dc, ConversationPrediction basePrediction, CancellationToken cancellationToken)
    {

        Intent targetIntentResult = basePrediction?.Intents?.Where(x => x.ConfidenceScore >= 0.75)
                                                             .OrderByDescending(o => o.ConfidenceScore)
                                                             .FirstOrDefault();

        if (targetIntentResult != null)
            await ConversationalHandlerAsync(dc, basePrediction, cancellationToken);

        //{Answering} Debe ser el nombre configurado en Language Studio como intencion en el proyecto de tipo Workflow
        else if (basePrediction.TopIntent.Equals("Answering"))
        {

            await dc.Context.SendActivityAsync(basePrediction.Answering.Result.Answers.FirstOrDefault()?.Result
                ?? Resources.AnswerQuestionHandlerAsyncNotUnderstand,
                cancellationToken: cancellationToken);

            DialogId = (default, string.Empty);
        }
        else
        {
            await dc.Context.SendActivityAsync(Resources.AnswerQuestionHandlerAsyncNotUnderstand, cancellationToken: cancellationToken);
            DialogId = (default, string.Empty);
        }
    }

    public async Task EvaluateIntentsAsync(DialogContext dc, CancellationToken cancellationToken)
    {
        var message = dc.Context.Activity.Text;

        var detectLanguageResult = await textAnalyticsClient.DetectLanguageAsync(message, DetectLanguageInput.None, cancellationToken);

        if (detectLanguageResult.Value.Iso6391Name != "es")
            await dc.Context.SendActivityAsync(Resources.BotNotUnderstand, cancellationToken: cancellationToken);

        var response = await conversationsProject.AnalyzeConversationAsync(message);

        if (response.ProjectKind is ProjectKind.Workflow)
            await WorkflowHandlerAsync(dc, response, cancellationToken);
        else
            await dc.Context.SendActivityAsync(Resources.BotNotUnderstand, cancellationToken: cancellationToken);

    }
}