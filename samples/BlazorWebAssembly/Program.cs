using Blazor.WinOld;
using BlazorWebAssembly;
using Brism;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add WinOld Blazor components services
builder.Services.AddWinOldComponents();

builder.Services.AddBrism();

await builder.Build().RunAsync();
