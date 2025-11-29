using chackgpt.Web.Models;

namespace chackgpt.Web.Services;

/// <summary>
/// Service to manage and broadcast video display events across the application
/// </summary>
// C# 14 Primary Constructor - eliminates boilerplate field assignments
public class DisplayVideoService(ILogger<DisplayVideoService> logger) : IVideoService
{
    private VideoDisplayInfo? _currentVideo;
    private readonly object _lock = new();
    
    /// <summary>
    /// Event raised when a video should be displayed
    /// </summary>
    public event EventHandler<VideoDisplayInfo>? VideoDisplayRequested;
    
    /// <summary>
    /// Event raised when the video popup should be closed
    /// </summary>
    public event EventHandler? VideoCloseRequested;

    /// <summary>
    /// Gets the currently displayed video, if any
    /// </summary>
    public VideoDisplayInfo? CurrentVideo
    {
        get
        {
            lock (_lock)
            {
                return _currentVideo;
            }
        }
    }

    /// <summary>
    /// Displays a video to all connected clients
    /// </summary>
    /// <param name="videoInfo">The video information to display</param>
    public void DisplayVideo(VideoDisplayInfo videoInfo)
    {
        ArgumentNullException.ThrowIfNull(videoInfo); 
        
        lock (_lock)
        {
            _currentVideo = videoInfo;
            logger.LogInformation("🎬 Displaying video: {Title} ({Id})", videoInfo.Title, videoInfo.Id);
            VideoDisplayRequested?.Invoke(this, videoInfo);
            logger.LogInformation("📢 VideoDisplayRequested event fired to all subscribers");
        }
    }

    /// <summary>
    /// Closes the currently displayed video
    /// </summary>
    public void CloseVideo()
    {
        lock (_lock)
        {
            if (_currentVideo != null)
            {
                logger.LogInformation("🚫 Closing video: {Title}", _currentVideo.Title);
                _currentVideo = null;
                VideoCloseRequested?.Invoke(this, EventArgs.Empty);
                logger.LogInformation("📢 VideoCloseRequested event fired to all subscribers");
            }
        }
    }
}
