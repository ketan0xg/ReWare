using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using ReWare.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext db = new ApplicationDbContext();

    // NEW Landing page
    public ActionResult Landing()
    {
        // Only show approved & available items
        var featuredItems = db.Items
            .Where(i => i.ModerationStatus == "Approved" && i.AvailabilityStatus == "Available")
            .OrderByDescending(i => i.Id)
            .Include(i => i.Images)   // If multi-image
            .Take(6)
            .ToList();

        return View(featuredItems);
    }

    // (Optional) Keep Index redirecting to Landing
    public ActionResult Index()
    {
        return RedirectToAction("Landing");
    }

    public ActionResult About() => View();
    public ActionResult Contact() => View();
}
