using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GastosApp.Client;
using GastosApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Registramos nuestro servicio Supabase como singleton (uno solo para toda la app)
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<AccountService>();
var host = builder.Build();

// Inicializamos la conexión de Supabase antes de arrancar
var supabase = host.Services.GetRequiredService<SupabaseService>();
await supabase.InitializeAsync();

await host.RunAsync();