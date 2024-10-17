using Bigview.Bot.Core.Properties;
using Bigview.Bot.Core.Utils;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Bigview.Bot.Core.Dialogs.WelcomeDialog;

public class WelcomeBot<T> : DialogBotBase<T>
    where T : Dialog
{
    public WelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBotBase<T>> logger)
        : base(conversationState, userState, dialog, logger)
    {
    }

    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {

                var welcomeCard = GetType().CreateAdaptiveCardAttachment(Resources.CardsWelcomeCard);

                var response = MessageFactory.Attachment(welcomeCard, ssml: Resources.WelcomeMessage);
                await turnContext.SendActivityAsync(response, cancellationToken);

                //Carga de menú inicial 

                var menuCard = GetType().CreateAdaptiveCardAttachment(Resources.CardsPrincipalMenu);
                var activecardMenu = MessageFactory.Attachment(menuCard);

                await turnContext.SendActivityAsync(activecardMenu, cancellationToken);

                await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(Resources.PropertyDialogState), cancellationToken);

            }
        }
    }
}