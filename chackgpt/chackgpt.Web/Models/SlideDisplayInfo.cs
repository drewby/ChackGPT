namespace chackgpt.Web.Models;

/// <summary>
/// Represents information about a presentation slide to display
/// </summary>
public class SlideDisplayInfo
{
    /// <summary>
    /// The topic of the presentation
    /// </summary>
    public string Topic { get; set; } = string.Empty;
    
    /// <summary>
    /// The slide number
    /// </summary>
    public int SlideNumber { get; set; }
    
    /// <summary>
    /// The title of the slide
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The content of the slide in markdown format
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional notes or metadata about the slide
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Total number of slides in this topic (optional, for navigation)
    /// </summary>
    public int? TotalSlides { get; set; }

    /// <summary>
    /// The layout type for this slide (e.g., "hero-with-image", "text-only")
    /// </summary>
    public string? Layout { get; set; }

    /// <summary>
    /// Optional badge text to display (e.g., "RELEASED", "DEMO")
    /// </summary>
    public string? Badge { get; set; }

    /// <summary>
    /// Optional path to the main slide image
    /// </summary>
    public string? ImagePath { get; set; }

    /// <summary>
    /// Optional alt text for the slide image
    /// </summary>
    public string? ImageAlt { get; set; }

    /// <summary>
    /// Optional URL to display on the slide
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Description/subtitle for the slide
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Bullet points for the slide
    /// </summary>
    public List<string>? Bullets { get; set; }

    /// <summary>
    /// Optional sections for grid-based layouts
    /// </summary>
    public List<SlideSection>? Sections { get; set; }
}

/// <summary>
/// Represents a section in a grid-based slide layout
/// </summary>
public class SlideSection
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
