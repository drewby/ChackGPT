using System.ComponentModel.DataAnnotations;

namespace chackgpt.Web.Configuration;

/// <summary>
/// Configuration options for AI agent settings.
/// </summary>
public class AgentOptions
{
    /// <summary>
    /// Name of the agent.
    /// </summary>
    [Required(ErrorMessage = "Agent name is required")]
    [MinLength(1, ErrorMessage = "Agent name cannot be empty")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// System instructions/prompt for the agent.
    /// </summary>
    [Required(ErrorMessage = "Agent instructions are required")]
    [MinLength(10, ErrorMessage = "Agent instructions must be at least 10 characters")]
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of iterations for group chat workflows.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Maximum iterations must be between 1 and 100")]
    public int MaximumIterationCount { get; set; } = 5;
}
