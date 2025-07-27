using Microsoft.AspNet.Identity;
using ReWare.Models;
using ReWare.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

[Authorize]
public class DashboardController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    public ActionResult Index()
    {
        var userId = User.Identity.GetUserId();
        var user = db.Users.Find(userId);

        var myItems = db.Items.Where(i => i.UploadedByUserId == userId).ToList();
        var mySwaps = db.Swaps.Include("Item").Where(s => s.RequesterId == userId).ToList();

        var transactions = db.PointsTransactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).ToList();

        var vm = new DashboardViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Points = user.Points,
            MyItems = myItems,
            MySwaps = mySwaps,
            MyTransactions = transactions
        };


        return View(vm);
    }
}
