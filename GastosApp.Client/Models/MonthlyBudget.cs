using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace GastosApp.Client.Models;

[Table("monthly_budgets")]
public class MonthlyBudget : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Column("year")]
    public int Year { get; set; }

    [Column("month")]
    public int Month { get; set; }

    // Previsión / aporte conjunto del mes.
    // - Compartida: aporte total al banco entre todos los miembros
    //   (cada uno aporta amount × share_percent).
    // - Personal: actualmente sin uso. Reservado.
    [Column("amount")]
    public decimal Amount { get; set; }

    // Saldo al inicio del mes (lo que había en la cuenta).
    // Primera vez: lo introduces a mano. Después se sugiere a partir del
    // saldo final del mes anterior.
    [Column("opening_balance")]
    public decimal? OpeningBalance { get; set; }

    // Ingreso del mes (nómina). Solo aplica a cuentas personales.
    [Column("income")]
    public decimal? Income { get; set; }
}
