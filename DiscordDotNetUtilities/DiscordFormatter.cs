using Discord;
using DiscordDotNetUtilities.Interfaces;

namespace DiscordDotNetUtilities;
public class DiscordFormatter : IDiscordFormatter
{
    public Embed BuildRegularEmbed(string title, string messageText, IUser? user = null, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "")
    {
        return BuildEmbed(title, messageText, user, Color.Default, embedFieldBuilders, url).Build();
    }
    public Embed BuildErrorEmbed(string title, string messageText, IUser? user = null, IList<EmbedFieldBuilder>? embedFieldBuilders = null, string url = "")
    {
        return BuildEmbed(title, messageText, user, Color.Red, embedFieldBuilders, url).Build();
    }

    private static EmbedBuilder BuildEmbed(string title,
        string messageText,
        IUser? user = null,
        Color? color = null,
        IList<EmbedFieldBuilder>? embedFieldBuilders = null,
        string url = "")
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(messageText)
            .WithColor(color ?? Color.Default)
            .WithCurrentTimestamp();

        if (user != null)
        {
            embedBuilder.WithFooter(BuildEmbedFooter(user));
        }
        if (embedFieldBuilders != null && embedFieldBuilders.Any())
        {
            embedBuilder.WithFields(embedFieldBuilders);
        }
        if (!string.IsNullOrEmpty(url))
        {
            embedBuilder.WithUrl(url);
        }

        return embedBuilder;
    }

    private static EmbedFooterBuilder BuildEmbedFooter(IUser user)
    {
        return new EmbedFooterBuilder()
            .WithText(GetEmbedFooterText(user))
            .WithIconUrl(GetUserAvatar(user));
    }

    private static string GetEmbedFooterText(IUser user)
    {
        return $"Requested by {user.Username}";
    }

    private static string GetUserAvatar(IUser user)
    {
        return user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
    }

    public IReadOnlyList<string> BuildMessageListFromStringList(IReadOnlyList<string> messagesToCombine, int maxMessageLength,
        string firstMessageTitle = "", int newLinesAfterFirstMessage = 0)
    {
        var messageList = new List<string>();
        if (!string.IsNullOrEmpty(firstMessageTitle))
        {
            var title = $"**{firstMessageTitle}**";
            for (var i = 0; i < newLinesAfterFirstMessage; i++)
            {
                title += "\n";
            }
            messageList.Add(title);
        }

        var listIndex = 0;
        foreach (var individualMessage in messagesToCombine)
        {
            var messageToAdd = $"{individualMessage}\n";
            var concatenatedMessage = messageList[listIndex] + messageToAdd;
            if (concatenatedMessage.Length >= maxMessageLength)
            {
                messageList.Add(messageToAdd);
                listIndex++;
            }
            else
            {
                messageList[listIndex] += messageToAdd;
            }
        }
        return messageList;
    }
}
