using ReWare.Models;
using System;

public class PointsTransaction
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int PointsAdded { get; set; }
    public int PointsDeducted { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Description { get; set; }
    public virtual ApplicationUser User { get; set; }
}
