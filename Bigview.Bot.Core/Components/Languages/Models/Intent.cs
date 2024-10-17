namespace Bigview.Bot.Core.Components.Languages.Models;

using Newtonsoft.Json;

public class Intent
{
    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("confidenceScore")]
    public float ConfidenceScore { get; set; }
}


