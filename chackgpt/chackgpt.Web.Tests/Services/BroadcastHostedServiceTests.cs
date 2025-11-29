using chackgpt.Web.Hubs;
using chackgpt.Web.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace chackgpt.Web.Tests.Services;

/// <summary>
/// Tests for the generic BroadcastHostedService base class.
/// Verifies lifecycle management and broadcasting functionality.
/// </summary>
public class BroadcastHostedServiceTests
{
    /// <summary>
    /// Test implementation of BroadcastHostedService for testing purposes
    /// </summary>
    public class TestBroadcastService : BroadcastHostedService<TestEventSource, ChatHub>
    {
        public bool SubscribedCalled { get; private set; }
        public bool UnsubscribedCalled { get; private set; }

        public TestBroadcastService(
            IHubContext<ChatHub> hubContext,
            TestEventSource eventSource,
            ILogger<TestBroadcastService> logger)
            : base(hubContext, eventSource, logger)
        {
        }

        protected override Task SubscribeToEventsAsync(CancellationToken cancellationToken)
        {
            SubscribedCalled = true;
            EventSource.TestEvent += OnTestEvent;
            return Task.CompletedTask;
        }

        protected override Task UnsubscribeFromEventsAsync()
        {
            UnsubscribedCalled = true;
            EventSource.TestEvent -= OnTestEvent;
            return Task.CompletedTask;
        }

        private async void OnTestEvent(object? sender, string data)
        {
            await BroadcastAsync("TestMethod", data);
        }

        // Expose protected method for testing
        public Task TestBroadcastAsync(string methodName, params object?[] args)
        {
            return BroadcastAsync(methodName, args);
        }
    }

    /// <summary>
    /// Test event source for testing
    /// </summary>
    public class TestEventSource
    {
        public event EventHandler<string>? TestEvent;

        public void RaiseTestEvent(string data)
        {
            TestEvent?.Invoke(this, data);
        }
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange (C# 9 target-typed new)
        Mock<IHubContext<ChatHub>> mockHubContext = new();
        TestEventSource eventSource = new();
        Mock<ILogger<TestBroadcastService>> mockLogger = new();

        // Act
        TestBroadcastService service = new(
            mockHubContext.Object,
            eventSource,
            mockLogger.Object);

        // Assert - C# 14 primary constructor successfully creates instance
        Assert.NotNull(service);
    }

    [Fact]
    public async Task StartAsync_CallsSubscribeToEventsAsync()
    {
        // Arrange (C# 9 target-typed new)
        Mock<IHubContext<ChatHub>> mockHubContext = new();
        TestEventSource eventSource = new();
        Mock<ILogger<TestBroadcastService>> mockLogger = new();
        
        TestBroadcastService service = new(
            mockHubContext.Object,
            eventSource,
            mockLogger.Object);

        // Act
        using CancellationTokenSource cts = new();
        await service.StartAsync(cts.Token);

        // Wait a bit for ExecuteAsync to start
        await Task.Delay(100);

        // Assert
        Assert.True(service.SubscribedCalled);
    }

    [Fact]
    public async Task StopAsync_CallsUnsubscribeFromEventsAsync()
    {
        // Arrange (C# 9 target-typed new)
        Mock<IHubContext<ChatHub>> mockHubContext = new();
        TestEventSource eventSource = new();
        Mock<ILogger<TestBroadcastService>> mockLogger = new();
        
        TestBroadcastService service = new(
            mockHubContext.Object,
            eventSource,
            mockLogger.Object);

        // Act
        using CancellationTokenSource cts = new();
        await service.StartAsync(cts.Token);
        await Task.Delay(100); // Wait for ExecuteAsync to start
        await service.StopAsync(cts.Token);

        // Assert
        Assert.True(service.UnsubscribedCalled);
    }

    [Fact]
    public async Task BroadcastAsync_InvokesSignalRHubMethod()
    {
        // Arrange (C# 9 target-typed new)
        Mock<IHubClients> mockHubClients = new();
        Mock<IClientProxy> mockClientProxy = new();
        mockHubClients.Setup(c => c.All).Returns(mockClientProxy.Object);
        
        Mock<IHubContext<ChatHub>> mockHubContext = new();
        mockHubContext.Setup(h => h.Clients).Returns(mockHubClients.Object);
        
        TestEventSource eventSource = new();
        Mock<ILogger<TestBroadcastService>> mockLogger = new();
        
        TestBroadcastService service = new(
            mockHubContext.Object,
            eventSource,
            mockLogger.Object);

        // Act
        await service.TestBroadcastAsync("TestMethod", "test-data");

        // Assert - Verify SendAsync was called on the client proxy
        mockClientProxy.Verify(
            c => c.SendCoreAsync(
                "TestMethod",
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task BroadcastAsync_WithException_LogsError()
    {
        // Arrange
        var mockHubClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClientProxy
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));
        
        mockHubClients.Setup(c => c.All).Returns(mockClientProxy.Object);
        
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        mockHubContext.Setup(h => h.Clients).Returns(mockHubClients.Object);
        
        TestEventSource eventSource = new();
        var mockLogger = new Mock<ILogger<TestBroadcastService>>();
        
        TestBroadcastService service = new(
            mockHubContext.Object,
            eventSource,
            mockLogger.Object);

        // Act
        await service.TestBroadcastAsync("TestMethod", "test-data");

        // Assert - Verify error was logged
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task EmotionBroadcastService_CanBeConstructed()
    {
        // Arrange
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var mockEmotionServiceLogger = new Mock<ILogger<ChackEmotionService>>();
        ChackEmotionService emotionService = new(mockEmotionServiceLogger.Object);
        var mockLogger = new Mock<ILogger<EmotionBroadcastService>>();

        // Act
        EmotionBroadcastService service = new(
            mockHubContext.Object,
            emotionService,
            mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task DrewEmotionBroadcastService_CanBeConstructed()
    {
        // Arrange
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var mockEmotionServiceLogger = new Mock<ILogger<DrewEmotionService>>();
        DrewEmotionService emotionService = new(mockEmotionServiceLogger.Object);
        var mockLogger = new Mock<ILogger<DrewEmotionBroadcastService>>();

        // Act
        DrewEmotionBroadcastService service = new(
            mockHubContext.Object,
            emotionService,
            mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task SlideBroadcastService_CanBeConstructed()
    {
        // Arrange
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var mockSlideServiceLogger = new Mock<ILogger<DisplaySlideService>>();
        DisplaySlideService slideService = new(mockSlideServiceLogger.Object);
        var mockLogger = new Mock<ILogger<SlideBroadcastService>>();

        // Act
        SlideBroadcastService service = new(
            mockHubContext.Object,
            slideService,
            mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task VideoBroadcastService_CanBeConstructed()
    {
        // Arrange
        var mockHubContext = new Mock<IHubContext<ChatHub>>();
        var mockVideoServiceLogger = new Mock<ILogger<DisplayVideoService>>();
        DisplayVideoService videoService = new(mockVideoServiceLogger.Object);
        var mockLogger = new Mock<ILogger<VideoBroadcastService>>();

        // Act
        VideoBroadcastService service = new(
            mockHubContext.Object,
            videoService,
            mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }
}
