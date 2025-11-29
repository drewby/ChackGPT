using System.Text.Json;
using chackgpt.ApiService.Tools.Video.Models;

namespace chackgpt.ApiService.Tools.Video.Services;

/// <summary>
/// Service for managing and retrieving video content metadata.
/// Supports the Model Context Protocol (MCP) for video delivery.
/// </summary>
public class VideoService
{
    private readonly Dictionary<string, VideoContent> _videosById;
    private readonly Dictionary<string, VideoContent> _videosByTitle;
    private readonly ILogger<VideoService> _logger;

    public VideoService(ILogger<VideoService> logger)
    {
        _logger = logger;
        var videos = LoadVideoLibrary();
        _videosById = videos.ToDictionary(v => v.Id.ToLowerInvariant(), v => v);
        _videosByTitle = videos.ToDictionary(v => v.Title.ToLowerInvariant(), v => v);
    }

    /// <summary>
    /// Gets a video by its ID.
    /// </summary>
    /// <param name="id">The video identifier</param>
    /// <returns>The video content, or null if not found</returns>
    public VideoContent? GetVideoById(string id)
    {
        if (_videosById.TryGetValue(id.ToLowerInvariant(), out var video))
        {
            return video;
        }

        _logger.LogWarning("Video with ID '{Id}' not found", id);
        return null;
    }

    /// <summary>
    /// Gets a video by its title.
    /// </summary>
    /// <param name="title">The video title</param>
    /// <returns>The video content, or null if not found</returns>
    public VideoContent? GetVideoByTitle(string title)
    {
        if (_videosByTitle.TryGetValue(title.ToLowerInvariant(), out var video))
        {
            return video;
        }

        _logger.LogWarning("Video with title '{Title}' not found", title);
        return null;
    }

    /// <summary>
    /// Gets all available videos.
    /// </summary>
    public IEnumerable<VideoContent> GetAllVideos() => _videosById.Values;

    private List<VideoContent> LoadVideoLibrary()
    {
        try
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Tools", "Video", "Data", "video-library.json");
            
            if (!File.Exists(jsonPath))
            {
                _logger.LogError("Video library file not found at: {Path}", jsonPath);
                return [];
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var root = JsonSerializer.Deserialize<VideoLibraryRoot>(jsonContent, options);
            
            if (root?.Videos == null)
            {
                _logger.LogError("Failed to deserialize video library or no videos found");
                return [];
            }

            _logger.LogInformation("Loaded {Count} videos from library", root.Videos.Count);

            return root.Videos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading video library");
            return [];
        }
    }

    private class VideoLibraryRoot
    {
        public List<VideoContent> Videos { get; init; } = [];
    }
}
