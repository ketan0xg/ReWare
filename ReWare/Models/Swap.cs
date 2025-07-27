using ReWare.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Swap
{
    public int Id { get; set; }

    public string RequesterId { get; set; }
    [ForeignKey("RequesterId")]
    public virtual ApplicationUser Requester { get; set; }

    public int ItemId { get; set; }
    [ForeignKey("ItemId")]
    public virtual Item Item { get; set; }

    public string Status { get; set; } // Pending / Approved / Rejected
}
