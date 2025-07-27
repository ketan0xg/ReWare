using Microsoft.AspNet.Identity;
using ReWare.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

public class ItemsController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: Items
    public ActionResult Index(string search, string category, string size, string condition)
    {
        var items = db.Items
                      .Where(i => i.ModerationStatus == "Approved" && i.AvailabilityStatus == "Available");

        if (!string.IsNullOrEmpty(search))
        {
            items = items.Where(i => i.Title.Contains(search) || i.Description.Contains(search) || i.Tags.Contains(search));
        }
        if (!string.IsNullOrEmpty(category))
        {
            items = items.Where(i => i.Category == category);
        }
        if (!string.IsNullOrEmpty(size))
        {
            items = items.Where(i => i.Size == size);
        }
        if (!string.IsNullOrEmpty(condition))
        {
            items = items.Where(i => i.Condition == condition);
        }

        ViewBag.Categories = db.Items.Select(i => i.Category).Distinct().ToList();
        ViewBag.Sizes = db.Items.Select(i => i.Size).Distinct().ToList();
        ViewBag.Conditions = db.Items.Select(i => i.Condition).Distinct().ToList();

        return View(items.ToList());
    }

    // GET: Items/Details/5
    public ActionResult Details(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        var item = db.Items.Include("UploadedBy").FirstOrDefault(i => i.Id == id);
        if (item == null) return HttpNotFound();

        ViewBag.CanRequestSwap = false;
        if (User.Identity.IsAuthenticated && item.UploadedByUserId != User.Identity.GetUserId())
        {
            var userId = User.Identity.GetUserId();
            bool alreadyRequested = db.Swaps.Any(s => s.ItemId == id && s.RequesterId == userId && s.Status == "Pending");
            if (!alreadyRequested && item.AvailabilityStatus == "Available" && item.ModerationStatus == "Approved")
            {
                ViewBag.CanRequestSwap = true;
            }
        }

        return View(item);
    }

    // GET: Items/Create
    [Authorize(Roles = "User,Admin")]
    public ActionResult Create()
    {
        return View();
    }

    // POST: Items/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User,Admin")]
    public ActionResult Create(Item item, HttpPostedFileBase ImageFile)
    {
        if (ModelState.IsValid)
        {
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                var ext = Path.GetExtension(ImageFile.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    string fileName = Guid.NewGuid() + ext;
                    string path = Path.Combine(Server.MapPath("~/Content/Uploads/Items"), fileName);
                    ImageFile.SaveAs(path);
                    item.ImagePath = "/Content/Uploads/Items/" + fileName;
                }
            }

            item.ModerationStatus = "Pending";
            item.AvailabilityStatus = "Available";
            item.UploadedByUserId = User.Identity.GetUserId();

            db.Items.Add(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(item);
    }

    // GET: Items/Edit/5
    [Authorize(Roles = "User,Admin")]
    public ActionResult Edit(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var item = db.Items.Find(id);
        if (item == null) return HttpNotFound();
        return View(item);
    }

    // POST: Items/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User,Admin")]
    public ActionResult Edit(Item item, HttpPostedFileBase ImageFile)
    {
        if (ModelState.IsValid)
        {
            var existingItem = db.Items.Find(item.Id);
            if (existingItem != null)
            {
                existingItem.Title = item.Title;
                existingItem.Description = item.Description;
                existingItem.Category = item.Category;
                existingItem.Type = item.Type;
                existingItem.Size = item.Size;
                existingItem.Condition = item.Condition;
                existingItem.Tags = item.Tags;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var ext = Path.GetExtension(ImageFile.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                    {
                        string fileName = Guid.NewGuid() + ext;
                        string path = Path.Combine(Server.MapPath("~/Content/Uploads/Items"), fileName);
                        ImageFile.SaveAs(path);
                        existingItem.ImagePath = "/Content/Uploads/Items/" + fileName;
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        return View(item);
    }

    // DELETE: Items/Delete/5
    [Authorize(Roles = "Admin")]
    public ActionResult Delete(int? id)
    {
        if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var item = db.Items.Find(id);
        if (item == null) return HttpNotFound();
        db.Items.Remove(item);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize]
    public ActionResult RequestSwap(int id)
    {
        var item = db.Items.Find(id);
        if (item == null) return HttpNotFound();

        var userId = User.Identity.GetUserId();

        if (db.Swaps.Any(s => s.ItemId == id && s.RequesterId == userId && s.Status == "Pending"))
        {
            TempData["Message"] = "You already requested this item.";
            return RedirectToAction("Details", new { id = id });
        }

        if (item.AvailabilityStatus != "Available" || item.ModerationStatus != "Approved")
        {
            TempData["Message"] = "This item is not available for swap.";
            return RedirectToAction("Details", new { id = id });
        }

        var swap = new Swap
        {
            ItemId = id,
            RequesterId = userId,
            Status = "Pending"
        };

        db.Swaps.Add(swap);
        db.SaveChanges();

        TempData["Message"] = "Swap request sent!";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public ActionResult Redeem(int itemId)
    {
        var userId = User.Identity.GetUserId();
        var user = db.Users.Find(userId);
        var item = db.Items.Find(itemId);

        if (item == null || !item.IsRedeemable || item.PointsCost == null)
            return HttpNotFound("Item cannot be redeemed.");

        if (user.Points < item.PointsCost.Value)
            return Content("Not enough points to redeem this item.");

        user.Points -= item.PointsCost.Value;

        db.PointsTransactions.Add(new PointsTransaction
        {
            UserId = userId,
            PointsDeducted = item.PointsCost.Value,
            PointsAdded = 0,
            Date = DateTime.Now,
            Description = $"Redeemed item: {item.Title}"
        });

        db.SaveChanges();

        return RedirectToAction("Dashboard", "User");
    }
}
