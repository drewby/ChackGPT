# ChackGPT - Your .NET Technology Companion 🤖

> A conversational AI assistant built with .NET 10, ASP.NET Core 10, and .NET Aspire 9.5 to help the presenters teach audiences about the latest Microsoft .NET technologies during live presentations.

## ⚠️ Important Note

This project was a **fun experiment** created to support a presentation on .NET 10 and C# 14. It demonstrates various new features and technologies in an entertaining way during live demos. **This is not intended as a reference example or best practice guide** for building production .NET applications or agentic AI systems. The architecture and patterns used here prioritize presentation value and feature demonstration over production concerns like scalability, security hardening, and enterprise architecture patterns.

Fair warning: you'll find some creative hacks, clever workarounds, and yes, a few pieces of code that might make you cringe. 😅 That's all part of the experimental fun!

If you're looking to build production applications, please refer to official Microsoft documentation and architectural guidance.

## 🎯 Overview

ChackGPT is the digital avatar of Chack, a .NET and C# expert software engineer. It provides an engaging, real-time chat interface powered by Azure OpenAI and demonstrates cutting-edge .NET technologies including Blazor Server, SignalR streaming, Microsoft Agent Framework, and .NET Aspire orchestration.

## ✨ Key Features

- **Real-Time Streaming Chat** - Token-by-token response delivery using SignalR for engaging live presentations
- **Multi-Agent Conversations** - ChackGPT and DrewGPT agents can collaborate with keyword-based handoff
- **Azure OpenAI Integration** - Powered by GPT-4 for conversational AI responses
- **Model Context Protocol (MCP)** - ApiService exposes presentation slides and videos via MCP for AI agent tool use
- **FluentUI Components** - Professional, accessible UI with Microsoft design language
- **Markdown Support** - Rich text rendering with syntax highlighting for code blocks
- **.NET Aspire Orchestration** - Cloud-native service orchestration and observability
- **Presentation Tools** - AI agents can display slides and play videos during presentations
- **Avatar Emotions** - Dynamic emotion display for ChackGPT and DrewGPT avatars

## 🚀 Technology Stack

