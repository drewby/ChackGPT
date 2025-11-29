# ADR-001: Chat Interface Architecture

## Status

Accepted

## Context

ChackGPT is a digital avatar designed to teach audiences about .NET technologies during live presentations. The MVP (User Story US-001) requires a text-based chat interface with streaming responses that works reliably in presentation scenarios.

### Requirements
- Text-based conversational AI with streaming responses
- Real-time chat capabilities for immediate audience interaction
- Clean, accessible UI suitable for live presentations
- Integration with Azure OpenAI Service
- Built on .NET 10 and ASP.NET Core 10
- Orchestrated with .NET Aspire 9.5
- Support for presentation tools (slides, videos) during live demos

### Technical Considerations
1. **Architecture Complexity**: Initial planning suggested a multi-service architecture with separate Chat Service. However, for MVP, this adds unnecessary complexity without immediate value.

2. **Real-Time Communication**: Need streaming responses to provide engaging user experience during live presentations.

3. **UI Framework**: Need professional, accessible components that work well in presentation environments.

4. **Cloud-Native Requirements**: Must leverage Aspire 9.5 for orchestration, service discovery, and observability.

5. **Presentation Features**: Architecture supports MCP server integration for presentation tools (slides, videos) and avatar emotion display.

## Decision

### Simplified MVP Architecture

We will implement a **single-service architecture** for the MVP by integrating chat functionality directly into the existing `chackgpt.Web` Blazor Server project.

#### Core Components

1. **Blazor Server Web Application** (`chackgpt.Web`)
   - Hosts chat UI and SignalR hub in same project
   - Uses ASP.NET Core 10 with interactive server-side rendering
   - Simplifies deployment and debugging for MVP

2. **Microsoft Agent Framework Integration**
   - Uses `Microsoft.Agents.AI` package for agent-based AI orchestration
   - Multi-agent group chat workflow with ChackGPT and DrewGPT
   - Custom AspireKeywordGroupChatManager for keyword-based agent handoff
   - Provides streaming capabilities for token-by-token response delivery
   - Thread-based conversation state management (in-memory per connection)
   - Integrates with Azure OpenAI Service for GPT-4 capabilities

3. **SignalR for Real-Time Streaming**
   - ASP.NET Core 10 SignalR hub (`ChatHub`) for bidirectional communication
   - Streams OpenAI responses token-by-token to client
   - Provides immediate visual feedback during presentations
   - Built-in reconnection and error handling

4. **Microsoft FluentUI Blazor Components**
   - Professional Microsoft design language
   - Built-in accessibility (ARIA, keyboard navigation)
   - Responsive layout suitable for presentation displays
   - Components: `FluentStack`, `FluentCard`, `FluentTextArea`, `FluentButton`

5. **Aspire Orchestration**
   - Service discovery and health checks via existing `AppHost`
   - Observability dashboard for monitoring during development
   - Configuration management for Azure OpenAI settings
   - Prepares foundation for future microservices

#### Why Not Separate Chat Service?

- **Demo Speed**: Single project reduces complexity and accelerates delivery for presentation demos
- **Debugging**: Easier to debug and demo with fewer moving parts during live presentations
- **Deployment**: Simpler deployment for demonstration purposes
- **Sufficient for Purpose**: The current architecture meets the needs of a presentation demonstration project

#### Technology Justification

**Microsoft Agent Framework (`Microsoft.Agents.AI`)**
- Agent-based orchestration framework for AI conversations
- Multi-agent group chat workflow implementation
- Built-in streaming capabilities for real-time response delivery
- Thread-based state management for conversation context (in-memory)
- AI tool integration for presentation features (slides, videos, emotions)
- Integration with Azure OpenAI Service for GPT-4 conversations
- Demonstrates .NET 10 AI agent patterns for presentations

**ASP.NET Core 10 SignalR**
- Real-time bidirectional communication required for streaming responses
- Native integration with Blazor Server
- Excellent performance for WebSocket connections
- Automatic fallback to long polling if WebSocket unavailable
- **New in ASP.NET Core 10**: Enhanced performance and reliability improvements

**Microsoft FluentUI Blazor Components**
- Provides professional Microsoft design language
- Built-in accessibility features (WCAG 2.1 compliant)
- Consistent with Microsoft presentation aesthetics
- Active community and Microsoft support
- Responsive and presentation-friendly layouts

**Azure OpenAI Service**
- Enterprise-grade AI with GPT-4 capabilities
- Azure integration for security and compliance
- Streaming API support for real-time responses
- Content filtering and safety features
- Required for ChackGPT's conversational AI via Agent Framework

### Project Structure

```
chackgpt.Web/
├── Components/
│   ├── Pages/
│   │   ├── Chat.razor          # New chat UI page
│   │   └── [existing pages]
│   └── Layout/
│       └── NavMenu.razor        # Updated with chat link
├── Hubs/
│   └── ChatHub.cs               # New SignalR hub
├── Program.cs                   # Updated with services
└── chackgpt.Web.csproj          # Updated with packages
```

### Configuration

Agent Framework and Azure OpenAI settings managed via Aspire configuration:
- `AzureOpenAI:Endpoint` - Azure OpenAI service endpoint
- `AzureOpenAI:ApiKey` - API key (managed securely via Aspire secrets)
- `AzureOpenAI:DeploymentName` - GPT model deployment name
- Agent Framework configured via dependency injection with Azure OpenAI client

## Consequences

### Positive

1. **Fast Demo Development**: Single-service architecture accelerates development for presentation demos
2. **Simplified Debugging**: All chat logic in one project makes debugging easier during live presentations
3. **Multi-Agent Patterns**: Microsoft Agent Framework demonstrates group chat workflows with multiple AI agents
4. **Real-Time Experience**: SignalR provides excellent streaming response UX for live demos
5. **Professional UI**: FluentUI components provide polished, accessible interface
6. **Aspire Benefits**: Leverages orchestration, service discovery, and observability dashboard
7. **.NET 10 Showcase**: Demonstrates latest ASP.NET Core 10, Blazor, Agent Framework, and MCP features
8. **Presentation Tools**: MCP integration enables AI agents to display slides and videos during presentations

### Negative

1. **Tight Coupling**: Chat logic coupled with Web UI (acceptable for demo/presentation purposes)
2. **Azure Dependency**: Requires Azure OpenAI Service with API key
3. **State Management**: In-memory conversation state only (lost on disconnect)
4. **Not Production-Ready**: Architecture prioritizes demo value over production concerns
5. **Limited Scalability**: Single-service design not suitable for high-load scenarios

### Risks & Mitigations

**Risk**: Architecture not suitable for production use
- **Acceptance**: Project is explicitly a demo/experiment for presentations, not a production reference

**Risk**: Azure OpenAI API costs during development and demos
- **Mitigation**: Use consumption-based pricing; monitor usage during presentations

**Risk**: SignalR connection issues in different network environments
- **Mitigation**: SignalR automatic fallback to long polling; test in presentation venue beforehand

**Risk**: Conversation state lost on disconnect
- **Acceptance**: In-memory state is sufficient for short live presentation demos

## References

- #file:docs/project.description.md - Project scope and implementation details
- #file:docs/whats_new_in_aspnet_10.md - ASP.NET Core 10 features
- https://www.fluentui-blazor.net/ - FluentUI Blazor documentation
- https://aka.ms/dotnet/aspire - .NET Aspire documentation
- https://learn.microsoft.com/en-us/agent-framework/ - Microsoft Agent Framework documentation
