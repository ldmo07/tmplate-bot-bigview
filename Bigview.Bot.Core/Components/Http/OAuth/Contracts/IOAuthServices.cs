using Bigview.Bot.Core.Components.Http.OAuth.Model;

namespace Bigview.Bot.Core.Components.Http.OAuth.Contracts;

public interface IOAuthServices
{
    Task<TokenModel> GetAccessTokenAsync();
}