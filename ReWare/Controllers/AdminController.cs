using ReWare.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext db = new ApplicationDbContext();

    public ActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Shows items waiting for moderation (approval/rejection).
    /// </summary>
    public ActionResult PendingItems()
    {
        var pendingItems = db.Items
            .Where(i => i.ModerationStatus == "Pending")
            .ToList();
        return View(pendingItems);
    }

    /// <summary>
    /// Shows swap requests still pending (admin is still moderating swaps in this version).
    /// If later you move swap approval to item owners, you can remove this.
    /// </summary>
    public ActionResult PendingSwaps()
    {
        var pendingSwaps = db.Swaps
            .Include(s => s.Item)
            .Include(s => s.Requester)
            .Where(s => s.Status == "Pending")
            .ToList();
        return View(pendingSwaps);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ApproveItem(int id)
    {
        var item = db.Items.Find(id);
        if (item != null && item.ModerationStatus == "Pending")
        {
            item.ModerationStatus = "Approved";
            // Ensure availability is set if not initialized
            if (string.IsNullOrEmpty(item.AvailabilityStatus))
                item.AvailabilityStatus = "Available";
            db.SaveChanges();
        }
        return RedirectToAction("PendingItems");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RejectItem(int id)
    {
        var item = db.Items.Find(id);
        if (item != null && item.ModerationStatus == "Pending")
        {
            item.ModerationStatus = "Rejected";
            // You might optionally also set AvailabilityStatus = "Unavailable"
            db.SaveChanges();
        }
        return RedirectToAction("PendingItems");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ApproveSwap(int id)
    {
        var swap = db.Swaps
            .Include(s => s.Item)
            .Include(s => s.Requester)
            .FirstOrDefault(s => s.Id == id);

        if (swap != null &&
            swap.Status == "Pending" &&
            swap.Item != null &&
            swap.Item.ModerationStatus == "Approved" &&
            swap.Item.AvailabilityStatus == "Available") // Only allow if item still available
        {
            swap.Status = "Approved";

            // Mark item as completed (no longer available for others)
            swap.Item.AvailabilityStatus = "Completed";

            // Points logic
            var owner = db.Users.Find(swap.Item.UploadedByUserId);
            var requester = db.Users.Find(swap.RequesterId);

            int pointsForSwap = 50; // TODO: extract to config/constants

            if (owner != null)
            {
                owner.Points += pointsForSwap;
                db.PointsTransactions.Add(new PointsTransaction
                {
                    UserId = owner.Id,
                    PointsAdded = pointsForSwap,
                    PointsDeducted = 0,
                    Description = "Points earned for approved swap"
                });
            }

            if (requester != null)
            {
                // Only deduct if they have enough (you might enforce having enough earlier)
                if (requester.Points >= pointsForSwap)
                {
                    requester.Points -= pointsForSwap;
                    db.PointsTransactions.Add(new PointsTransaction
                    {
                        UserId = requester.Id,
                        PointsAdded = 0,
                        PointsDeducted = pointsForSwap,
                        Description = "Points spent on swap"

                    });
                }
                else
                {
                    // Optional: rollback approval if insufficient points
                    // For now, we just approve without deduction OR you can reject:
                    // swap.Status = "Rejected"; return RedirectToAction("PendingSwaps");
                }
            }

            db.SaveChanges();
        }

        return RedirectToAction("PendingSwaps");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RejectSwap(int id)
    {
        var swap = db.Swaps.Find(id);
        if (swap != null && swap.Status == "Pending")
        {
            swap.Status = "Rejected";
            db.SaveChanges();
        }
        return RedirectToAction("PendingSwaps");
    }
}
