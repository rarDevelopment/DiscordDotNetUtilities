using DiscordDotNetUtilities;
using DiscordDotNetUtilities.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiscordDotNetUtilities.Tests;

[TestClass]
public class EventBusTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ILogger<EventBus>> _mockLogger;
    private readonly EventBus _eventBus;

    public EventBusTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<EventBus>>();
        _eventBus = new EventBus(_mockServiceProvider.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task PublishAsync_WithSingleHandler_ShouldCallHandler()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };
        var mockHandler = new Mock<IEventHandler<TestEvent>>();
        
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent>>)))
            .Returns(new[] { mockHandler.Object });

        // Act
        await _eventBus.PublishAsync(testEvent);

        // Assert
        mockHandler.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task PublishAsync_WithMultipleHandlers_ShouldCallAllHandlers()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };
        var mockHandler1 = new Mock<IEventHandler<TestEvent>>();
        var mockHandler2 = new Mock<IEventHandler<TestEvent>>();
        var mockHandler3 = new Mock<IEventHandler<TestEvent>>();
        
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent>>)))
            .Returns(new[] { mockHandler1.Object, mockHandler2.Object, mockHandler3.Object });

        // Act
        await _eventBus.PublishAsync(testEvent);

        // Assert
        mockHandler1.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        mockHandler2.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        mockHandler3.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task PublishAsync_WithNoHandlers_ShouldNotThrow()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };
        
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent>>)))
            .Returns(Array.Empty<IEventHandler<TestEvent>>());

        // Act & Assert
        try
        {
            await _eventBus.PublishAsync(testEvent);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected no exception, but got: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task PublishAsync_WithHandlerThatThrows_ShouldLogErrorAndContinue()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };
        var mockHandler1 = new Mock<IEventHandler<TestEvent>>();
        var mockHandler2 = new Mock<IEventHandler<TestEvent>>();
        
        var expectedException = new InvalidOperationException("Handler failed");
        mockHandler1.Setup(h => h.HandleAsync(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent>>)))
            .Returns(new[] { mockHandler1.Object, mockHandler2.Object });

        // Act
        await _eventBus.PublishAsync(testEvent);

        // Assert
        mockHandler1.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        mockHandler2.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        
        // Verify error was logged
        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task PublishAsync_WithCancellationToken_ShouldPassTokenToHandlers()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };
        var mockHandler = new Mock<IEventHandler<TestEvent>>();
        var cancellationToken = new CancellationToken();
        
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent>>)))
            .Returns(new[] { mockHandler.Object });

        // Act
        await _eventBus.PublishAsync(testEvent, cancellationToken);

        // Assert
        mockHandler.Verify(h => h.HandleAsync(testEvent, cancellationToken), Times.Once);
    }

    [TestMethod]
    public async Task PublishAsync_WithMultipleHandlersAndSomeThrow_ShouldExecuteAllAndLogErrors()
    {
        // Arrange
        var testEvent = new TestEvent { Message = "Test message" };
        var mockHandler1 = new Mock<IEventHandler<TestEvent>>();
        var mockHandler2 = new Mock<IEventHandler<TestEvent>>();
        var mockHandler3 = new Mock<IEventHandler<TestEvent>>();
        
        var exception1 = new InvalidOperationException("Handler 1 failed");
        var exception3 = new ArgumentException("Handler 3 failed");
        
        mockHandler1.Setup(h => h.HandleAsync(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception1);
        mockHandler3.Setup(h => h.HandleAsync(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception3);
        
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestEvent>>)))
            .Returns(new[] { mockHandler1.Object, mockHandler2.Object, mockHandler3.Object });

        // Act
        await _eventBus.PublishAsync(testEvent);

        // Assert
        mockHandler1.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        mockHandler2.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        mockHandler3.Verify(h => h.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        
        // Verify both errors were logged
        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    public class TestEvent
    {
        public string Message { get; set; } = string.Empty;
    }
}