| Category | Technologies |
|----------|-------------|
| **Runtime** | .NET 10, ASP.NET Core 10 |
| **Web Framework** | Blazor Server |
| **AI** | Microsoft Agent Framework, Azure OpenAI Service |
| **Real-Time** | SignalR |
| **UI** | Microsoft FluentUI Blazor |
| **Orchestration** | .NET Aspire 9.5 |
| **Markdown** | Markdig, Prism.js |
| **Testing** | xUnit, Moq |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service) account with API access
- Visual Studio 2025 or Visual Studio Code with C# Dev Kit

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd dotnet10
```

### 2. Configure Azure OpenAI

Update the configuration in `chackgpt/chackgpt.Web/appsettings.Development.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-openai-endpoint.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "gpt-4"
  }
}
```

**Security Note**: For production, use Azure Key Vault or User Secrets instead of storing the API key in configuration files:

```bash
cd chackgpt/chackgpt.Web
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key-here"
```

### 3. Run the Application

Using .NET Aspire (Recommended):

```bash
cd chackgpt
dotnet run --project chackgpt.AppHost
```

This will:
- Start the Aspire AppHost orchestrator
- Launch the Web application
- Launch the API service
- Open the Aspire Dashboard for monitoring

Or run just the Web application:

```bash
cd chackgpt/chackgpt.Web
dotnet run
```

### 4. Access the Application

- **Web Application**: https://localhost:7266
- **Aspire Dashboard**: https://localhost:17063 (when using AppHost)
- **Chat Interface**: Navigate to the "Chat" page in the application

## Project Structure

```
chackgpt/
├── chackgpt.AppHost/          # .NET Aspire orchestration host
├── chackgpt.ServiceDefaults/  # Shared service configuration
├── chackgpt.Web/              # Blazor Server web application
│   ├── Components/
│   │   ├── Pages/
│   │   │   └── Chat.razor     # Chat interface page
│   │   └── Layout/            # Application layout components
│   ├── Hubs/
│   │   └── ChatHub.cs         # SignalR hub for real-time chat
│   └── Program.cs             # Application startup and configuration
├── chackgpt.ApiService/       # Weather API service (demo)
└── chackgpt.Web.Tests/        # Unit tests for Web project
```

## Architecture

ChackGPT uses a simplified MVP architecture with the following key components:

### Microsoft Agent Framework
The application uses Microsoft Agent Framework (`Microsoft.Agents.AI`) for AI agent orchestration. The implementation includes:
- Multi-agent group chat workflow (ChackGPT and DrewGPT)
- Custom keyword-based agent handoff logic
- Streaming response delivery
- Thread-based conversation state (in-memory per connection)
- AI tool integration (emotion display, slide presentation, video playback)

### SignalR Streaming
Real-time communication is powered by ASP.NET Core 10 SignalR, which:
- Streams AI responses token-by-token to the client
- Provides automatic reconnection handling
- Supports WebSocket with fallback to long polling

### FluentUI Components
The UI is built with Microsoft FluentUI Blazor components, offering:
- Microsoft design language consistency
- Built-in accessibility (WCAG 2.1 compliant)
- Responsive layouts for presentations

For detailed architecture decisions, see [ADR-001: Chat Interface Architecture](docs/adr/adr-001-chat-interface-architecture.md).

## Testing

Run the test suite:

```bash
cd chackgpt/chackgpt.Web.Tests
dotnet test
```

All tests should pass with output similar to:
```
Test summary: total: 6, failed: 0, succeeded: 6, skipped: 0
```

## Usage

1. Navigate to the **Chat** page in the application
2. Type your question about .NET technologies in the input area
3. Press **Send** or press Enter (without Shift)
4. Watch as ChackGPT streams its response in real-time
5. Continue the conversation with follow-up questions

### Example Questions
- "What's new in .NET 10?"
- "Explain the benefits of .NET Aspire"
- "How does Blazor Server rendering work?"
- "What are the key features of C# 14?"
- "Show me a code example of using minimal APIs in ASP.NET Core 10"

### Markdown Support

ChackGPT's responses support rich markdown formatting, making technical content easier to read during presentations:

**Supported Markdown Features:**
- **Headers** (H1-H6): `# Heading 1`, `## Heading 2`, etc.
- **Bold text**: `**bold**` renders as **bold**
- **Italic text**: `*italic*` renders as *italic*
- **Lists**: Ordered (`1. Item`) and unordered (`- Item`)
- **Links**: `[Link text](url)` renders as clickable links
- **Inline code**: `` `code` `` renders with monospace font
- **Code blocks**: Multi-line code with syntax highlighting

**Syntax Highlighting:**

Code blocks support syntax highlighting for common languages:
- C# (`.cs` files)
- JavaScript (`.js` files)
- JSON (`.json` files)
- Bash (shell scripts)
- PowerShell (`.ps1` files)

Example request: "Show me a minimal API example in C# with dependency injection"

**Rendering Notes:**
- User messages display as plain text
- Assistant (ChackGPT) messages render with full markdown support
- Content is rendered safely using GitHub Flavored Markdown standards
- Code blocks are automatically highlighted using Prism.js

## 🤝 Contributing

This project demonstrates .NET 10 and ASP.NET Core 10 features for educational purposes. Contributions are welcome!

## Documentation

- [Project Description](docs/project.description.md) - Full project scope and user stories
- [ADR-001: Chat Interface Architecture](docs/adr/adr-001-chat-interface-architecture.md)
- [What's New in .NET 10](docs/whats_new_in_dotnet_libraries_for_10.md)
- [What's New in ASP.NET Core 10](docs/whats_new_in_aspnet_10.md)
- [What's New in C# 14](docs/whats_new_in_chsarp_14.md)
- [What's New in .NET Aspire 9.5](docs/whats_new_in_aspire_9.5.md)

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

---

**Built with ❤️ using .NET 10, Azure OpenAI, and .NET Aspire 9.5**
