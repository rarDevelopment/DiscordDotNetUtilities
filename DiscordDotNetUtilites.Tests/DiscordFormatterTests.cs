using Discord;
using Moq;

namespace DiscordDotNetUtilities.Tests;

[TestClass]
public class DiscordFormatterTests
{
    private readonly DiscordFormatter _discordFormatter;

    public DiscordFormatterTests()
    {
        _discordFormatter = new DiscordFormatter();
    }

    [TestMethod]
    public void BuildRegularEmbedWithUserFooter_ShouldCreateEmbedWithUserFooter()
    {
        // Arrange
        var mockUser = new Mock<IUser>();
        mockUser.Setup(u => u.Username).Returns("TestUser");
        mockUser.Setup(u => u.GetAvatarUrl(It.IsAny<ImageFormat>(), It.IsAny<ushort>())).Returns("https://example.com/avatar.png");
        
        var title = "Test Title";
        var messageText = "Test message content";

        // Act
        var result = _discordFormatter.BuildRegularEmbedWithUserFooter(title, messageText, mockUser.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(title, result.Title);
        Assert.AreEqual(messageText, result.Description);
        Assert.AreEqual(Color.Default, result.Color);
        Assert.IsNotNull(result.Footer);
        Assert.AreEqual("Requested by TestUser", result.Footer.Value.Text);
        Assert.AreEqual("https://example.com/avatar.png", result.Footer.Value.IconUrl);
    }

    [TestMethod]
    public void BuildRegularEmbedWithUserFooter_ShouldUseDefaultAvatarWhenNoCustomAvatar()
    {
        // Arrange
        var mockUser = new Mock<IUser>();
        mockUser.Setup(u => u.Username).Returns("TestUser");
        mockUser.Setup(u => u.GetAvatarUrl(It.IsAny<ImageFormat>(), It.IsAny<ushort>())).Returns((string?)null);
        mockUser.Setup(u => u.GetDefaultAvatarUrl()).Returns("https://example.com/default-avatar.png");
        
        var title = "Test Title";
        var messageText = "Test message content";

        // Act
        var result = _discordFormatter.BuildRegularEmbedWithUserFooter(title, messageText, mockUser.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Footer);
        Assert.AreEqual("https://example.com/default-avatar.png", result.Footer.Value.IconUrl);
    }

    [TestMethod]
    public void BuildRegularEmbed_ShouldCreateBasicEmbed()
    {
        // Arrange
        var title = "Test Title";
        var messageText = "Test message content";

        // Act
        var result = _discordFormatter.BuildRegularEmbed(title, messageText);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(title, result.Title);
        Assert.AreEqual(messageText, result.Description);
        Assert.AreEqual(Color.Default, result.Color);
        Assert.IsNull(result.Footer);
    }

    [TestMethod]
    public void BuildRegularEmbed_WithFields_ShouldIncludeFields()
    {
        // Arrange
        var title = "Test Title";
        var messageText = "Test message content";
        var fields = new List<EmbedFieldBuilder>
        {
            new() { Name = "Field1", Value = "Value1", IsInline = true },
            new() { Name = "Field2", Value = "Value2", IsInline = false }
        };

        // Act
        var result = _discordFormatter.BuildRegularEmbed(title, messageText, embedFieldBuilders: fields);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Fields.Length);
        Assert.AreEqual("Field1", result.Fields[0].Name);
        Assert.AreEqual("Value1", result.Fields[0].Value);
        Assert.IsTrue(result.Fields[0].Inline);
        Assert.AreEqual("Field2", result.Fields[1].Name);
        Assert.AreEqual("Value2", result.Fields[1].Value);
        Assert.IsFalse(result.Fields[1].Inline);
    }

    [TestMethod]
    public void BuildRegularEmbed_WithUrlAndImage_ShouldIncludeUrlAndImage()
    {
        // Arrange
        var title = "Test Title";
        var messageText = "Test message content";
        var url = "https://example.com";
        var imageUrl = "https://example.com/image.png";

        // Act
        var result = _discordFormatter.BuildRegularEmbed(title, messageText, url: url, imageUrl: imageUrl);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(url, result.Url);
        Assert.IsNotNull(result.Image);
        Assert.AreEqual(imageUrl, result.Image.Value.Url);
    }

    [TestMethod]
    public void BuildErrorEmbedWithUserFooter_ShouldCreateRedEmbedWithUserFooter()
    {
        // Arrange
        var mockUser = new Mock<IUser>();
        mockUser.Setup(u => u.Username).Returns("TestUser");
        mockUser.Setup(u => u.GetAvatarUrl(It.IsAny<ImageFormat>(), It.IsAny<ushort>())).Returns("https://example.com/avatar.png");
        
        var title = "Error Title";
        var messageText = "Error message content";

        // Act
        var result = _discordFormatter.BuildErrorEmbedWithUserFooter(title, messageText, mockUser.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(title, result.Title);
        Assert.AreEqual(messageText, result.Description);
        Assert.AreEqual(Color.Red, result.Color);
        Assert.IsNotNull(result.Footer);
        Assert.AreEqual("Requested by TestUser", result.Footer.Value.Text);
    }

    [TestMethod]
    public void BuildErrorEmbed_ShouldCreateRedEmbed()
    {
        // Arrange
        var title = "Error Title";
        var messageText = "Error message content";

        // Act
        var result = _discordFormatter.BuildErrorEmbed(title, messageText);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(title, result.Title);
        Assert.AreEqual(messageText, result.Description);
        Assert.AreEqual(Color.Red, result.Color);
        Assert.IsNull(result.Footer);
    }

    [DataTestMethod]
    [DataRow(TextStyleOption.Bold, "test", "**test**")]
    [DataRow(TextStyleOption.Italic, "test", "_test_")]
    [DataRow(TextStyleOption.Underline, "test", "__test__")]
    [DataRow(TextStyleOption.None, "test", "test")]
    public void ApplyTextStyleOptionToString_ShouldApplyCorrectFormatting(TextStyleOption option, string input, string expected)
    {
        // Act
        var result = _discordFormatter.ApplyTextStyleOptionToString(input, option);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void BuildRegularEmbed_WithFooter_ShouldIncludeCustomFooter()
    {
        // Arrange
        var title = "Test Title";
        var messageText = "Test message content";
        var footer = new EmbedFooterBuilder()
            .WithText("Custom Footer")
            .WithIconUrl("https://example.com/footer-icon.png");

        // Act
        var result = _discordFormatter.BuildRegularEmbed(title, messageText, footer);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Footer);
        Assert.AreEqual("Custom Footer", result.Footer.Value.Text);
        Assert.AreEqual("https://example.com/footer-icon.png", result.Footer.Value.IconUrl);
    }
}