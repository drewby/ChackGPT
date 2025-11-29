using System.ComponentModel;
using chackgpt.Web.Models;
using chackgpt.Web.Services;
using Microsoft.Extensions.AI;

namespace chackgpt.Web.Tools;

/// <summary>
/// AI Tool that allows ChackGPT to display videos to the user
/// This tool is invoked by the AI agent to show video content in a full-screen player on the client
/// </summary>
public static class DisplayVideoTool
{
    /// <summary>
    /// Creates an AITool that can be used by ChatClientAgent to display videos
    /// </summary>
    /// <param name="displayVideoService">The video display service (interface for loose coupling)</param>
    /// <param name="logger">Logger for tracking video displays</param>
    /// <returns>An AITool that the agent can invoke</returns>
    public static AITool CreateTool(IVideoService displayVideoService, ILogger logger)
    {
        return AIFunctionFactory.Create(
            (string id, string title, string description, string videoUrl) =>
            {
                logger.LogInformation("🎬 DisplayVideo tool called: {Title} ({Id})", title, id);
                
                VideoDisplayInfo videoInfo = new() 
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    VideoUrl = videoUrl
                };
                
                displayVideoService.DisplayVideo(videoInfo);
                logger.LogInformation("✅ Video display successfully triggered");
                
                return $"Video displayed: {title}";
            },
            name: "DisplayVideo",
            description: "Displays a video to the user in a full-screen player with black background. " +
                        "Use this tool after retrieving video metadata from the video MCP server (GetVideo) " +
                        "to show it to the user. The video will be displayed with 100% height in the center " +
                        "of the content area (excluding the navigation sidebar)."
        );
    }
}
