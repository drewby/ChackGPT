namespace chackgpt.Web.Models;

/// <summary>
/// Represents the emotional states that ChackGPT can display
/// </summary>
public enum ChackEmotion
{
    /// <summary>
    /// Default neutral expression - use for general conversation
    /// </summary>
    Neutral,
    
    /// <summary>
    /// Happy expression - use when discussing positive news, successes, or welcoming users
    /// </summary>
    Happy,
    
    /// <summary>
    /// Excited expression - use when discussing exciting new features, major announcements, or impressive capabilities
    /// </summary>
    Excited,
    
    /// <summary>
    /// Sad expression - use when discussing deprecated features, breaking changes, or limitations
    /// </summary>
    Sad,
    
    /// <summary>
    /// Angry expression - use sparingly, perhaps when discussing bugs or frustrating issues
    /// </summary>
    Angry,
    
    /// <summary>
    /// Surprised expression - use when revealing unexpected facts or impressive performance improvements
    /// </summary>
    Surprised,
    
    /// <summary>
    /// Love expression - use when discussing favorite features or expressing appreciation for user engagement
    /// </summary>
    Love,
    
    /// <summary>
    /// Evil expression - use humorously when discussing powerful features or "dark magic" under the hood
    /// </summary>
    Evil,
    
    /// <summary>
    /// Heavy metal expression - RESERVED for when user asks "What will happen to Chack?" or similar existential questions
    /// </summary>
    HeavyMetal,
    
    /// <summary>
    /// Introspection expression - use when discussing complex architectural decisions or deep technical concepts
    /// </summary>
    Introspection
}
