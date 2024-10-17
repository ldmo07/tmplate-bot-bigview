using Bigview.Bot.Core.Components.Languages.Models;
using Bigview.Bot.Core.Components.Languages.Services;
using Bigview.Bot.Core.Properties;
using Bigview.Bot.Core.Utils;

using Microsoft.Bot.Builder.Dialogs;

namespace Bigview.Bot.Core.Dialogs.StudentDialog;

public class StudentConversationalDialogs : ConversationalServices
{

    private readonly IServiceProvider _serviceProvider;
    public StudentConversationalDialogs(ConversationalParameters conversationalParameters,
                        IServiceProvider serviceProvider) : base(conversationalParameters)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Manejador de la conversación asincrónico.
    /// </summary>
    /// <param name="dc">Contexto del diálogo.</param>
    /// <param name="basePrediction">Predicción base.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Entidad de conversación.</returns>
    public async override Task<Entity> ConversationalHandlerAsync(DialogContext dc, ConversationPrediction basePrediction, CancellationToken cancellationToken)
    {
        // Llama al método ConversationalHandlerAsync de la clase base y guarda el resultado en la variable conversationEntities.
        var conversationEntities = await base.ConversationalHandlerAsync(dc, basePrediction, cancellationToken);

        // Verifica si conversationEntities no es nulo.
        if (conversationEntities != null)
        {
            // Valida si debe lanzar un diálogo de tipo DocumentsStudentDialog.
            if (conversationEntities.Category.ToLower() == Resources.DialogsEntityCalificaciones.ToLower())
                DialogId = (_serviceProvider.GetDialog<DocumentStudentDialog>(), nameof(DocumentStudentDialog));

            // TODO: Adicionar los diálogos adicionales que se requieran configurar.

        }
        else
        {
            // Si conversationEntities es nulo, asigna valores predeterminados al DialogId.
            DialogId = (default, string.Empty);
        }

        // Devuelve la entidad de conversación.
        return conversationEntities;
    }



}