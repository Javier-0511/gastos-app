using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace GastosApp.Client.Models;

[Table("accounts")]
public class Account : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("is_shared")]
    public bool IsShared { get; set; }

    [Column("owner_id")]
    public Guid OwnerId { get; set; }

    [Column("created_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime CreatedAt { get; set; }
}
