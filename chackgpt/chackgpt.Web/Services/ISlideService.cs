using chackgpt.Web.Models;

namespace chackgpt.Web.Services;

/// <summary>
/// Interface for slide display management services.
/// </summary>
public interface ISlideService
{
    /// <summary>
    /// Event raised when a slide should be displayed.
    /// </summary>
    event EventHandler<SlideDisplayInfo>? SlideDisplayRequested;

    /// <summary>
    /// Event raised when the slide popup should be closed.
    /// </summary>
    event EventHandler? SlideCloseRequested;

    /// <summary>
    /// Gets the currently displayed slide, if any.
    /// </summary>
    SlideDisplayInfo? CurrentSlide { get; }

    /// <summary>
    /// Displays a slide to all connected clients.
    /// </summary>
    /// <param name="slideInfo">The slide information to display.</param>
    void DisplaySlide(SlideDisplayInfo slideInfo);

    /// <summary>
    /// Closes the currently displayed slide.
    /// </summary>
    void CloseSlide();
}
