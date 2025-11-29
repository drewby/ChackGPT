using chackgpt.ApiService.Tools.Video.Models;
using chackgpt.ApiService.Tools.Video.Services;
using Microsoft.AspNetCore.Mvc;

namespace chackgpt.ApiService.Tools.Video;

/// <summary>
/// Endpoints for video content delivery via Model Context Protocol (MCP).
/// Provides video metadata for pre-generated content during presentations.
/// </summary>
public static class VideoContentEndpoints
{
    /// <summary>
    /// Registers video content MCP tool endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapVideoContentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/video")
            .WithTags("Video Content");

        // MCP Tool: GetVideo
        // Retrieves video metadata by ID or title
        group.MapGet("/{identifier}", GetVideo)
            .WithName("GetVideo")
            .WithDescription("Gets video metadata by ID or title. Identifier can be a video ID (e.g., 'what-will-happen-to-chack') or a video title.")
            .Produces<VideoContent>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // Helper endpoint to list all videos
        group.MapGet("", GetAllVideos)
            .WithName("GetAllVideos")
            .WithDescription("Lists all available videos")
            .Produces<VideoContent[]>(StatusCodes.Status200OK);

        return endpoints;
    }

    /// <summary>
    /// MCP Tool: Gets video metadata by ID or title.
    /// This tool is designed to be invoked by AI agents via MCP for delivering
    /// video content during live presentations.
    /// </summary>
    /// <param name="identifier">The video ID or title</param>
    /// <param name="service">The video service</param>
    /// <returns>The video metadata</returns>
    private static IResult GetVideo(
        [FromRoute] string identifier,
        [FromServices] VideoService service)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Results.Problem(
                detail: "Video identifier cannot be empty",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Identifier"
            );
        }

        // Try to find by ID first
        var video = service.GetVideoById(identifier);

        // If not found by ID, try by title
        if (video == null)
        {
            video = service.GetVideoByTitle(identifier);
        }

        if (video == null)
        {
            return Results.Problem(
                detail: $"Video with identifier '{identifier}' not found. Please check the ID or title.",
                statusCode: StatusCodes.Status404NotFound,
                title: "Video Not Found"
            );
        }

        return Results.Ok(video);
    }

    /// <summary>
    /// Gets a list of all available videos.
    /// </summary>
    private static IResult GetAllVideos([FromServices] VideoService service)
    {
        var videos = service.GetAllVideos().ToArray();
        return Results.Ok(videos);
    }
}
