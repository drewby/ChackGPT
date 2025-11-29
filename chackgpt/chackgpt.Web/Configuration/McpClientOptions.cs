namespace chackgpt.Web.Configuration;

/// <summary>
/// Configuration options for Model Context Protocol (MCP) client.
/// Defines connection settings for ApiService MCP tool endpoints.
/// </summary>
public class McpClientOptions
{
    /// <summary>
    /// Base URL for the ApiService that hosts MCP tools.
    /// Uses Aspire service discovery format (e.g., "https+http://apiservice").
    /// </summary>
    public string ApiServiceBaseUrl { get; set; } = "https+http://apiservice";

    /// <summary>
    /// Timeout for MCP tool invocations in seconds.
    /// Default is 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
