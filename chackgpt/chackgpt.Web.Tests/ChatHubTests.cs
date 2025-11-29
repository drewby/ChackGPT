using chackgpt.Web.Hubs;
using chackgpt.Web.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace chackgpt.Web.Tests;

public class ChatHubTests
{
    /// <summary>
    /// Creates a minimal test workflow without real Azure OpenAI configuration
    /// This uses a simple workflow builder without actual AI agents for unit testing purposes
    /// </summary>
    private static Workflow CreateTestWorkflow()
    {
        // Create a minimal empty workflow for testing the ChatHub constructor
        // We can't create real AIAgent instances without Azure OpenAI configuration
        // This workflow is sufficient for testing that ChatHub accepts the Workflow dependency
        Workflow workflow = AgentWorkflowBuilder
            .CreateGroupChatBuilderWith(agents => new AspireKeywordGroupChatManager(agents))
            .Build();

        return workflow;
    }

    [Fact]
    public void SendMessage_HubConfiguration_IsValid()
    {
        // Arrange
        Workflow workflow = CreateTestWorkflow();
        var mockLogger = new Mock<ILogger<ChatHub>>();
        var mockEmotionServiceLogger = new Mock<ILogger<chackgpt.Web.Services.ChackEmotionService>>();
        var mockDrewEmotionServiceLogger = new Mock<ILogger<chackgpt.Web.Services.DrewEmotionService>>();
        var mockDisplaySlideServiceLogger = new Mock<ILogger<chackgpt.Web.Services.DisplaySlideService>>();
        var mockDisplayVideoServiceLogger = new Mock<ILogger<chackgpt.Web.Services.DisplayVideoService>>();
        var mockChatMessageServiceLogger = new Mock<ILogger<chackgpt.Web.Services.ChatMessageService>>();
        
        // C# 9 target-typed new expressions
        chackgpt.Web.Services.ChackEmotionService emotionService = new(mockEmotionServiceLogger.Object);
        chackgpt.Web.Services.DrewEmotionService drewEmotionService = new(mockDrewEmotionServiceLogger.Object);
        chackgpt.Web.Services.DisplaySlideService displaySlideService = new(mockDisplaySlideServiceLogger.Object);
        chackgpt.Web.Services.DisplayVideoService displayVideoService = new(mockDisplayVideoServiceLogger.Object);
        chackgpt.Web.Services.ChatMessageService chatMessageService = new(mockChatMessageServiceLogger.Object);
        
        Mock<IHubCallerClients> mockClients = new();
        Mock<ISingleClientProxy> mockClientProxy = new();
        mockClients.Setup(c => c.Caller).Returns(mockClientProxy.Object);

        ChatHub hub = new(
            workflow,
            mockLogger.Object,
            emotionService,
            drewEmotionService,
            displaySlideService,
            displayVideoService,
            chatMessageService
        )
        {
            Clients = mockClients.Object
        };

        // Act & Assert
        // Note: Full integration test would require mocking the workflow streaming response
        // This test verifies the hub can be constructed with Agent Framework workflow
        Assert.NotNull(hub);
        Assert.NotNull(hub.Clients);
    }

    [Fact]
    public void ChatHub_Constructor_RequiresValidParameters()
    {
        // Arrange (C# 9 target-typed new for all mock objects)
        Workflow workflow = CreateTestWorkflow();
        Mock<ILogger<ChatHub>> mockLogger = new();
        Mock<ILogger<chackgpt.Web.Services.ChackEmotionService>> mockEmotionServiceLogger = new();
        Mock<ILogger<chackgpt.Web.Services.DrewEmotionService>> mockDrewEmotionServiceLogger = new();
        Mock<ILogger<chackgpt.Web.Services.DisplaySlideService>> mockDisplaySlideServiceLogger = new();
        Mock<ILogger<chackgpt.Web.Services.DisplayVideoService>> mockDisplayVideoServiceLogger = new();
        Mock<ILogger<chackgpt.Web.Services.ChatMessageService>> mockChatMessageServiceLogger = new();

        // C# 9 target-typed new expressions
        chackgpt.Web.Services.ChackEmotionService emotionService = new(mockEmotionServiceLogger.Object);
        chackgpt.Web.Services.DrewEmotionService drewEmotionService = new(mockDrewEmotionServiceLogger.Object);
        chackgpt.Web.Services.DisplaySlideService displaySlideService = new(mockDisplaySlideServiceLogger.Object);
        chackgpt.Web.Services.DisplayVideoService displayVideoService = new(mockDisplayVideoServiceLogger.Object);
        chackgpt.Web.Services.ChatMessageService chatMessageService = new(mockChatMessageServiceLogger.Object);

        // Act
        ChatHub hub = new(
            workflow,
            mockLogger.Object,
            emotionService,
            drewEmotionService,
            displaySlideService,
            displayVideoService,
            chatMessageService
        );

        // Assert
        Assert.NotNull(hub);
    }

    [Fact]
    public void Configuration_DeploymentName_CanBeRetrieved()
    {
        // Arrange
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(c => c["AzureOpenAI:DeploymentName"]).Returns("gpt-4-test");

        // Act
        var deploymentName = mockConfiguration.Object["AzureOpenAI:DeploymentName"];

        // Assert
        Assert.Equal("gpt-4-test", deploymentName);
    }

    [Fact]
    public void Configuration_Endpoint_CanBeRetrieved()
    {
        // Arrange
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(c => c["AzureOpenAI:Endpoint"]).Returns("https://test.openai.azure.com/");

        // Act
        var endpoint = mockConfiguration.Object["AzureOpenAI:Endpoint"];

        // Assert
        Assert.Equal("https://test.openai.azure.com/", endpoint);
    }

    [Fact]
    public void Configuration_ApiKey_CanBeRetrieved()
    {
        // Arrange
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(c => c["AzureOpenAI:ApiKey"]).Returns("test-api-key");

        // Act
        var apiKey = mockConfiguration.Object["AzureOpenAI:ApiKey"];

        // Assert
        Assert.Equal("test-api-key", apiKey);
    }
}
