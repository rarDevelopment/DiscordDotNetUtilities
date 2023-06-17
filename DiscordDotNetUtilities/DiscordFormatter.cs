using Discord;
using DiscordDotNetUtilities.Interfaces;

namespace DiscordDotNetUtilities;
public class DiscordFormatter : IDiscordFormatter
{
    public Embed BuildRegularEmbed(string title,
        string messageText,
        IUser user,
        IList<EmbedFieldBuilder>? embedFieldBuilders = null,
        string url = "",
        string imageUrl = "")
    {
        var embedFooterBuilder = SetUpEmbedFooterBuilder(user);
        return BuildEmbed(title, messageText, Color.Default, embedFieldBuilders, url, imageUrl, embedFooterBuilder).Build();
    }

    public Embed BuildRegularEmbed(string title,
        string messageText,
        EmbedFooterBuilder? embedFooterBuilder,
        IList<EmbedFieldBuilder>? embedFieldBuilders = null,
        string url = "",
        string imageUrl = "")
    {
        return BuildEmbed(title, messageText, Color.Default, embedFieldBuilders, url, imageUrl, embedFooterBuilder).Build();
    }

    public Embed BuildErrorEmbed(string title,
        string messageText,
        IUser user,
        IList<EmbedFieldBuilder>? embedFieldBuilders = null,
        string url = "",
        string imageUrl = "")
    {
        var embedFooterBuilder = SetUpEmbedFooterBuilder(user);
        return BuildEmbed(title, messageText, Color.Red, embedFieldBuilders, url, imageUrl, embedFooterBuilder).Build();
    }

    public Embed BuildErrorEmbed(string title,
        string messageText,
        EmbedFooterBuilder? embedFooterBuilder,
        IList<EmbedFieldBuilder>? embedFieldBuilders = null,
        string url = "",
        string imageUrl = "")
    {
        return BuildEmbed(title, messageText, Color.Red, embedFieldBuilders, url, imageUrl, embedFooterBuilder).Build();
    }

    private static EmbedBuilder BuildEmbed(string title,
        string messageText,
        Color? color = null,
        IList<EmbedFieldBuilder>? embedFieldBuilders = null,
        string url = "",
        string imageUrl = "",
        EmbedFooterBuilder? embedFooterBuilder = null)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(messageText)
            .WithColor(color ?? Color.Default)
            .WithCurrentTimestamp();

        if (embedFooterBuilder != null)
        {
            embedBuilder.WithFooter(embedFooterBuilder);
        }

        if (embedFieldBuilders != null && embedFieldBuilders.Any())
        {
            embedBuilder.WithFields(embedFieldBuilders);
        }

        if (!string.IsNullOrEmpty(url))
        {
            embedBuilder.WithUrl(url);
        }

        if (!string.IsNullOrEmpty(imageUrl))
        {
            embedBuilder.WithImageUrl(imageUrl);
        }

        return embedBuilder;
    }

    private static EmbedFooterBuilder SetUpEmbedFooterBuilder(IUser user)
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

    public string ApplyTextStyleOptionToString(string text, TextStyleOption textStyleOption)
    {
        return textStyleOption switch
        {
            TextStyleOption.Bold => $"**{text}**",
            TextStyleOption.Italic => $"_{text}_",
            TextStyleOption.Underline => $"__{text}__",
            _ => text
        };
    }
}
