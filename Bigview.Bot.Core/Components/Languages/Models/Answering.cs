namespace Bigview.Bot.Core.Components.Languages.Models;

using Newtonsoft.Json;

public class Answering
{
    [JsonProperty("confidenceScore")]
    public double ConfidenceScore { get; set; }

    [JsonProperty("targetProjectKind")]
    public string TargetProjectKind { get; set; }

    [JsonProperty("result")]
    public Result Result { get; set; }
}


