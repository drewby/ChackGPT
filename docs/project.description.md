# Project Overview

## Project Name

ChackGPT

## Purpose

ChackGPT is the digital avatar of Chack, a .NET and C# expert software engineer. ChackGPT serves as an AI chat interface that can teach audiences about .NET and more. The system is designed to support live presentations and demonstrations, providing information about new features in .NET 10, C# 14, ASP.NET Core 10, Aspire 9.5, and related technologies.

## Scope

### Current Implementation
- Text-based chat interface with streaming responses
- Multi-agent group chat (ChackGPT and DrewGPT) with keyword-based handoff
- Built using .NET Aspire 9.5 for cloud-native orchestration and service discovery
- Azure OpenAI integration for conversational AI
- Real-time chat using SignalR with token-by-token streaming
- Blazor Server web UI with FluentUI components
- Model Context Protocol (MCP) server in ApiService exposing:
  - Presentation slide retrieval tools
  - Video content discovery and metadata tools
- AI agent tools:
  - Display presentation slides
  - Play presentation videos
  - Set avatar emotions (ChackGPT and DrewGPT)
- In-memory conversation state (per SignalR connection)
- Local development with Aspire orchestration and dashboard

## Technologies

### Current Implementation
- **Runtime**: .NET 10, ASP.NET Core 10
- **Web Framework**: Blazor Server with interactive server-side rendering
- **AI**: Microsoft Agent Framework (Microsoft.Agents.AI), Azure OpenAI Service (GPT-4)
- **Real-Time Communication**: SignalR for streaming chat responses
- **UI Framework**: Microsoft FluentUI Blazor components
- **MCP**: Model Context Protocol server (ModelContextProtocol.AspNetCore) in ApiService
- **Orchestration**: .NET Aspire 9.5 (service orchestration, discovery, observability)
- **Languages**: C# 14, HTML, JavaScript, CSS
- **Testing**: xUnit, Moq
- **Development**: Docker dev container

## Key Features

### Chat Interface
- Text-based conversational AI with streaming responses (token-by-token)
- Multi-agent conversation: ChackGPT and DrewGPT collaborate with keyword-based handoff
- Real-time communication using SignalR
- Clean, accessible UI with FluentUI Blazor components
- Markdown rendering with syntax highlighting for code blocks
- In-memory conversation history (per SignalR connection)

### Presentation Tools
- AI agents can display presentation slides via MCP tools
- AI agents can play videos via MCP tools
- Dynamic avatar emotions for ChackGPT and DrewGPT
- Broadcast services sync UI state across components
- Slide and video content served from ApiService

### Model Context Protocol (MCP)
- ApiService exposes MCP server at `/api/mcp`
- Presentation slide tools: search, list, and retrieve slides
- Video content tools: search and get video metadata
- HTTP transport for remote MCP clients
- Auto-discovery of MCP tools via attributes

### .NET 10 & C# 14 Demo
- Includes demo code showing C# 14 features:
  - Field-backed properties
  - Extension members
  - Null-conditional assignment
  - Top-level statements with file-based app properties
- ChackGPT chatmode for guided modernization demo
