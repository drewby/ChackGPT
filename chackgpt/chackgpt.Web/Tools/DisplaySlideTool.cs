using System.ComponentModel;
using chackgpt.Web.Models;
using chackgpt.Web.Services;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace chackgpt.Web.Tools;

/// <summary>
/// AI Tool that allows ChackGPT to display presentation slides to the user
/// This tool is invoked by the AI agent to show slide content in a popup on the client
/// </summary>
public static class DisplaySlideTool
{
    /// <summary>
    /// Creates an AITool that can be used by ChatClientAgent to display presentation slides
    /// </summary>
    /// <param name="displaySlideService">The slide display service (interface for loose coupling)</param>
    /// <param name="logger">Logger for tracking slide displays</param>
    /// <returns>An AITool that the agent can invoke</returns>
    public static AITool CreateTool(ISlideService displaySlideService, ILogger logger)
    {
        return AIFunctionFactory.Create(
            (string topic, int slideNumber, string title, string? description, List<string>? bullets, string? layout, 
             string? badge, string? imagePath, string? imageAlt, string? url, List<SlideSection>? sections, string? content, string? notes, int? totalSlides) =>
            {
                logger.LogInformation("📊 DisplaySlide tool called: {Topic} - Slide {SlideNumber}/{TotalSlides}: {Title} (Layout: {Layout})", 
                    topic, slideNumber, totalSlides, title, layout ?? "text-only");
                
                SlideDisplayInfo slideInfo = new()
                {
                    Topic = topic,
                    SlideNumber = slideNumber,
                    Title = title,
                    Description = description,
                    Bullets = bullets,
                    Layout = layout,
                    Badge = badge,
                    ImagePath = imagePath,
                    ImageAlt = imageAlt,
                    Url = url,
                    Sections = sections,
                    Content = content ?? string.Empty,
                    Notes = notes,
                    TotalSlides = totalSlides
                };
                
                displaySlideService.DisplaySlide(slideInfo);
                logger.LogInformation("✅ Slide display successfully triggered");
                
                return $"Slide displayed: {topic} - Slide {slideNumber}" + (totalSlides.HasValue ? $" of {totalSlides}" : "");
            },
            name: "DisplaySlide",
            description: "Displays a presentation slide to the user in a popup window. " +
                        "Use this tool after retrieving slide content from the presentation MCP server (GetPresentationSlide) " +
                        "to show it to the user. Supports multiple layouts: 'hero-with-image' (split layout with image), " +
                        "'title-only' (minimal design), 'text-only' (traditional), 'grid-sections' (4-column grid with sections). " +
                        "Include all properties from the slide data: description, bullets, badge, imagePath, imageAlt, url, sections, and layout. " +
                        "For grid-sections layout, pass sections array with icon, title, and content. " +
                        "Include totalSlides parameter if known to enable Next Slide navigation."
        );
    }
}
