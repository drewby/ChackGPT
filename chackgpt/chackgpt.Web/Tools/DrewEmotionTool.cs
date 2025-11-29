using System.ComponentModel;
using chackgpt.Web.Models;
using chackgpt.Web.Services;
using Microsoft.Extensions.AI;

namespace chackgpt.Web.Tools;

/// <summary>
/// AI Tool that allows DrewGPT to set its emotional expression
/// This tool is invoked by the AI agent to change the avatar's displayed emotion
/// </summary>
public static class DrewEmotionTool
{
    /// <summary>
    /// Creates an AITool that can be used by ChatClientAgent to set DrewGPT's emotion
    /// </summary>
    /// <param name="emotionService">The emotion service to update</param>
    /// <param name="logger">Logger for tracking emotion changes</param>
    /// <returns>An AITool that the agent can invoke</returns>
    public static AITool CreateTool(IEmotionService<DrewEmotion> emotionService, ILogger logger)
    {
        return AIFunctionFactory.Create(
            (DrewEmotion emotion) =>
            {
                logger.LogInformation("🎭 SetDrewEmotion tool called: Changing emotion to {Emotion}", emotion);
                emotionService.SetEmotion(emotion);
                logger.LogInformation("✅ Drew emotion successfully changed to {Emotion}", emotion);
                return $"Drew emotion set to {emotion}";
            },
            name: "SetEmotion",
            description: "Sets your emotional expression to match the conversation context. " +
                        "Use this tool to make the avatar more engaging and expressive during conversations." +
                        "ALWAYS call this tool at least once in each response to ensure the avatar reflects your current emotion."
        );
    }
}
