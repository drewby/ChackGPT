namespace chackgpt.ApiService.Tools.Video.Models;

/// <summary>
/// Represents video content metadata for MCP video delivery.
/// </summary>
public class VideoContent
{
    /// <summary>
    /// Unique identifier for the video.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The title of the video.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// A description of the video content.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// The URL where the video can be accessed.
    /// </summary>
    public required string VideoUrl { get; init; }
}
