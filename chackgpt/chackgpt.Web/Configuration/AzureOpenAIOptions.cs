using System.ComponentModel.DataAnnotations;

namespace chackgpt.Web.Configuration;

/// <summary>
/// Configuration options for Azure OpenAI service integration.
/// Uses .NET 10 Options pattern with DataAnnotations validation.
/// </summary>
public class AzureOpenAIOptions
{
    /// <summary>
    /// Azure OpenAI endpoint URL (e.g., https://your-resource.openai.azure.com/).
    /// </summary>
    [Required(ErrorMessage = "AzureOpenAI:Endpoint configuration is required")]
    [Url(ErrorMessage = "AzureOpenAI:Endpoint must be a valid URL")]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI API key for authentication.
    /// </summary>
    [Required(ErrorMessage = "AzureOpenAI:ApiKey configuration is required")]
    [MinLength(1, ErrorMessage = "AzureOpenAI:ApiKey cannot be empty")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI deployment name for the model to use.
    /// </summary>
    [Required(ErrorMessage = "AzureOpenAI:DeploymentName configuration is required")]
    [MinLength(1, ErrorMessage = "AzureOpenAI:DeploymentName cannot be empty")]
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI service version (e.g., 2024-06-01, 2025-01-01-preview).
    /// </summary>
    [Required(ErrorMessage = "AzureOpenAI:ServiceVersion configuration is required")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}(-preview)?$", 
        ErrorMessage = "AzureOpenAI:ServiceVersion must be in format YYYY-MM-DD or YYYY-MM-DD-preview")]
    public string ServiceVersion { get; set; } = string.Empty;
}
