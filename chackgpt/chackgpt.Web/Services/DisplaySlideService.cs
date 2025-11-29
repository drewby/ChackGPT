using chackgpt.Web.Models;

namespace chackgpt.Web.Services;

/// <summary>
/// Service to manage and broadcast slide display events across the application
/// </summary>
// C# 14 Primary Constructor - eliminates boilerplate field assignments
public class DisplaySlideService(ILogger<DisplaySlideService> logger) : ISlideService
{
    private SlideDisplayInfo? _currentSlide;
    private readonly object _lock = new();
    
    /// <summary>
    /// Event raised when a slide should be displayed
    /// </summary>
    public event EventHandler<SlideDisplayInfo>? SlideDisplayRequested;
    
    /// <summary>
    /// Event raised when the slide popup should be closed
    /// </summary>
    public event EventHandler? SlideCloseRequested;

    /// <summary>
    /// Gets the currently displayed slide, if any
    /// </summary>
    public SlideDisplayInfo? CurrentSlide
    {
        get
        {
            lock (_lock)
            {
                return _currentSlide;
            }
        }
    }

    /// <summary>
    /// Displays a slide to all connected clients
    /// </summary>
    /// <param name="slideInfo">The slide information to display</param>
    public void DisplaySlide(SlideDisplayInfo slideInfo)
    {
        ArgumentNullException.ThrowIfNull(slideInfo); 
        
        lock (_lock)
        {
            _currentSlide = slideInfo;
            logger.LogInformation("📊 Displaying slide: {Topic} - Slide {SlideNumber}: {Title}", 
                slideInfo.Topic, slideInfo.SlideNumber, slideInfo.Title);
            SlideDisplayRequested?.Invoke(this, slideInfo);
            logger.LogInformation("📢 SlideDisplayRequested event fired to all subscribers");
        }
    }

    /// <summary>
    /// Closes the currently displayed slide
    /// </summary>
    public void CloseSlide()
    {
        lock (_lock)
        {
            if (_currentSlide != null)
            {
                logger.LogInformation("🚫 Closing slide: {Topic} - Slide {SlideNumber}", 
                    _currentSlide.Topic, _currentSlide.SlideNumber);
                _currentSlide = null;
                SlideCloseRequested?.Invoke(this, EventArgs.Empty);
                logger.LogInformation("📢 SlideCloseRequested event fired to all subscribers");
            }
        }
    }
}
