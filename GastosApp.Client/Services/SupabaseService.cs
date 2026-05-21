namespace GastosApp.Client.Services;

public class SupabaseService
{
    public Supabase.Client Client { get; }

    public SupabaseService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"]
            ?? throw new InvalidOperationException("Falta Supabase:Url en appsettings.json");
        var anonKey = configuration["Supabase:AnonKey"]
            ?? throw new InvalidOperationException("Falta Supabase:AnonKey en appsettings.json");

        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = true
        };

        Client = new Supabase.Client(url, anonKey, options);
    }

    public async Task InitializeAsync()
    {
        await Client.InitializeAsync();
    }
}