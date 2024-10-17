using Newtonsoft.Json;

namespace Bigview.Bot.Core.Components.Http.OAuth.Model;

public class TokenModel
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonProperty("token_type")]
    public string TokenType { get; set; } = null!;

}
