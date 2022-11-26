namespace DiscordDotNetUtilities.Tests
{
    public class MessageListBuilderTests
    {
        [Theory]
        [InlineData(2000, 100, 200, 2, null, null)]
        [InlineData(2000, 100, 200, 2, "\n", "test")]
        [InlineData(2000, 100, 10, 2, "\n", "test")]
        [InlineData(2000, 100, 2000, 0, "\n", null)]
        [InlineData(48339, 500, 43079, 5, "\n\n\n", "big title or something")]
        public void BuildMessageListFromStringList_Success(
            int maxMessageLength,
            int lengthOfString,
            int numberOfMessages,
            int dividersAfterTitle,
            string? divider,
            string? title)
        {
            var stringList = new List<string>();


            for (var i = 0; i < numberOfMessages; i++)
            {
                var messageToAdd = string.Empty;
                for (var s = 0; s < lengthOfString; s++)
                {
                    messageToAdd += "word";
                }
                stringList.Add(messageToAdd);
            }

            MessageListBuilder discordMessageListBuilder =
                new MessageListBuilder(stringList, maxMessageLength);

            if (!string.IsNullOrEmpty(title))
            {
                discordMessageListBuilder = discordMessageListBuilder.WithTitle(title, new[]
                    {
                        TextStyleOption.Bold,
                        TextStyleOption.Italic,
                        TextStyleOption.Underline,
                    }, "\n", dividersAfterTitle);
            }

            if (!string.IsNullOrEmpty(divider))
            {
                discordMessageListBuilder = discordMessageListBuilder.WithDivider(divider);
            }

            var messageList = discordMessageListBuilder.Build();

            Assert.All(messageList, s => Assert.True(s.Length <= maxMessageLength));
        }
    }
}
