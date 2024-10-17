namespace Bigview.Bot.Core.Components.Languages.Models;

using Newtonsoft.Json;

public class ConversationPrediction
{

    [JsonProperty("topIntent")]
    public string TopIntent { get; set; }

    [JsonProperty("projectKind")]
    public ProjectKind ProjectKind { get; set; }

    [JsonProperty("intents")]
    public Intent[] Intents { get; set; }

    [JsonProperty("entities")]
    public Entity[] Entities { get; set; }

    [JsonProperty("answering")]
    public Answering Answering { get; set; }
}


