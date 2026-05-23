using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace GastosApp.Client.Models;

[Table("account_members")]
public class AccountMember : BaseModel
{
    [PrimaryKey("account_id", true)]
    public Guid AccountId { get; set; }

    [PrimaryKey("user_id", true)]
    public Guid UserId { get; set; }

    [Column("share_percent")]
    public decimal SharePercent { get; set; }

    [Column("joined_at", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime JoinedAt { get; set; }
}
