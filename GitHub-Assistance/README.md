# GitHubAnalyzerAgent

The `GitHubAnalyzerAgent` class is a service designed to analyze GitHub repositories for vulnerabilities, outdated dependencies, and code quality improvements. It uses the GitHub API, NuGet API, and OpenAI for various analyses.

## Features
- Fetches code scanning alerts from GitHub.
- Scans for secret leaks in repositories.
- Checks for outdated NuGet dependencies.
- Analyzes code quality using OpenAI.

---

## Class Overview

### Constructor
```csharp
public GitHubAnalyzerAgent()