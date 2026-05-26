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
    /// Devuelve el registro mensual de esa cuenta-mes, o null si no existe.
    /// Contiene previsión, saldo inicial e ingreso (los dos últimos son nullable).
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
    /// Actualiza la previsión (amount). Crea el registro si no existe.
    /// </summary>
    public Task<MonthlyBudget> SetAmountAsync(Guid accountId, int year, int month, decimal amount) =>
        UpsertFieldsAsync(accountId, year, month, b => b.Amount = amount, defaultAmount: amount);

    /// <summary>
    /// Actualiza el saldo inicial. Crea el registro si no existe.
    /// </summary>
    public Task<MonthlyBudget> SetOpeningBalanceAsync(Guid accountId, int year, int month, decimal openingBalance) =>
        UpsertFieldsAsync(accountId, year, month, b => b.OpeningBalance = openingBalance);

    /// <summary>
    /// Actualiza la nómina del mes. Crea el registro si no existe.
    /// </summary>
    public Task<MonthlyBudget> SetIncomeAsync(Guid accountId, int year, int month, decimal income) =>
        UpsertFieldsAsync(accountId, year, month, b => b.Income = income);

    // Helper interno: busca el registro y le aplica una mutación, o lo crea si no existe.
    // `defaultAmount` solo se usa al crear (la columna amount es NOT NULL en la BBDD; si
    // creamos un registro porque el usuario está editando saldo/nómina antes que previsión,
    // ponemos amount = 0 por defecto).
    private async Task<MonthlyBudget> UpsertFieldsAsync(
        Guid accountId,
        int year,
        int month,
        Action<MonthlyBudget> mutate,
        decimal defaultAmount = 0m)
    {
        var existing = await GetForMonthAsync(accountId, year, month);

        if (existing is null)
        {
            var newBudget = new MonthlyBudget
            {
                AccountId = accountId,
                Year = year,
                Month = month,
                Amount = defaultAmount
            };
            mutate(newBudget);

            var response = await _supabase.Client.From<MonthlyBudget>().Insert(newBudget);
            return response.Models.First();
        }
        else
        {
            mutate(existing);
            var response = await existing.Update<MonthlyBudget>();
            return response.Models.First();
        }
    }
}
