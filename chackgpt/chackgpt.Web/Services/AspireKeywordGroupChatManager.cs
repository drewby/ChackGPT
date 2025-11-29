using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace chackgpt.Web.Services;

/// <summary>
/// Custom group chat manager that enforces the presence of the keyword "aspire" in conversation.
/// Extends RoundRobinGroupChatManager to control multi-agent chat flow based on keyword detection.
/// </summary>
/// <remarks>
/// This manager terminates the conversation if the keyword "aspire" is not mentioned,
/// preventing agents from continuing without discussing the relevant topic.
/// Demonstrates .NET 10 AI Agents workflow customization capabilities.
/// </remarks>
public class AspireKeywordGroupChatManager : RoundRobinGroupChatManager
{
    private int _currentIteration = 0;
    private static int _internalMaximumIterationCount = 5;
    /// <summary>
    /// Initializes a new instance of the AspireKeywordGroupChatManager.
    /// </summary>
    /// <param name="agents">The list of AI agents participating in the group chat.</param>
    public AspireKeywordGroupChatManager(IReadOnlyList<Microsoft.Agents.AI.AIAgent> agents) 
        : base(agents)
  {
  }

    /// <summary>
    /// Determines whether the group chat conversation should terminate based on keyword presence.
    /// </summary>
    /// <param name="history">The chat message history to analyze.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>
    /// True if the conversation should terminate (keyword not found or max iterations reached),
    /// false if the conversation should continue.
    /// </returns>
    /// <remarks>
    /// Termination logic:
    /// - First iteration always continues (allows agents to establish conversation)
    /// - Terminates if maximum iteration count is reached
    /// - Terminates if "aspire" keyword is NOT mentioned in non-system messages
    /// This ensures agents stay focused on the Aspire topic.
    /// </remarks>
    protected override ValueTask<bool> ShouldTerminateAsync(
        IReadOnlyList<ChatMessage> history, 
        CancellationToken cancellationToken = default)
    {
        // Increment iteration count
        _currentIteration++;

        if (_currentIteration == 1)
        {
            // Always allow the first iteration
            return ValueTask.FromResult(false);
        }

        // Always enforce maximum iteration count
        if (_currentIteration >= _internalMaximumIterationCount)
        {
            // Set next MaximumIterationCount to 3
            _internalMaximumIterationCount = 3;
            return ValueTask.FromResult(true);
        }

        /// Check if "future of AI" appears in any non-system message in the chat history
        /// if so, set _maximumIterationCount to 5
        bool containsFutureOfAI = history
            .Where(msg => msg.Role != ChatRole.System)
            .Any(msg => msg.Text?.Contains("future of AI", StringComparison.OrdinalIgnoreCase) == true);

        if (containsFutureOfAI)
        {
            _internalMaximumIterationCount = 5;
        }

        // Check if "aspire" appears in any non-system message in the chat history
        bool containsAspire = history
            .Where(msg => msg.Role != ChatRole.System)
            .Any(msg => msg.Text?.Contains("aspire", StringComparison.OrdinalIgnoreCase) == true);

        // Terminate (stop conversation) if "aspire" was NOT mentioned
        // This prevents agents from continuing the conversation without the keyword
        return ValueTask.FromResult(!containsAspire);
    }
}
