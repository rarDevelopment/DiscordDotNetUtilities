using DiscordDotNetUtilities.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiscordDotNetUtilities.Tests;

[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public void DiscordFormatter_ShouldImplementIDiscordFormatter()
    {
        // Arrange
        var formatter = new DiscordFormatter();

        // Act & Assert
        Assert.IsInstanceOfType<IDiscordFormatter>(formatter);
    }

    [TestMethod]
    public void EventBus_ShouldImplementIEventBus()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockLogger = new Mock<ILogger<EventBus>>();
        var eventBus = new EventBus(mockServiceProvider.Object, mockLogger.Object);

        // Act & Assert
        Assert.IsInstanceOfType<IEventBus>(eventBus);
    }

    [TestMethod]
    public async Task EventBus_WithServiceCollection_ShouldResolveHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IEventHandler<TestEvent>, TestEventHandler>();
        services.AddSingleton<ILogger<EventBus>>(Mock.Of<ILogger<EventBus>>());
        
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = new EventBus(serviceProvider, serviceProvider.GetService<ILogger<EventBus>>()!);

        // Act & Assert - Should not throw
        await eventBus.PublishAsync(new TestEvent { Message = "Test" });
    }

    [TestMethod]
    public void MessageListBuilder_WithDiscordFormatter_ShouldWorkTogether()
    {
        // Arrange
        var messages = new List<string> { "Hello", "World" };
        var builder = new MessageListBuilder(messages, 2000);

        // Act
        var result = builder
            .WithTitle("Test", new[] { TextStyleOption.Bold, TextStyleOption.Italic })
            .WithDivider("\n")
            .Build();

        // Assert
        Assert.AreEqual(1, result.Count);
        var message = result[0];
        StringAssert.StartsWith(message, "_**Test**_");
        StringAssert.Contains(message, "Hello\n");
        StringAssert.Contains(message, "World\n");
    }

    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }

    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            // Simulate some work
            return Task.Delay(1, cancellationToken);
        }
    }
}