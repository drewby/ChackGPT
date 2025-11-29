using Azure.AI.OpenAI;
using chackgpt.Web.Configuration;
using chackgpt.Web.Constants;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System.ClientModel;

namespace chackgpt.Web.Services;

/// <summary>
/// Factory service for creating and configuring AI agents.
/// Uses C# 14 primary constructor to eliminate boilerplate field assignments.
/// </summary>
// C# 14 Primary Constructor - eliminates boilerplate field assignments
public class AgentFactory(
    IOptions<AzureOpenAIOptions> azureOpenAIOptions,
    ILogger<AgentFactory> logger)
{
    private readonly AzureOpenAIOptions _azureOpenAIOptions = azureOpenAIOptions.Value;
    private readonly ILogger<AgentFactory> _logger = logger;

    /// <summary>
    /// Creates a configured AI agent with the specified configuration and tools.
    /// </summary>
    /// <param name="agentName">Name of the agent.</param>
    /// <param name="instructions">System instructions/prompt for the agent.</param>
    /// <param name="tools">AI tools to provide to the agent. Uses C# 14 params collections for flexible tool registration.</param>
    /// <returns>Configured AI agent.</returns>
    // C# 14 params collections - flexible tool registration
    public AIAgent CreateAgent(
        string agentName,
        string instructions,
        params AITool[] tools)
    {
        ArgumentNullException.ThrowIfNull(agentName); // C# 11 - concise null argument checks
        ArgumentNullException.ThrowIfNull(instructions);

        // Parse the service version string to enum
        AzureOpenAIClientOptions.ServiceVersion versionEnum = ParseServiceVersion(_azureOpenAIOptions.ServiceVersion);
        AzureOpenAIClientOptions clientOptions = new(versionEnum); // C# 9 target-typed new - clearer type declaration

        AzureOpenAIClient azureClient = new(
            new Uri(_azureOpenAIOptions.Endpoint),
            new ApiKeyCredential(_azureOpenAIOptions.ApiKey),
            clientOptions);
        
        IChatClient chatClient = azureClient
            .GetChatClient(_azureOpenAIOptions.DeploymentName)
            .AsIChatClient();
        
        chatClient = new OpenTelemetryChatClient(chatClient, sourceName: ChatConstants.TelemetrySourceName);

        // Create the agent with the provided tools
        AIAgent agent = new ChatClientAgent(
            chatClient,
            instructions: instructions,
            name: agentName,
            tools: [.. tools] // C# 14 collection expressions - already in use
        )
        .AsBuilder()
        .UseOpenTelemetry(sourceName: ChatConstants.TelemetrySourceName)
        .Build();

        _logger.LogInformation("✅ {AgentName} agent created with {ToolCount} tools", agentName, tools.Length);

        return agent;
    }

    /// <summary>
    /// Parses Azure OpenAI service version string to the corresponding enum value.
    /// </summary>
    /// <param name="version">Service version string (e.g., "2024-06-01", "2025-01-01-preview").</param>
    /// <returns>Corresponding ServiceVersion enum value.</returns>
    /// <exception cref="ArgumentException">Thrown when the version string is not supported.</exception>
    private static AzureOpenAIClientOptions.ServiceVersion ParseServiceVersion(string version)
    {
        return version switch
        {
            "2024-06-01" => AzureOpenAIClientOptions.ServiceVersion.V2024_06_01,
            "2024-08-01-preview" => AzureOpenAIClientOptions.ServiceVersion.V2024_08_01_Preview,
            "2024-09-01-preview" => AzureOpenAIClientOptions.ServiceVersion.V2024_09_01_Preview,
            "2024-10-01-preview" => AzureOpenAIClientOptions.ServiceVersion.V2024_10_01_Preview,
            "2024-10-21" => AzureOpenAIClientOptions.ServiceVersion.V2024_10_21,
            "2024-12-01-preview" => AzureOpenAIClientOptions.ServiceVersion.V2024_12_01_Preview,
            "2025-01-01-preview" => AzureOpenAIClientOptions.ServiceVersion.V2025_01_01_Preview,
            "2025-03-01-preview" => AzureOpenAIClientOptions.ServiceVersion.V2025_03_01_Preview,
            "2025-04-01-preview" => AzureOpenAIClientOptions.ServiceVersion.V2025_04_01_Preview,
            _ => throw new ArgumentException($"Unsupported service version: {version}", nameof(version))
        };
    }
}
