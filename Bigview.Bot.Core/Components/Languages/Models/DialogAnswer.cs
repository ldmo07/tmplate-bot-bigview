namespace Bigview.Bot.Core.Components.Languages.Models;

using Newtonsoft.Json;

public class DialogAnswer
{
    [JsonProperty("isContextOnly")]
    public bool IsContextOnly { get; set; }

    [JsonProperty("prompts")]
    public object[] Prompts { get; set; }
}


