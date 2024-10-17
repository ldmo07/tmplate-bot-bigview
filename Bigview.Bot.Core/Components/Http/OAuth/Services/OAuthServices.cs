using Bigview.Bot.Core.Components.Http.OAuth.Contracts;
using Bigview.Bot.Core.Components.Http.OAuth.Model;

using Newtonsoft.Json;

namespace Bigview.Bot.Core.Components.Http.OAuth.Services;

public class OAuthServices : IOAuthServices
{
    private readonly HttpClient http;
    private readonly TokenParametersModel tokenParameters;

    public OAuthServices(IHttpClientFactory clientFactory, TokenParametersModel tokenParameters)
    {
        http = clientFactory.CreateClient("bv-oAuth");
        this.tokenParameters = tokenParameters;
    }

    public async Task<TokenModel> GetAccessTokenAsync()
    {

        Dictionary<string, string> keyValues = new();

        keyValues.TryAdd("appid", tokenParameters.AppId);
        keyValues.TryAdd("secrect", tokenParameters.SecrectId);

        FormUrlEncodedContent urlEncodedContent = new(keyValues);

        var response = await http.PostAsync("security/token", urlEncodedContent);

        response.EnsureSuccessStatusCode();

        string tokenModelResponse = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(tokenModelResponse))
            throw new ArgumentException("token expired");

        return JsonConvert.DeserializeObject<TokenModel>(tokenModelResponse);
    }
}
