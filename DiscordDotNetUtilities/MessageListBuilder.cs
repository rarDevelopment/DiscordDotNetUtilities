namespace DiscordDotNetUtilities
{
    public class MessageListBuilder
    {
        private readonly IEnumerable<string> _messagesToCombine;
        private int MaxMessageLength { get; }
        private string? Title { get; set; }
        private string? Divider { get; set; }

        public MessageListBuilder(IEnumerable<string> messagesToCombine, int maxMessageLength)
        {
            MaxMessageLength = maxMessageLength;
            _messagesToCombine = messagesToCombine;
        }

        /// <summary>
        ///     Build the list of messages with the specified maximum message length.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> Build()
        {
            var messageList = new List<string>();
            if (!string.IsNullOrEmpty(Title))
            {
                messageList.Add(Title);
            }

            var listIndex = 0;
            foreach (var individualMessage in _messagesToCombine)
            {
                var messageToAdd = $"{individualMessage}{Divider}";
                var concatenatedMessage = messageList.Count > 0 ? messageList[listIndex] + messageToAdd : messageToAdd;
                if (concatenatedMessage.Length > MaxMessageLength)
                {
                    messageList.Add(messageToAdd);
                    listIndex++;
                }
                else
                {
                    if (messageList.Count > 0)
                    {
                        messageList[listIndex] += messageToAdd;
                    }
                    else
                    {
                        messageList.Add(messageToAdd);
                    }
                }
            }
            return messageList;
        }

        /// <summary>
        ///     Adds a title to display before the first message of the list.
        /// </summary>
        /// <param name="title">The first message to add to beginning of the first message in the message list, to serve as the title.</param>
        /// <param name="textStyleOptions">Formatting options of type <see cref="TextStyleOption"/>.</param>
        /// <param name="titleDivider">Divider to add after the title for formatting reasons.</param>
        /// <param name="dividersAfterTitle">Number of dividers (specified in <see cref="titleDivider"/>) to include after the title.</param>
        /// <returns></returns>
        public MessageListBuilder WithTitle(string title,
            TextStyleOption[]? textStyleOptions = null,
            string titleDivider = "",
            int dividersAfterTitle = 0)
        {
            Title = title;
            if (textStyleOptions != null)
            {
                foreach (var textStyleOption in textStyleOptions)
                {
                    Title = ApplyTextStyleOptionToString(Title, textStyleOption);
                }
            }

            if (!string.IsNullOrEmpty(titleDivider))
            {
                for (var i = 0; i < dividersAfterTitle; i++)
                {
                    Title += titleDivider;
                }
            }
            return this;
        }

        /// <summary>
        ///     Adds a divider that will be added between messages.
        /// </summary>
        /// <param name="divider">The divider that will be added between messages. Example: \n</param>
        /// <returns></returns>
        public MessageListBuilder WithDivider(string divider)
        {
            Divider = divider;
            return this;
        }

        private static string ApplyTextStyleOptionToString(string text, TextStyleOption textStyleOption)
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
}
