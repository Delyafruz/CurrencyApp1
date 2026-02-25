using CurrencyApp1;
using CurrencyApp1.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient без BaseAddress — каждый сервис использует полные URL
builder.Services.AddScoped(sp => new HttpClient());

builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<CryptoService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
