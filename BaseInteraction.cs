using Discord;
using Discord.Interactions;

namespace tDarkBot
{
    public abstract class BaseInteraction<T> : InteractionModuleBase<T>
        where T : class, IInteractionContext
    {

    }
}