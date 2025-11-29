using System.ComponentModel;

namespace chackgpt.ApiService.Tools.Presentation.Models;

/// <summary>
/// Represents the available presentation topics.
/// </summary>
public enum PresentationTopic
{
    /// <summary>
    /// .NET 10 overview and features
    /// </summary>
    [Description("dotnet10")]
    Dotnet10,

    /// <summary>
    /// .NET Aspire 13.0 polyglot platform features
    /// </summary>
    [Description("aspire13")]
    Aspire13,

    /// <summary>
    /// .NET 10 Platform (ASP.NET Core, Libraries, SDK & Tooling)
    /// </summary>
    [Description("dotnet10platform")]
    Dotnet10Platform,

    /// <summary>
    /// Intelligence (Microsoft Agent Framework, Model Context Protocol, MCP C# SDK)
    /// </summary>
    [Description("intelligence")]
    Intelligence,

    /// <summary>
    /// Summary of the .NET 10 intelligent apps ecosystem
    /// </summary>
    [Description("summary")]
    Summary
}

/// <summary>
/// Extension methods for PresentationTopic enum.
/// </summary>
public static class PresentationTopicExtensions
{
    /// <summary>
    /// Converts the PresentationTopic enum to its string identifier.
    /// </summary>
    public static string ToTopicId(this PresentationTopic topic)
    {
        return topic switch
        {
            PresentationTopic.Dotnet10 => "dotnet10",
            PresentationTopic.Aspire13 => "aspire13",
            PresentationTopic.Dotnet10Platform => "dotnet10platform",
            PresentationTopic.Intelligence => "intelligence",
            PresentationTopic.Summary => "summary",
            _ => throw new ArgumentOutOfRangeException(nameof(topic), topic, null)
        };
    }
}
