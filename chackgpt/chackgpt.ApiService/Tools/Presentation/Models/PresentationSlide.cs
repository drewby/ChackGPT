namespace chackgpt.ApiService.Tools.Presentation.Models;

/// <summary>
/// Represents a single presentation slide with structured content.
/// </summary>
public class PresentationSlide
{
    /// <summary>
    /// The title of the slide.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// A description or subtitle for the slide.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// A list of 4-5 bullet points highlighting key information.
    /// </summary>
    public List<string>? Bullets { get; init; }

    /// <summary>
    /// The current slide number (1-based).
    /// </summary>
    public required int CurrentSlideNumber { get; init; }

    /// <summary>
    /// The total number of slides in this topic.
    /// </summary>
    public required int TotalSlides { get; init; }

    /// <summary>
    /// The topic identifier for this slide.
    /// </summary>
    public required string Topic { get; init; }

    /// <summary>
    /// The layout type for this slide (e.g., "hero-with-image", "text-only", "centered-image", "title-only").
    /// </summary>
    public string? Layout { get; init; }

    /// <summary>
    /// Optional badge text to display (e.g., "RELEASED", "DEMO").
    /// </summary>
    public string? Badge { get; init; }

    /// <summary>
    /// Optional path to the main slide image.
    /// </summary>
    public string? ImagePath { get; init; }

    /// <summary>
    /// Optional alt text for the slide image (accessibility).
    /// </summary>
    public string? ImageAlt { get; init; }

    /// <summary>
    /// Optional URL to display on the slide.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Optional sections for grid-based layouts with icons and content.
    /// </summary>
    public List<SlideSection>? Sections { get; init; }
}

/// <summary>
/// Represents a section in a grid-based slide layout.
/// </summary>
public class SlideSection
{
    /// <summary>
    /// The icon identifier for this section.
    /// </summary>
    public required string Icon { get; init; }

    /// <summary>
    /// The title of this section.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The content/description for this section.
    /// </summary>
    public required string Content { get; init; }
}

/// <summary>
/// Represents a collection of slides for a specific topic.
/// </summary>
public class PresentationDeck
{
    /// <summary>
    /// The topic identifier (e.g., "dotnet10", "csharp14").
    /// </summary>
    public required string Topic { get; init; }

    /// <summary>
    /// The collection of slides for this topic.
    /// </summary>
    public required List<PresentationSlide> Slides { get; init; }
}
