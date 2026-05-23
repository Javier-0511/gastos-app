using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace GastosApp.Client.Models;

[Table("expenses")]
public class Expense : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("paid_by")]
    public Guid PaidBy { get; set; }

    [Column("description")]
    public string Description { get; set; } = "";

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("expense_date")]
    public DateTime ExpenseDate { get; set; }

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime CreatedAt { get; set; }
}
