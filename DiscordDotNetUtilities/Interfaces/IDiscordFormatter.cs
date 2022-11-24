using Discord;

namespace DiscordDotNetUtilities.Interfaces;

public interface IDiscordFormatter
{
    Embed BuildRegularEmbed(string title, string messageText, IUser? user = null, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "");
    Embed BuildErrorEmbed(string title, string messageText, IUser? user = null, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "");

    IReadOnlyList<string> BuildMessageListFromStringList(IReadOnlyList<string> messagesToCombine, int maxMessageLength,
        string firstMessageTitle = "", int newLinesAfterFirstMessage = 0);
}