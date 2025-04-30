using GitHub_Assistance.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Octokit;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace GitHub_Assistance.Services;

public class GitHubAnalyzerAgent
{
    private readonly GitHubClient _githubClient;
    private readonly Kernel _kernel;
    private static readonly HttpClient client = new HttpClient();
    public GitHubAnalyzerAgent()
    {
        _githubClient = new GitHubClient(new ProductHeaderValue("GitHubAnalyzerAgent"));
        var modelId = "<Model>";
        var endpoint = "<AI Model Endpoint>";
        var apiKey = "<AI deployment API Key>";   
        var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
        _kernel = builder.Build();
    }

    public async Task<AnalysisResult> AnalyzeRepository(string repoUrl)
    {
        try
        {
            var (owner, name) = ParseGitHubRepoUrl(repoUrl);

            var repo = await _githubClient.Repository.Get(owner, name);
            var localPath = CloneRepository(repo.CloneUrl);
            var suggestions = new List<string>();
            //  var suggestions = await AnalyzeCodeQuality(localPath);
            var secretScan = await ScanForSecret(owner, name);
              var vulnerabilities = await ScanForVulnerabilities(owner, name);
         //   var vulnerabilities = new List<string>();
          
            var outdatedLibraries = await CheckDependencies(localPath);

            return new AnalysisResult
            {
                Suggestions = suggestions,
                Vulnerabilities = vulnerabilities,
                OutdatedLibraries = outdatedLibraries
            };
        }
        catch(Exception ex)
        {
            return new AnalysisResult();
        }
    }
    private string CloneRepository(string cloneUrl)
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolder);

        var gitClone = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone {cloneUrl} .",
            WorkingDirectory = tempFolder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        var process = Process.Start(gitClone);
        process.WaitForExit();

        return tempFolder;
    }

    //private async Task<List<string>> ScanForVulnerabilities(string localPath)
    //{
    //    //try
    //    //{
    //    //    var codeqlScan = new ProcessStartInfo
    //    //    {
    //    //        FileName = "codeql",
    //    //        Arguments = $"database analyze --format=sarifv2.1.0 {localPath}",
    //    //        RedirectStandardOutput = true,
    //    //        RedirectStandardError = true,
    //    //        UseShellExecute = false
    //    //    };

    //    //    var process = Process.Start(codeqlScan);
    //    //    var output = await process.StandardOutput.ReadToEndAsync();
    //    //    process.WaitForExit();

    //    //    return output.Split('\n').Where(l => l.Contains("[Security]")).ToList();
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    return new List<string>();
    //    //}

    //    var vulnerabilities = new List<string>();

    //    try
    //    {
    //        var alerts = await _githubClient.Repository.CodeScanning.GetAlertsForRepository(owner, repoName);

    //        foreach (var alert in alerts)
    //        {
    //            vulnerabilities.Add($"{alert.RuleId}: {alert.MostRecentInstance?.Message}");
    //        }
    //    }
    //    catch (NotFoundException)
    //    {
    //        vulnerabilities.Add("No code scanning alerts found or repository does not have code scanning enabled.");
    //    }
    //    catch (Exception ex)
    //    {
    //        vulnerabilities.Add($"Error fetching vulnerabilities: {ex.Message}");
    //    }

    //    return vulnerabilities;
    //}
    private async Task<List<string>> ScanForVulnerabilities(string owner, string repoName)
    {
        var vulnerabilities = new List<string>();
        var token = "<Github PAT token>";

        if (string.IsNullOrEmpty(token))
        {
            vulnerabilities.Add("GitHub token is missing. Please set GITHUB_TOKEN environment variable.");
            return vulnerabilities;
        }

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("GitHubAnalyzerAgent");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

            var url = $"https://api.github.com/repos/{owner}/{repoName}/code-scanning/alerts";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                vulnerabilities.Add($"Failed to fetch vulnerabilities: {response.StatusCode} - {response.ReasonPhrase}");
                return vulnerabilities;
            }

            var content = await response.Content.ReadAsStringAsync();
            var alerts = System.Text.Json.JsonSerializer.Deserialize<List<GitHubCodeScanningAlert>>(content, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (alerts == null || alerts.Count == 0)
            {
                vulnerabilities.Add("No code scanning alerts found or not enabled.");
                return vulnerabilities;
            }

            foreach (var alert in alerts)
            {
                vulnerabilities.Add($"Alert Number: {alert.Number}, Rule: {alert.Rule.Name}, Severity: {alert.Rule.Severity}");
            }
        }
        catch (Exception ex)
        {
            vulnerabilities.Add($"Error scanning vulnerabilities: {ex.Message}");
        }

        return vulnerabilities;
    }

    private async Task<List<string>> ScanForSecret(string owner, string repoName)
    {
        var vulnerabilities = new List<string>();
        var token = "<Github PAT token>";

        if (string.IsNullOrEmpty(token))
        {
            vulnerabilities.Add("GitHub token is missing. Please set GITHUB_TOKEN environment variable.");
            return vulnerabilities;
        }

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("GitHubAnalyzerAgent");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

            var url = $"https://api.github.com/repos/{owner}/{repoName}/secret-scanning/alerts";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                vulnerabilities.Add($"Failed to fetch vulnerabilities: {response.StatusCode} - {response.ReasonPhrase}");
                return vulnerabilities;
            }

            var content = await response.Content.ReadAsStringAsync();
            var alerts = System.Text.Json.JsonSerializer.Deserialize<List<SecretScanningAlert>>(content, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (alerts == null || alerts.Count == 0)
            {
                vulnerabilities.Add("No code scanning alerts found or not enabled.");
                return vulnerabilities;
            }

            foreach (var alert in alerts)
            {
                vulnerabilities.Add($"{alert.SecretType}: {alert.SecretTypeDisplayName}");
            }
        }
        catch (Exception ex)
        {
            vulnerabilities.Add($"Error scanning vulnerabilities: {ex.Message}");
        }

        return vulnerabilities;
    }

    //public static async Task<List<CodeScanningAlert>> GetCodeScanningAlertsAsync(string owner, string repo, string personalAccessToken)
    //{

    //    // Set up HttpClient headers.  These are CRUCIAL.
    //    client.DefaultRequestHeaders.Accept.Clear();
    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json")); // Recommended media type
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalAccessToken);
    //    client.DefaultRequestHeaders.UserAgent.TryParseAdd("My-GitHub-App"); // **IMPORTANT:** Change this to your application's name

    //    // Construct the API URL.
    //    string url = $"https://api.github.com/repos/{owner}/{repo}/code-scanning/alerts";

    //    try
    //    {
    //        // Make the GET request.
    //        HttpResponseMessage response = await client.GetAsync(url);
    //        response.EnsureSuccessStatusCode(); // Ensure a successful response (throws exception on error)

    //        // Read the response content as a string.
    //        string responseBody = await response.Content.ReadAsStringAsync();

    //        // Deserialize the JSON response into a list of CodeScanningAlert objects.
    //        // Use the System.Text.Json library.
    //        List<CodeScanningAlert> alerts = JsonSerializer.Deserialize<List<CodeScanningAlert>>(responseBody);
    //        return alerts;
    //    }
    //    catch (HttpRequestException e)
    //    {
    //        Console.WriteLine($"Error: {e.Message}");
    //        return null; // Or throw, depending on your error handling strategy
    //    }
    //    catch (JsonException e)
    //    {
    //        Console.WriteLine($"JSON Deserialization Error: {e.Message}");
    //        return null;
    //    }
    //}


    private async Task<List<string>> CheckDependencies(string localPath)
    {
        var outdated = new List<string>();
        var files = Directory.GetFiles(localPath, "*.csproj", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            var packageRefs = System.Text.RegularExpressions.Regex.Matches(text, "<PackageReference Include=\"(.*?)\" Version=\"(.*?)\" />");

            foreach (System.Text.RegularExpressions.Match match in packageRefs)
            {
                var packageName = match.Groups[1].Value;
                var currentVersion = match.Groups[2].Value;

                var latestVersion = await GetLatestNuGetVersion(packageName);

                if (currentVersion != latestVersion)
                {
                    outdated.Add($"{packageName}: {currentVersion} => {latestVersion}");
                }
            }
        }

        return outdated;
    }
    private async Task<string> GetLatestNuGetVersion(string packageName)
    {
        using var client = new HttpClient();
        var json = await client.GetStringAsync($"https://api.nuget.org/v3-flatcontainer/{packageName.ToLower()}/index.json");
        var versions = System.Text.Json.JsonDocument.Parse(json).RootElement.GetProperty("versions");
        return versions.EnumerateArray().Last().GetString();
    }
    private (string owner, string name) ParseGitHubRepoUrl(string url)
    {
        var uri = new Uri(url);
        var parts = uri.AbsolutePath.Trim('/').Split('/');
        return (parts[0], parts[1]);
    }
    private async Task<List<string>> AnalyzeCodeQuality(string localPath)
    {
        try
        {
            var codeSummary = string.Join("\n", Directory.GetFiles(localPath, "*.cs", SearchOption.AllDirectories)
                .Select(File.ReadAllText)
                .Take(10));

            var prompt = $"Analyze the following C# code for improvements and suggest best practices:\n\n{codeSummary}";

            var result = await _kernel.InvokePromptAsync(prompt);
            return new List<string> { result.GetValue<string>() ?? string.Empty };
        }
       catch(Exception ex)
        {
            return new List<string>();
        }
    }

    // (everything else same)
}

