namespace Bigview.Bot.Core.Components.Languages.Models;

using Newtonsoft.Json;

public class Result
{
    [JsonProperty("answers")]
    public Answer[] Answers { get; set; }
}


