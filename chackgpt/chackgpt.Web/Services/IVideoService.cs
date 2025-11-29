using chackgpt.Web.Models;

namespace chackgpt.Web.Services;

/// <summary>
/// Interface for video display management services.
/// </summary>
public interface IVideoService
{
    /// <summary>
    /// Event raised when a video should be displayed.
    /// </summary>
    event EventHandler<VideoDisplayInfo>? VideoDisplayRequested;

    /// <summary>
    /// Event raised when the video popup should be closed.
    /// </summary>
    event EventHandler? VideoCloseRequested;

    /// <summary>
    /// Gets the currently displayed video, if any.
    /// </summary>
    VideoDisplayInfo? CurrentVideo { get; }

    /// <summary>
    /// Displays a video to all connected clients.
    /// </summary>
    /// <param name="videoInfo">The video information to display.</param>
    void DisplayVideo(VideoDisplayInfo videoInfo);

    /// <summary>
    /// Closes the currently displayed video.
    /// </summary>
    void CloseVideo();
}
