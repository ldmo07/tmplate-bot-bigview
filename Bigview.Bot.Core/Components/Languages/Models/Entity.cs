namespace Bigview.Bot.Core.Components.Languages.Models;

using Newtonsoft.Json;

public class Entity
{
    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("offset")]
    public long Offset { get; set; }

    [JsonProperty("length")]
    public long Length { get; set; }

    [JsonProperty("confidenceScore")]
    public float ConfidenceScore { get; set; }
}


