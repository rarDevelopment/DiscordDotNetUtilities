namespace DiscordDotNetUtilities.Tests
{
    [TestClass]
    public class MessageListBuilderTests
    {
        [DataTestMethod]
        [DataRow(2000, 100, 200, 2, null, null)]
        [DataRow(2000, 100, 200, 2, "\n", "test")]
        [DataRow(2000, 100, 10, 2, "\n", "test")]
        [DataRow(2000, 100, 2000, 0, "\n", null)]
        [DataRow(48339, 500, 43079, 5, "\n\n\n", "big title or something")]
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

            foreach (var message in messageList)
            {
                Assert.IsTrue(message.Length <= maxMessageLength);
            }
        }

        [TestMethod]
        public void Build_WithEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<string>();
            var builder = new MessageListBuilder(emptyList, 2000);

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Build_WithSingleShortMessage_ShouldReturnSingleMessage()
        {
            // Arrange
            var messages = new List<string> { "Hello" };
            var builder = new MessageListBuilder(messages, 2000);

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Hello", result[0]);
        }

        [TestMethod]
        public void Build_WithTitleOnly_ShouldIncludeTitle()
        {
            // Arrange
            var messages = new List<string> { "Hello", "World" };
            var builder = new MessageListBuilder(messages, 2000)
                .WithTitle("Test Title");

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            StringAssert.StartsWith(result[0], "Test Title");
            StringAssert.Contains(result[0], "Hello");
            StringAssert.Contains(result[0], "World");
        }

        [TestMethod]
        public void WithTitle_WithStyleOptions_ShouldApplyFormatting()
        {
            // Arrange
            var messages = new List<string> { "Hello" };
            var builder = new MessageListBuilder(messages, 2000)
                .WithTitle("Title", new[] { TextStyleOption.Bold });

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            StringAssert.StartsWith(result[0], "**Title**");
        }

        [TestMethod]
        public void WithTitle_WithMultipleStyleOptions_ShouldApplyAllFormatting()
        {
            // Arrange
            var messages = new List<string> { "Hello" };
            var builder = new MessageListBuilder(messages, 2000)
                .WithTitle("Title", new[] { TextStyleOption.Bold, TextStyleOption.Italic });

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            StringAssert.StartsWith(result[0], "_**Title**_");
        }

        [TestMethod]
        public void WithTitle_WithDividers_ShouldAddDividers()
        {
            // Arrange
            var messages = new List<string> { "Hello" };
            var builder = new MessageListBuilder(messages, 2000)
                .WithTitle("Title", null, "\n", 2);

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            StringAssert.StartsWith(result[0], "Title\n\n");
        }

        [TestMethod]
        public void WithDivider_ShouldAddDividerBetweenMessages()
        {
            // Arrange
            var messages = new List<string> { "Hello", "World" };
            var builder = new MessageListBuilder(messages, 2000)
                .WithDivider("\n");

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Hello\nWorld\n", result[0]);
        }

        [TestMethod]
        public void Build_WithMessagesThatExceedMaxLength_ShouldSplitIntoMultipleMessages()
        {
            // Arrange
            var longMessage = new string('a', 1500);
            var messages = new List<string> { longMessage, longMessage };
            var builder = new MessageListBuilder(messages, 2000);

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(2, result.Count);
            foreach (var message in result)
            {
                Assert.IsTrue(message.Length <= 2000);
            }
        }

        [TestMethod]
        public void Build_FluentInterface_ShouldAllowChaining()
        {
            // Arrange
            var messages = new List<string> { "Hello", "World" };

            // Act
            var result = new MessageListBuilder(messages, 2000)
                .WithTitle("Title", new[] { TextStyleOption.Bold })
                .WithDivider("\n")
                .Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            StringAssert.StartsWith(result[0], "**Title**");
            StringAssert.Contains(result[0], "Hello\n");
            StringAssert.Contains(result[0], "World\n");
        }

        [TestMethod]
        public void Build_WithLongTitleAndShortMaxLength_ShouldHandleCorrectly()
        {
            // Arrange
            var longTitle = new string('A', 100);
            var messages = new List<string> { "Hello" };
            var builder = new MessageListBuilder(messages, 150)
                .WithTitle(longTitle);

            // Act
            var result = builder.Build();

            // Assert
            Assert.IsTrue(result.Count >= 1);
            foreach (var message in result)
            {
                Assert.IsTrue(message.Length <= 150);
            }
        }

        [TestMethod]
        public void WithTitle_WithNullStyleOptions_ShouldNotThrow()
        {
            // Arrange
            var messages = new List<string> { "Hello" };
            var builder = new MessageListBuilder(messages, 2000);

            // Act & Assert
            try
            {
                builder.WithTitle("Title", null);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception, but got: {ex.Message}");
            }
        }

        [TestMethod]
        public void WithTitle_WithEmptyTitleDivider_ShouldNotAddDividers()
        {
            // Arrange
            var messages = new List<string> { "Hello" };
            var builder = new MessageListBuilder(messages, 2000)
                .WithTitle("Title", null, "", 5);

            // Act
            var result = builder.Build();

            // Assert
            Assert.AreEqual(1, result.Count);
            StringAssert.StartsWith(result[0], "Title");
            Assert.IsFalse(result[0].Substring(0, 5).Contains("\n"));
        }
    }
}
