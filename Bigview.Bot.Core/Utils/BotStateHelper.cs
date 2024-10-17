using Microsoft.Bot.Builder.Dialogs;

namespace Bigview.Bot.Core.Utils;

public static class BotStateHelper
{
    public static ComponentDialog CurrentDialog { get; set; }
}
