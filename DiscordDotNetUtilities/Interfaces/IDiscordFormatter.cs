using Discord;

namespace DiscordDotNetUtilities.Interfaces;

public interface IDiscordFormatter
{
    Embed BuildRegularEmbedWithUserFooter(string title, string messageText, IUser user, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "", string imageUrl = "");
    Embed BuildRegularEmbed(string title, string messageText, EmbedFooterBuilder? embedFooterBuilder = null, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "", string imageUrl = "");
    Embed BuildErrorEmbedWithUserFooter(string title, string messageText, IUser user, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "", string imageUrl = "");
    Embed BuildErrorEmbed(string title, string messageText, EmbedFooterBuilder? embedFooterBuilder = null, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "", string imageUrl = "");
}