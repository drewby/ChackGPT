using chackgpt.Web.Configuration;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ConfigOptions = chackgpt.Web.Configuration.McpClientOptions;

namespace chackgpt.Web.Services;

/// <summary>
/// MCP client service for connecting to ApiService MCP server.
/// Implements Model Context Protocol client using HttpClientTransport with Aspire service discovery.
/// Reference: https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-client
/// </summary>
// C# 14 Primary Constructor - eliminates boilerplate field assignments
public class McpClientService(
    IOptions<ConfigOptions> options,
    ILoggerFactory loggerFactory,
    ILogger<McpClientService> logger,
    IConfiguration configuration) : IMcpClientService
{
    // C# 11 - concise null argument checks for all primary constructor parameters
    private readonly ConfigOptions _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    private readonly ILogger<McpClientService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    
    private McpClient? _mcpClient;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    /// <summary>
    /// Gets the MCP client instance, initializing it if necessary.
    /// </summary>
    private async Task<McpClient> GetMcpClientAsync()
    {
        if (_mcpClient != null)
        {
            return _mcpClient;
        }

        await _initLock.WaitAsync();
        try
        {
            if (_mcpClient != null)
            {
                return _mcpClient;
            }

            // Resolve apiservice endpoint from Aspire service discovery configuration
            // Aspire exposes resolved endpoints via configuration like "services:apiservice:http:0"
            // This provides the actual endpoint URL (e.g., "http://localhost:5344") instead of the logical name
            var resolvedEndpoint = _configuration["services:apiservice:http:0"]
                ?? _configuration["services:apiservice:https:0"]
                ?? _options.ApiServiceBaseUrl; // Fallback to configured URL

            if (string.IsNullOrEmpty(resolvedEndpoint))
            {
                throw new InvalidOperationException(
                    "Unable to resolve apiservice endpoint from Aspire service discovery. " +
                    "Ensure AddServiceDefaults() is called and apiservice is running.");
            }

            Uri mcpEndpoint = new(new Uri(resolvedEndpoint), "/api/mcp"); 
            _logger.LogInformation("Initializing MCP client connection to {Endpoint}", mcpEndpoint);

            // Create MCP client with HttpClientTransport for Streamable HTTP 
            HttpClientTransportOptions transportOptions = new()
            {
                Endpoint = mcpEndpoint
            };

            HttpClientTransport transport = new(transportOptions, _loggerFactory);
            
            ModelContextProtocol.Client.McpClientOptions clientOptions = new() 
            {
                ClientInfo = new Implementation
                {
                    Name = "ChackGPT Web Client",
                    Version = "1.0.0"
                }
            };

            _mcpClient = await McpClient.CreateAsync(transport, clientOptions, _loggerFactory);

            _logger.LogInformation("MCP client connected successfully");
            
            return _mcpClient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MCP client");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// Lists all available MCP tools from the ApiService server.
    /// </summary>
    /// <returns>List of MCP client tools</returns>
    public async Task<IList<McpClientTool>> ListToolsAsync()
    {
        try
        {
            var client = await GetMcpClientAsync();
            var tools = await client.ListToolsAsync();
            
            _logger.LogInformation("Retrieved {Count} tools from MCP server", tools.Count);
            
            return tools;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing MCP tools");
            return Array.Empty<McpClientTool>();
        }
    }

    /// <summary>
    /// Disposes the MCP client connection.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_mcpClient != null)
        {
            await _mcpClient.DisposeAsync();
            _mcpClient = null;
        }
        
        _initLock.Dispose();
    }
}
