using Supabase.Gotrue;

namespace GastosApp.Client.Services;

public class AuthService
{
    private readonly SupabaseService _supabase;

    public event Action? OnAuthStateChanged;

    public AuthService(SupabaseService supabase)
    {
        _supabase = supabase;

        // Cuando Supabase detecta cambios de sesión, avisamos al resto de la app
        _supabase.Client.Auth.AddStateChangedListener((sender, changed) =>
        {
            OnAuthStateChanged?.Invoke();
        });
    }

    public Session? CurrentSession => _supabase.Client.Auth.CurrentSession;
    public User? CurrentUser => _supabase.Client.Auth.CurrentUser;
    public bool IsLoggedIn => CurrentSession is not null;

    public async Task<Session?> SignInAsync(string email, string password)
    {
        return await _supabase.Client.Auth.SignIn(email, password);
    }

    public async Task SignOutAsync()
    {
        await _supabase.Client.Auth.SignOut();
    }
}