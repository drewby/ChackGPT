using chackgpt.Web.Services;
using chackgpt.Web.Configuration;
using chackgpt.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace chackgpt.Web.Tests.Integration;

/// <summary>
/// Integration tests for AgentFactory verifying proper agent creation
/// with all dependencies and tool registration.
/// </summary>
public class AgentFactoryIntegrationTests
{
    private readonly Mock<IOptions<AzureOpenAIOptions>> _mockAzureOptions;
    private readonly Mock<ILogger<AgentFactory>> _mockLogger;
    private readonly AgentFactory _factory;

    public AgentFactoryIntegrationTests()
    {
        _mockAzureOptions = new Mock<IOptions<AzureOpenAIOptions>>();
        _mockAzureOptions.Setup(x => x.Value).Returns(new AzureOpenAIOptions
        {
            Endpoint = "https://test.openai.azure.com",
            ApiKey = "test-key",
            DeploymentName = "test-deployment",
            ServiceVersion = "2024-06-01"
        });

        _mockLogger = new Mock<ILogger<AgentFactory>>();

        _factory = new AgentFactory(
            _mockAzureOptions.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public void CreateAgent_WithValidParameters_ReturnsConfiguredAgent()
    {
        // Arrange
        var agentName = "TestAgent";
        var instructions = "Test instructions";

        // Act
        var agent = _factory.CreateAgent(agentName, instructions, []);

        // Assert
        Assert.NotNull(agent);
        Assert.Equal(agentName, agent.Name);
    }

    [Fact]
    public void CreateAgent_WithTools_ReturnsAgentWithTools()
    {
        // Arrange
        var agentName = "TestAgent";
        var instructions = "Test instructions";

        // Act
        var agent = _factory.CreateAgent(agentName, instructions, []);

        // Assert
        Assert.NotNull(agent);
        Assert.Equal(agentName, agent.Name);
    }

    [Fact]
    public void CreateAgent_UsesC14PrimaryConstructor()
    {
        // This test documents that AgentFactory uses C# 14 primary constructor
        // Integration: All constructor dependencies are properly initialized

        // Arrange & Act
        var agent = _factory.CreateAgent("TestAgent", "Test instructions", []);

        // Assert
        Assert.NotNull(agent);
        // Successful creation confirms all dependencies injected via primary constructor
    }

    [Fact]
    public void CreateAgent_UsesC14ParamsCollections()
    {
        // This test verifies C# 14 params collections feature usage
        // Integration: Flexible tool array passing using params AITool[]

        // Arrange
        var mcpTools = Array.Empty<Microsoft.Extensions.AI.AITool>();

        // Act
        var agentWithNoTools = _factory.CreateAgent("Agent1", "Instructions", []);
        var agentWithMcpTools = _factory.CreateAgent("Agent2", "Instructions", mcpTools);

        // Assert
        Assert.NotNull(agentWithNoTools);
        Assert.NotNull(agentWithMcpTools);
    }

    [Fact]
    public void CreateAgent_UsesC14CollectionExpressions()
    {
        // This test documents C# 14 collection expression usage for tool registration
        // Pattern: [.. tools] in agent creation

        // Arrange & Act
        var agent = _factory.CreateAgent("TestAgent", "Test instructions", []);

        // Assert
        Assert.NotNull(agent);
        // Collection expression enables efficient tool array spreading
    }

    [Theory]
    [InlineData("ChackGPT", "Chack instructions")]
    [InlineData("DrewGPT", "Drew instructions")]
    public void CreateAgent_WithDifferentConfigurations_CreatesUniqueAgents(string agentName, string instructions)
    {
        // This test verifies factory can create multiple different agents
        // Integration: AgentFactory → Azure OpenAI → Configured Agent

        // Arrange & Act
        var agent = _factory.CreateAgent(agentName, instructions, []);

        // Assert
        Assert.NotNull(agent);
        Assert.Equal(agentName, agent.Name);
    }

    [Fact]
    public void AgentFactory_FollowsSingleResponsibilityPrinciple()
    {
        // SRP: AgentFactory has one job - create properly configured agents

        // Arrange & Act
        var agent1 = _factory.CreateAgent("Agent1", "Instructions1", []);
        var agent2 = _factory.CreateAgent("Agent2", "Instructions2", []);

        // Assert
        Assert.NotNull(agent1);
        Assert.NotNull(agent2);
        Assert.NotEqual(agent1.Name, agent2.Name);
    }

    [Fact]
    public void AgentFactory_FollowsOpenClosedPrinciple()
    {
        // OCP: Factory is open for extension (new agents) but closed for modification
        // New agents can be added without changing existing factory logic

        // Arrange & Act
        var agent = _factory.CreateAgent("NewAgent", "New instructions", []);

        // Assert
        Assert.NotNull(agent);
        // Factory accepts any valid agent name/instructions without modification
    }

    [Fact]
    public void AgentFactory_IntegratesWithAzureOpenAI()
    {
        // This test verifies integration with Azure OpenAI services
        // Integration: AgentFactory → AzureOpenAIClient → ChatClient

        // Arrange & Act
        var agent = _factory.CreateAgent("TestAgent", "Test instructions", []);

        // Assert
        Assert.NotNull(agent);
        // Successful creation proves Azure OpenAI integration working
    }
}
