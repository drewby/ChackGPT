using chackgpt.ApiService.Tools.Presentation.Models;
using chackgpt.ApiService.Tools.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace chackgpt.ApiService.Tools.Presentation;

/// <summary>
/// Endpoints for presentation content delivery via Model Context Protocol (MCP).
/// Provides structured .NET feature information for live presentations.
/// </summary>
public static class PresentationContentEndpoints
{
    /// <summary>
    /// Registers presentation content MCP tool endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapPresentationContentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/presentation")
            .WithTags("Presentation Content");

        // MCP Tool: GetPresentationSlide
        // Retrieves a specific slide by topic, slide number, and language
        group.MapGet("/slide/{topic}/{slideNumber:int}", GetPresentationSlide)
            .WithName("GetPresentationSlide")
            .WithDescription("Gets a presentation slide for the specified topic, slide number, and language. " +
                "Supported topics: dotnet10, csharp14, aspire13, aspnet10, dotnetlib10, sdktooling10. " +
                "Supported languages: en, jp (defaults to en)")
            .Produces<PresentationSlide>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // Helper endpoint to list available topics
        group.MapGet("/topics", GetAvailableTopics)
            .WithName("GetAvailableTopics")
            .WithDescription("Lists all available presentation topics")
            .Produces<string[]>(StatusCodes.Status200OK);

        return endpoints;
    }

    /// <summary>
    /// MCP Tool: Gets a presentation slide by topic, slide number, and language.
    /// This tool is designed to be invoked by AI agents via MCP for delivering
    /// structured presentation content during live demonstrations.
    /// </summary>
    /// <param name="topic">Topic identifier (dotnet10, csharp14, aspire13, aspnet10, dotnetlib10, sdktooling10)</param>
    /// <param name="slideNumber">Slide number (1-based)</param>
    /// <param name="language">Language code (en, jp) - defaults to en</param>
    /// <param name="service">The presentation service</param>
    /// <returns>The requested presentation slide</returns>
    private static IResult GetPresentationSlide(
        [FromRoute] string topic,
        [FromRoute] int slideNumber,
        [FromQuery] string? language,
        [FromServices] PresentationService service)
    {
        // Default language to English if not provided
        language ??= "en";

        // Validate language parameter
        var validLanguages = new[] { "en", "jp" };
        if (!validLanguages.Contains(language.ToLowerInvariant()))
        {
            return Results.Problem(
                detail: $"Invalid language '{language}'. Valid languages are: {string.Join(", ", validLanguages)}",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Language"
            );
        }

        // Validate topic parameter
        var validTopics = new[] { "dotnet10", "csharp14", "aspire13", "aspnet10", "dotnetlib10", "sdktooling10" };
        if (!validTopics.Contains(topic.ToLowerInvariant()))
        {
            return Results.Problem(
                detail: $"Invalid topic '{topic}'. Valid topics are: {string.Join(", ", validTopics)}",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Topic"
            );
        }

        // Validate slide number
        if (slideNumber < 1)
        {
            return Results.Problem(
                detail: "Slide number must be greater than 0",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Slide Number"
            );
        }

        var slide = service.GetSlide(topic, slideNumber, language);

        if (slide == null)
        {
            var totalSlides = service.GetSlideCount(topic, language);
            return Results.Problem(
                detail: $"Slide {slideNumber} not found for topic '{topic}' in language '{language}'. Valid range: 1-{totalSlides}",
                statusCode: StatusCodes.Status404NotFound,
                title: "Slide Not Found"
            );
        }

        return Results.Ok(slide);
    }

    /// <summary>
    /// Gets a list of all available presentation topics.
    /// </summary>
    private static IResult GetAvailableTopics([FromServices] PresentationService service)
    {
        var topics = service.GetAvailableTopics().ToArray();
        return Results.Ok(topics);
    }
}
