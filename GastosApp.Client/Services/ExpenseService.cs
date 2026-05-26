using GastosApp.Client.Models;
using Supabase.Postgrest;

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

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        var response = await _supabase.Client
            .From<Expense>()
            .Where(e => e.Id == id)
            .Single();

        return response;
    }

    public async Task<List<Expense>> GetByMonthAsync(int year, int month)
    {
        // Rango [primer día del mes, primer día del mes siguiente).
        // Postgres compara `date` con literales tipo 'YYYY-MM-DD'.
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var response = await _supabase.Client
            .From<Expense>()
            .Where(e => e.ExpenseDate >= from && e.ExpenseDate < to)
            .Order(e => e.ExpenseDate, Constants.Ordering.Descending)
            .Get();

        return response.Models;
    }

    /// <summary>
    /// Gastos de la cuenta entre dos fechas [from, to) (to exclusivo).
    /// Útil para gráficos de histórico (varios meses) en el dashboard.
    /// </summary>
    public async Task<List<Expense>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var response = await _supabase.Client
            .From<Expense>()
            .Where(e => e.ExpenseDate >= from.Date)
            .Where(e => e.ExpenseDate < to.Date)
            .Order(e => e.ExpenseDate, Constants.Ordering.Ascending)
            .Get();

        return response.Models;
    }

    public async Task<Expense> UpdateAsync(
        Guid id,
        Guid accountId,
        Guid categoryId,
        string description,
        decimal amount,
        DateTime expenseDate)
    {
        var existing = await _supabase.Client
            .From<Expense>()
            .Where(e => e.Id == id)
            .Single();

        if (existing is null)
            throw new InvalidOperationException("Gasto no encontrado.");

        existing.AccountId = accountId;
        existing.CategoryId = categoryId;
        existing.Description = description;
        existing.Amount = amount;
        existing.ExpenseDate = expenseDate.Date;

        var response = await existing.Update<Expense>();
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabase.Client
            .From<Expense>()
            .Where(e => e.Id == id)
            .Delete();
    }
}
