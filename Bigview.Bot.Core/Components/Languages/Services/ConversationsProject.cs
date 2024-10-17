using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;

using Bigview.Bot.Core.Components.Languages.Models;
using Bigview.Bot.Core.Utils;

using System.Text.Json;

namespace Bigview.Bot.Core.Components.Languages.Services;

public class ConversationsProject
{
    private readonly ConversationAnalysisClient conversationAnalysisClient;
    private readonly string deploymentName;
    private readonly string projectName;

    public ConversationsProject(ConversationalParameters conversationalParameters)
    {
        var azureKeyCredentials = new Azure.AzureKeyCredential(conversationalParameters.AzureKeyCredential);
        conversationAnalysisClient = new ConversationAnalysisClient(conversationalParameters.Endpoint, azureKeyCredentials);

        deploymentName = conversationalParameters.DeploymentName;
        projectName = conversationalParameters.ProjectName;

    }

    public async Task<ConversationPrediction> AnalyzeConversationAsync(string activityText)
    {

        var data = new
        {
            analysisInput = new
            {
                conversationItem = new
                {
                    text = activityText,
                    id = "1",
                    participantId = "1",
                }
            },
            parameters = new
            {
                projectName,
                deploymentName,
                stringIndexType = "Utf16CodeUnit",
            },
            kind = "Conversation",
        };

        Response response = await conversationAnalysisClient.AnalyzeConversationAsync(RequestContent.Create(data));

        using JsonDocument result = JsonDocument.Parse(response.ContentStream);
        JsonElement conversationalTaskResult = result.RootElement;
        JsonElement conversationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction");

        ConversationPrediction conversation = new()
        {
            TopIntent = conversationPrediction.GetProperty("topIntent").GetString(),
            ProjectKind = ProjectKind.Workflow
        };

        List<Intent> intents = new();

        //Si la intención es de tipo Answering se debe resolver el objeto de la siguiente manera. 
        if (conversation.TopIntent.Equals("Answering"))
        {
            var getRawAnsweringJson = conversationPrediction.GetProperty("intents").GetProperty(conversation.TopIntent);

            Answering answering = await getRawAnsweringJson.FromJsonAsync<Answering>();

            conversation.Answering = answering;
        }
        else
        {
            var getIntentsArray = conversationPrediction.GetProperty("intents").GetProperty(conversation.TopIntent).GetProperty("result").GetProperty("prediction").GetProperty("intents");

            foreach (JsonElement intent in getIntentsArray.EnumerateArray())
                intents.Add(new Intent
                {
                    Category = intent.GetProperty("category").GetString(),
                    ConfidenceScore = intent.GetProperty("confidenceScore").GetSingle()
                });


            conversation.Intents = intents.ToArray();

            List<Models.Entity> entities = new();
            var getEntitiesArray = conversationPrediction.GetProperty("intents").GetProperty(conversation.TopIntent).GetProperty("result").GetProperty("prediction").GetProperty("entities");
            foreach (JsonElement entity in getEntitiesArray.EnumerateArray())
                entities.Add(new Models.Entity
                {
                    Offset = entity.GetProperty("offset").GetInt32(),
                    Category = entity.GetProperty("category").GetString(),
                    Text = entity.GetProperty("text").GetString(),
                    Length = entity.GetProperty("length").GetInt32(),
                    ConfidenceScore = entity.GetProperty("confidenceScore").GetSingle()
                });


            conversation.Entities = entities.ToArray();
        }
        return conversation;
    }
}