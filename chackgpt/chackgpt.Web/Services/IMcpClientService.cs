using ModelContextProtocol.Client;

namespace chackgpt.Web.Services;

/// <summary>
/// Interface for Model Context Protocol (MCP) client services.
/// Provides access to MCP tools from the ApiService server.
/// </summary>
public interface IMcpClientService : IAsyncDisposable
{
    /// <summary>
    /// Lists all available MCP tools from the ApiService server.
    /// </summary>
    /// <returns>List of MCP client tools.</returns>
    Task<IList<McpClientTool>> ListToolsAsync();
}
