using GastosApp.Client.Models;

namespace GastosApp.Client.Services;

public class ExpenseService
{
    private readonly SupabaseService _supabase;
    private readonly AuthService _auth;

    public ExpenseService(SupabaseService supabase, AuthService auth)
    {
        _supabase = supabase;
        _auth = auth;
    }

    public async Task<Expense> CreateAsync(
        Guid accountId,
        Guid categoryId,
        string description,
        decimal amount,
        DateTime expenseDate)
    {
        var userIdString = _auth.CurrentUser?.Id
            ?? throw new InvalidOperationException("No hay usuario autenticado.");
        var userId = Guid.Parse(userIdString);

        var newExpense = new Expense
        {
            AccountId = accountId,
            CategoryId = categoryId,
            PaidBy = userId,
            Description = description,
            Amount = amount,
            ExpenseDate = expenseDate.Date
        };

        var response = await _supabase.Client.From<Expense>().Insert(newExpense);
        return response.Models.First();
    }
}
