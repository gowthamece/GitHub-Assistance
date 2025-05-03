using Azure.Identity;
using GitHub_Assistance;
using GitHub_Assistance.Data;
using GitHub_Assistance.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.SemanticKernel;
using Octokit;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri")!);
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<GitHubAnalyzerAgent>();

var environment = builder.Environment;
if(environment.IsDevelopment())
{
    builder.Services.AddSingleton<KeyVaultService>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var keyVaultUrl = configuration["AzureAI:KeyVaultUrl"]
                          ?? throw new InvalidOperationException("AzureAI:KeyVaultUrl is not configured.");
        return new KeyVaultService(keyVaultUrl);
    }); 
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
