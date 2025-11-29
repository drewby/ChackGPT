using System.ComponentModel;

namespace chackgpt.ApiService.Tools.Presentation.Models;

/// <summary>
/// Represents the available presentation languages.
/// </summary>
public enum PresentationLanguage
{
    // /// <summary>
    // /// English language
    // /// </summary>
    // [Description("en")]
    // English,

    /// <summary>
    /// Japanese language
    /// </summary>
    [Description("jp")]
    Japanese
}

/// <summary>
/// Extension methods for PresentationLanguage enum.
/// </summary>
public static class PresentationLanguageExtensions
{
    /// <summary>
    /// Converts the PresentationLanguage enum to its language code.
    /// </summary>
    public static string ToLanguageCode(this PresentationLanguage language)
    {
        return language switch
        {
            // PresentationLanguage.English => "en",
            PresentationLanguage.Japanese => "jp",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }

    /// <summary>
    /// Gets the file suffix for the language (empty for English, ".jp" for Japanese).
    /// </summary>
    public static string ToFileSuffix(this PresentationLanguage language)
    {
        return language switch
        {
            // PresentationLanguage.English => "",
            PresentationLanguage.Japanese => ".jp",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
}
