using GastosApp.Client.Models;

namespace GastosApp.Client.Services;

public class AccountService
{
    private readonly SupabaseService _supabase;
    private readonly AuthService _auth;

    public AccountService(SupabaseService supabase, AuthService auth)
    {
        _supabase = supabase;
        _auth = auth;
    }

    public async Task<List<Account>> GetMyAccountsAsync()
    {
        var response = await _supabase.Client.From<Account>().Get();
        return response.Models;
    }

    public async Task<Account> CreateAccountAsync(string name, bool isShared)
    {
        var userIdString = _auth.CurrentUser?.Id
            ?? throw new InvalidOperationException("No hay usuario autenticado.");
        var userId = Guid.Parse(userIdString);

        var newAccount = new Account
        {
            Name = name,
            IsShared = isShared,
            OwnerId = userId
        };

        var insertResponse = await _supabase.Client.From<Account>().Insert(newAccount);
        var created = insertResponse.Models.First();

        var member = new AccountMember
        {
            AccountId = created.Id,
            UserId = userId,
            SharePercent = isShared ? 50m : 100m
        };

        await _supabase.Client.From<AccountMember>().Insert(member);

        return created;
    }
}
