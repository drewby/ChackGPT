using chackgpt.ApiService.Tools.Presentation;
using chackgpt.ApiService.Tools.Presentation.Services;
using chackgpt.ApiService.Tools.Video;
using chackgpt.ApiService.Tools.Video.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register MCP tool services for presentation and video content
// These services provide structured content for live presentations via Model Context Protocol
builder.Services.AddSingleton<PresentationService>();
builder.Services.AddSingleton<VideoService>();

// Configure MCP Server with HTTP transport (Streamable HTTP)
// Reference: https://learn.microsoft.com/en-us/azure/app-service/tutorial-ai-model-context-protocol-server-dotnet
builder.Services.AddMcpServer()
    .WithHttpTransport() // Streamable HTTP transport for remote MCP clients
    .WithToolsFromAssembly(); // Auto-discover all classes marked with [McpServerToolType]

// Enable CORS for MCP server (required for streamable HTTP transport)
// Allows MCP clients (including those in Web app) to connect to this server
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Enable static files for presentation images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Tools", "Presentation", "Images")),
    RequestPath = "/images/presentations"
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Map MCP server endpoint at /api/mcp
// This exposes all tools marked with [McpServerTool] via Model Context Protocol
// MCP clients can connect to this endpoint to discover and invoke tools
app.MapMcp("/api/mcp");
app.UseCors();

// Map REST API endpoints for direct HTTP access (backward compatibility)
// These provide the same functionality as MCP tools but via traditional REST APIs
app.MapPresentationContentEndpoints();
app.MapVideoContentEndpoints();

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Make Program class accessible to integration tests
public partial class Program { }
