namespace chackgpt.Web.Models;

/// <summary>
/// Represents information about a video to display
/// </summary>
public class VideoDisplayInfo
{
    /// <summary>
    /// The video ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The title of the video
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The description of the video
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The URL to the video file
    /// </summary>
    public string VideoUrl { get; set; } = string.Empty;
}
