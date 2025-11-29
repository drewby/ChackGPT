using chackgpt.Web.Configuration;
using chackgpt.Web.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace chackgpt.Web.Tests;

/// <summary>
/// Unit tests for McpClientService using Model Context Protocol.
/// Tests MCP client initialization and configuration with Aspire service discovery.
/// </summary>
public class McpClientServiceTests
{
    private readonly Mock<ILogger<McpClientService>> _mockLogger;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly IOptions<McpClientOptions> _options;

    public McpClientServiceTests()
    {
        _mockLogger = new Mock<ILogger<McpClientService>>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(_mockLogger.Object);

        // Mock IConfiguration to simulate Aspire service discovery resolution
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["services:apiservice:http:0"])
            .Returns("http://localhost:5344");

        _options = Options.Create(new McpClientOptions
        {
            ApiServiceBaseUrl = "http://localhost:5344",
            TimeoutSeconds = 30
        });
    }

    [Fact]
    public void McpClientService_Constructor_RequiresValidParameters()
    {
        // Act 
        McpClientService service = new(_options, _mockLoggerFactory.Object, _mockLogger.Object, _mockConfiguration.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void McpClientOptions_DefaultValues_AreSet()
    {
        // Arrange & Act 
        McpClientOptions options = new();

        // Assert
        Assert.Equal("https+http://apiservice", options.ApiServiceBaseUrl);
        Assert.Equal(30, options.TimeoutSeconds);
    }

    [Fact]
    public void McpClientOptions_CustomValues_CanBeSet()
    {
        // Arrange & Act 
        McpClientOptions options = new()
        {
            ApiServiceBaseUrl = "http://custom.service:8080",
            TimeoutSeconds = 60
        };

        // Assert
        Assert.Equal("http://custom.service:8080", options.ApiServiceBaseUrl);
        Assert.Equal(60, options.TimeoutSeconds);
    }

    [Fact]
    public async Task McpClientService_ImplementsIAsyncDisposable()
    {
        // Arrange
        var service = new McpClientService(_options, _mockLoggerFactory.Object, _mockLogger.Object, _mockConfiguration.Object);

        // Act & Assert - Should not throw
        await service.DisposeAsync();
    }

    [Fact]
    public void McpClientService_ConstructorThrowsOnNullOptions()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new McpClientService(null!, _mockLoggerFactory.Object, _mockLogger.Object, _mockConfiguration.Object));
    }

    [Fact]
    public void McpClientService_ConstructorThrowsOnNullLoggerFactory()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new McpClientService(_options, null!, _mockLogger.Object, _mockConfiguration.Object));
    }

    [Fact]
    public void McpClientService_ConstructorThrowsOnNullLogger()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new McpClientService(_options, _mockLoggerFactory.Object, null!, _mockConfiguration.Object));
    }

    [Fact]
    public void McpClientService_ConstructorThrowsOnNullConfiguration()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new McpClientService(_options, _mockLoggerFactory.Object, _mockLogger.Object, null!));
    }
}
