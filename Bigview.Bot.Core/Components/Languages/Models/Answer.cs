namespace Bigview.Bot.Core.Components.Languages.Models;

using Newtonsoft.Json;

public class Answer
{
    [JsonProperty("questions")]
    public string[] Questions { get; set; }

    [JsonProperty("answer")]
    public string Result { get; set; }

    [JsonProperty("confidenceScore")]
    public double ConfidenceScore { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("dialog")]
    public DialogAnswer Dialog { get; set; }
}


