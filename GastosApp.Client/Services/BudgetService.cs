using GastosApp.Client.Models;

namespace GastosApp.Client.Services;

public class BudgetService
{
    private readonly SupabaseService _supabase;

    public BudgetService(SupabaseService supabase)
    {
        _supabase = supabase;
    }

    /// <summary>
    /// Devuelve la previsión de esa cuenta-mes, o null si no existe.
    /// </summary>
    public async Task<MonthlyBudget?> GetForMonthAsync(Guid accountId, int year, int month)
    {
        // El cliente Supabase .NET no traduce bien `&&` compuestos a la sintaxis
        // PostgREST (genera paréntesis mal balanceados → PGRST100). Encadenamos
        // los `.Where(...)` por separado, que PostgREST combina como AND.
        var response = await _supabase.Client
            .From<MonthlyBudget>()
            .Where(b => b.AccountId == accountId)
            .Where(b => b.Year == year)
            .Where(b => b.Month == month)
            .Single();

        return response;
    }

    /// <summary>
    /// Crea o actualiza la previsión de esa cuenta-mes.
    /// Usa upsert con la unique constraint (account_id, year, month) como conflict target.
    /// </summary>
    public async Task<MonthlyBudget> UpsertAsync(Guid accountId, int year, int month, decimal amount)
    {
        // Buscamos si ya existe para hacer update en su lugar (más predecible que
        // el upsert del cliente .NET, que requiere mapear bien la unique constraint).
        var existing = await GetForMonthAsync(accountId, year, month);

        if (existing is null)
        {
            var newBudget = new MonthlyBudget
            {
                AccountId = accountId,
                Year = year,
                Month = month,
                Amount = amount
            };

            var response = await _supabase.Client.From<MonthlyBudget>().Insert(newBudget);
            return response.Models.First();
        }
        else
        {
            existing.Amount = amount;
            var response = await existing.Update<MonthlyBudget>();
            return response.Models.First();
        }
    }
}
