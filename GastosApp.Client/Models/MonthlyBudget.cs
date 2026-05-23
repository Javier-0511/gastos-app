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

    [Column("amount")]
    public decimal Amount { get; set; }
}
