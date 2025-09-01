namespace DiscordDotNetUtilities.Tests;

[TestClass]
public class TextStyleOptionTests
{
    [TestMethod]
    public void TextStyleOption_ShouldHaveExpectedValues()
    {
        // Assert
        Assert.AreEqual(0, (int)TextStyleOption.None);
        Assert.AreEqual(1, (int)TextStyleOption.Bold);
        Assert.AreEqual(2, (int)TextStyleOption.Italic);
        Assert.AreEqual(3, (int)TextStyleOption.Underline);
    }

    [TestMethod]
    public void TextStyleOption_ShouldHaveAllExpectedMembers()
    {
        // Arrange
        var expectedValues = new[]
        {
            TextStyleOption.None,
            TextStyleOption.Bold,
            TextStyleOption.Italic,
            TextStyleOption.Underline
        };

        // Act
        var actualValues = Enum.GetValues<TextStyleOption>();

        // Assert
        Assert.AreEqual(expectedValues.Length, actualValues.Length);
        foreach (var expected in expectedValues)
        {
            CollectionAssert.Contains(actualValues, expected);
        }
    }

    [DataTestMethod]
    [DataRow(TextStyleOption.None, "None")]
    [DataRow(TextStyleOption.Bold, "Bold")]
    [DataRow(TextStyleOption.Italic, "Italic")]
    [DataRow(TextStyleOption.Underline, "Underline")]
    public void TextStyleOption_ToString_ShouldReturnCorrectName(TextStyleOption option, string expectedName)
    {
        // Act
        var result = option.ToString();

        // Assert
        Assert.AreEqual(expectedName, result);
    }

    [TestMethod]
    public void TextStyleOption_CanBeCastToInt()
    {
        // Arrange & Act
        var noneValue = (int)TextStyleOption.None;
        var boldValue = (int)TextStyleOption.Bold;
        var italicValue = (int)TextStyleOption.Italic;
        var underlineValue = (int)TextStyleOption.Underline;

        // Assert
        Assert.AreEqual(0, noneValue);
        Assert.AreEqual(1, boldValue);
        Assert.AreEqual(2, italicValue);
        Assert.AreEqual(3, underlineValue);
    }

    [TestMethod]
    public void TextStyleOption_CanBeCastFromInt()
    {
        // Act
        var none = (TextStyleOption)0;
        var bold = (TextStyleOption)1;
        var italic = (TextStyleOption)2;
        var underline = (TextStyleOption)3;

        // Assert
        Assert.AreEqual(TextStyleOption.None, none);
        Assert.AreEqual(TextStyleOption.Bold, bold);
        Assert.AreEqual(TextStyleOption.Italic, italic);
        Assert.AreEqual(TextStyleOption.Underline, underline);
    }
}