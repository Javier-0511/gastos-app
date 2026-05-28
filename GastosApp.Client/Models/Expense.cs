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

    // Nullable: si se borra la categoría asociada, la FK deja este campo a
    // null (gasto "huérfano"). El cliente Supabase requiere que el tipo lo
    // admita para no fallar al deserializar.
    [Column("category_id")]
    public Guid? CategoryId { get; set; }

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
