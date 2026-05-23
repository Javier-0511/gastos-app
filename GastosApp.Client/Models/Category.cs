using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace GastosApp.Client.Models;

[Table("categories")]
public class Category : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("block")]
    public string Block { get; set; } = "";

    [Column("color")]
    public string? Color { get; set; }

    [Column("icon")]
    public string? Icon { get; set; }

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime CreatedAt { get; set; }
}
