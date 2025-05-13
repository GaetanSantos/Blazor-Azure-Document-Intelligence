using BlazorDocumentIntelligence;
using BlazorDocumentIntelligence.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IAzureDocumentIntelligenceService, AzureDocumentIntelligenceService>();

await builder.Build().RunAsync();
