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

    // Normaliza un DateTime a "fecha de calendario en UTC" para evitar que
    // el cliente Supabase aplique conversión de TZ y desplace el día. El
    // <input type="date"> devuelve fechas como medianoche local (Kind=Unspecified),
    // que al serializar a UTC restan/suman horas y pueden cambiar el día.
    private static DateTime AsUtcDate(DateTime d) =>
        DateTime.SpecifyKind(d.Date, DateTimeKind.Utc);

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
            ExpenseDate = AsUtcDate(expenseDate)
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
        var from = AsUtcDate(new DateTime(year, month, 1));
        var to = AsUtcDate(from.AddMonths(1));

        var response = await _supabase.Client
            .From<Expense>()
            .Where(e => e.ExpenseDate >= from)
            .Where(e => e.ExpenseDate < to)
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
        var fromUtc = AsUtcDate(from);
        var toUtc = AsUtcDate(to);

        var response = await _supabase.Client
            .From<Expense>()
            .Where(e => e.ExpenseDate >= fromUtc)
            .Where(e => e.ExpenseDate < toUtc)
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
        existing.ExpenseDate = AsUtcDate(expenseDate);

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
