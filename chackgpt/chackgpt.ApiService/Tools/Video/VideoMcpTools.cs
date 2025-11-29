using System.ComponentModel;
using chackgpt.ApiService.Tools.Video.Services;
using ModelContextProtocol.Server;

namespace chackgpt.ApiService.Tools.Video;

/// <summary>
/// MCP tools for video content delivery.
/// Exposes video metadata retrieval via Model Context Protocol.
/// Reference: https://learn.microsoft.com/en-us/azure/app-service/tutorial-ai-model-context-protocol-server-dotnet
/// </summary>
[McpServerToolType]
public class VideoMcpTools
{
    private readonly VideoService _videoService;
    private readonly ILogger<VideoMcpTools> _logger;

    public VideoMcpTools(
        VideoService videoService,
        ILogger<VideoMcpTools> logger)
    {
        _videoService = videoService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves video metadata by ID or title.
    /// MCP Tool: GetVideo
    /// </summary>
    /// <param name="identifier">Video ID or title</param>
    /// <returns>JSON representation of the video metadata</returns>
    [McpServerTool]
    [Description("Retrieves video metadata by ID or title. " +
        "Returns video information including id, title, description, and videoUrl. " +
        "Can search by exact video ID (e.g., 'what-will-happen-to-chack') or by title.")]
    public async Task<string> GetVideo(
        [Description("Video ID or title")] string identifier)
    {
        try
        {
            _logger.LogInformation("MCP Tool GetVideo invoked: identifier={Identifier}", identifier);

            // Try to find by ID first
            var video = _videoService.GetVideoById(identifier);

            // If not found by ID, try by title
            if (video == null)
            {
                video = _videoService.GetVideoByTitle(identifier);
            }

            if (video == null)
            {
                var error = $"Video with identifier '{identifier}' not found";
                _logger.LogWarning(error);
                return System.Text.Json.JsonSerializer.Serialize(new { error });
            }

            var json = System.Text.Json.JsonSerializer.Serialize(video);
            _logger.LogInformation("MCP Tool GetVideo completed successfully");
            
            return await Task.FromResult(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetVideo MCP tool");
            return System.Text.Json.JsonSerializer.Serialize(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Lists all available videos.
    /// MCP Tool: GetAllVideos
    /// </summary>
    /// <returns>JSON array of all available videos</returns>
    [McpServerTool]
    [Description("Lists all available videos. Returns an array of video metadata objects.")]
    public async Task<string> GetAllVideos()
    {
        try
        {
            _logger.LogInformation("MCP Tool GetAllVideos invoked");

            var videos = _videoService.GetAllVideos();
            var json = System.Text.Json.JsonSerializer.Serialize(videos);

            _logger.LogInformation("MCP Tool GetAllVideos completed successfully");
            
            return await Task.FromResult(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllVideos MCP tool");
            return System.Text.Json.JsonSerializer.Serialize(new { error = ex.Message });
        }
    }
}
