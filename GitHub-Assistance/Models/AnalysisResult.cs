using System.Data;

namespace GitHub_Assistance.Models
{
    public class AnalysisResult
    {
        public List<string> Suggestions { get; set; } = new();
        public List<string> Vulnerabilities { get; set; } = new();
        public List<string> OutdatedLibraries { get; set; } = new();
        public List<string> SecretScanning { get; set;} = new();    
    }
    public class GitHubCodeScanningAlert
    {
        public int Number { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string State { get; set; }
        public string FixedAt { get; set; }
        public Rule Rule { get; set; }
        public Tool Tool { get; set; }
        public MostRecentInstance MostRecentInstance { get; set; }
    }

    public class Rule
    {
        public string Id { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string FullDescription { get; set; }
        public string Help { get; set; }
        public string SecuritySeverityLevel { get; set; }
    }

    public class Tool
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Version { get; set; }
    }

    public class MostRecentInstance
    {
        public string Ref { get; set; }
        public string AnalysisKey { get; set; }
        public string Environment { get; set; }
        public string Category { get; set; }
        public string State { get; set; }
        public string CommitSha { get; set; }
        public Message Message { get; set; }
        public Location Location { get; set; }
    }

    public class Message
    {
        public string Text { get; set; }
    }

    public class Location
    {
        public string Path { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
    }
}
