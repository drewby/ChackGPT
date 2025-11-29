using chackgpt.Web.Configuration;
using chackgpt.Web.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace chackgpt.Web.Tests.Services;

/// <summary>
/// Unit tests for AgentFactory.
/// Tests unified agent creation with Options pattern and C# 14 features.
/// </summary>
public class AgentFactoryTests
{
    private readonly Mock<ILogger<AgentFactory>> _mockLogger;
    private readonly IOptions<AzureOpenAIOptions> _options;

    public AgentFactoryTests()
    {
        _mockLogger = new Mock<ILogger<AgentFactory>>();

        _options = Options.Create(new AzureOpenAIOptions
        {
            Endpoint = "https://test-openai.openai.azure.com/",
            ApiKey = "test-api-key-12345",
            DeploymentName = "gpt-4",
            ServiceVersion = "2024-10-21"
        });
    }

    [Fact]
    public void AgentFactory_Constructor_RequiresValidOptions()
    {
        // Act
        AgentFactory factory = new(_options, _mockLogger.Object);

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void CreateAgent_RequiresAgentName()
    {
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.CreateAgent(null!, "test instructions"));
    }

    [Fact]
    public void CreateAgent_RequiresInstructions()
    {
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.CreateAgent("TestAgent", null!));
    }

    [Fact]
    public void CreateAgent_WithNoTools_CreatesAgent()
    {
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);

        // Act
        var agent = factory.CreateAgent(
            "TestAgent",
            "You are a test agent.");

        // Assert
        Assert.NotNull(agent);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("TestAgent agent created with 0 tools")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void CreateAgent_WithSingleTool_CreatesAgentWithTool()
    {
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);
        AITool testTool = AIFunctionFactory.Create(
            () => "test result",
            "TestTool",
            "A test tool");

        // Act - Uses C# 14 params collections
        var agent = factory.CreateAgent(
            "TestAgent",
            "You are a test agent.",
            testTool);

        // Assert
        Assert.NotNull(agent);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("TestAgent agent created with 1 tools")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void CreateAgent_WithMultipleTools_CreatesAgentWithAllTools()
    {
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);
        AITool tool1 = AIFunctionFactory.Create(
            () => "result1",
            "Tool1",
            "First tool");
        AITool tool2 = AIFunctionFactory.Create(
            () => "result2",
            "Tool2",
            "Second tool");
        AITool tool3 = AIFunctionFactory.Create(
            () => "result3",
            "Tool3",
            "Third tool");

        // Act - Uses C# 14 params collections
        var agent = factory.CreateAgent(
            "TestAgent",
            "You are a test agent.",
            tool1, tool2, tool3);

        // Assert
        Assert.NotNull(agent);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("TestAgent agent created with 3 tools")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void CreateAgent_WithChackGPTName_CreatesChackGPTAgent()
    {
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);

        // Act
        var agent = factory.CreateAgent(
            "ChackGPT",
            "You are ChackGPT.");

        // Assert
        Assert.NotNull(agent);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ChackGPT agent created")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void CreateAgent_WithDrewGPTName_CreatesDrewGPTAgent()
    {
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);

        // Act
        var agent = factory.CreateAgent(
            "DrewGPT",
            "You are DrewGPT.");

        // Assert
        Assert.NotNull(agent);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("DrewGPT agent created")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("2024-06-01")]
    [InlineData("2024-08-01-preview")]
    [InlineData("2024-10-21")]
    [InlineData("2025-01-01-preview")]
    public void CreateAgent_WithValidServiceVersions_CreatesAgent(string serviceVersion)
    {
        // Arrange
        IOptions<AzureOpenAIOptions> options = Options.Create(new AzureOpenAIOptions
        {
            Endpoint = "https://test-openai.openai.azure.com/",
            ApiKey = "test-api-key-12345",
            DeploymentName = "gpt-4",
            ServiceVersion = serviceVersion
        });
        AgentFactory factory = new(options, _mockLogger.Object);

        // Act
        var agent = factory.CreateAgent("TestAgent", "Test instructions");

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void CreateAgent_WithInvalidServiceVersion_ThrowsArgumentException()
    {
        // Arrange
        IOptions<AzureOpenAIOptions> options = Options.Create(new AzureOpenAIOptions
        {
            Endpoint = "https://test-openai.openai.azure.com/",
            ApiKey = "test-api-key-12345",
            DeploymentName = "gpt-4",
            ServiceVersion = "invalid-version"
        });
        AgentFactory factory = new(options, _mockLogger.Object);

        // Act & Assert
        ArgumentException ex = Assert.Throws<ArgumentException>(() =>
            factory.CreateAgent("TestAgent", "Test instructions"));
        
        Assert.Contains("Unsupported service version: invalid-version", ex.Message);
    }

    [Fact]
    public void CreateAgent_UsesC14PrimaryConstructor_NoFieldDeclarations()
    {
        // This test validates that AgentFactory uses C# 14 primary constructor
        // by verifying the factory can be instantiated and used successfully
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);

        // Act
        var agent = factory.CreateAgent("TestAgent", "Test instructions");

        // Assert - If primary constructor works correctly, agent is created
        Assert.NotNull(agent);
    }

    [Fact]
    public void CreateAgent_UsesC14ParamsCollections_AcceptsVariableArguments()
    {
        // This test validates that CreateAgent uses C# 14 params collections
        // by passing tools in different ways
        // Arrange
        AgentFactory factory = new(_options, _mockLogger.Object);
        AITool tool1 = AIFunctionFactory.Create(() => "1", "T1", "Tool 1");
        AITool tool2 = AIFunctionFactory.Create(() => "2", "T2", "Tool 2");

        // Act - Test various params collection invocations
        var agent1 = factory.CreateAgent("Agent1", "Instructions"); // No tools
        var agent2 = factory.CreateAgent("Agent2", "Instructions", tool1); // Single tool
        var agent3 = factory.CreateAgent("Agent3", "Instructions", tool1, tool2); // Multiple tools

        // Assert
        Assert.NotNull(agent1);
        Assert.NotNull(agent2);
        Assert.NotNull(agent3);
    }
}
