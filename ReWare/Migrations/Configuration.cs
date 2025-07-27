using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReWare.Models;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;
using System;

internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
{
    public Configuration()
    {
        // Better to keep FALSE in most team scenarios and use explicit migrations.
        // Set to true only if you know what you're doing.
        AutomaticMigrationsEnabled = true;
    }

    protected override void Seed(ApplicationDbContext context)
    {
        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        // --- Roles ---
        if (!roleManager.RoleExists("Admin"))
            roleManager.Create(new IdentityRole("Admin"));
        if (!roleManager.RoleExists("User"))
            roleManager.Create(new IdentityRole("User"));

        // --- Admin User ---
        if (!context.Users.Any(u => u.UserName == "admin@rewear.com"))
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@rewear.com",
                Email = "admin@rewear.com",
                Points = 1000
            };
            var result = userManager.Create(adminUser, "Admin@123");
            if (result.Succeeded)
                userManager.AddToRole(adminUser.Id, "Admin");
        }

        // --- Demo Users ---
        for (int i = 1; i <= 5; i++)
        {
            string email = $"user{i}@rewear.com";
            if (!context.Users.Any(u => u.UserName == email))
            {
                var demoUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Points = 100
                };
                var result = userManager.Create(demoUser, "User@123");
                if (result.Succeeded)
                    userManager.AddToRole(demoUser.Id, "User");
            }
        }
        context.SaveChanges();

        // Get 5 users to assign items to
        var userRoleId = context.Roles.FirstOrDefault(r => r.Name == "User")?.Id;
        var demoUsers = context
            .Users
            .Where(u => u.Roles.Any(r => r.RoleId == userRoleId))
            .Take(5)
            .ToList();

        // --- Demo Items (only seed if none exist) ---
        if (!context.Items.Any() && demoUsers.Count >= 5)
        {
            // Helper for readability
            string img(string file) => $"/Content/Images/{file}";

            var items = new List<Item>
            {
                new Item {
                    Title="Blue Shirt", Description="Casual cotton shirt",
                    Category="Shirts", Size="M", Condition="Good", Tags="Blue,Cotton",
                    ImagePath=img("blue-shirt.jpg"),
                    ModerationStatus="Approved", AvailabilityStatus="Available",
                    IsRedeemable=true, PointsCost=50,
                    UploadedByUserId=demoUsers[0].Id
                },
                new Item {
                    Title="Black Jacket", Description="Winter jacket",
                    Category="Jackets", Size="L", Condition="Like New", Tags="Black,Warm",
                    ImagePath=img("jacket.jpg"),
                    ModerationStatus="Pending", AvailabilityStatus="Available",
                    IsRedeemable=true, PointsCost=80,
                    UploadedByUserId=demoUsers[1].Id
                },
                new Item {
                    Title="Jeans", Description="Slim fit jeans",
                    Category="Pants", Size="32", Condition="Good", Tags="Denim",
                    ImagePath=img("jeans.jpg"),
                    ModerationStatus="Approved", AvailabilityStatus="Available",
                    IsRedeemable=false, PointsCost=null,
                    UploadedByUserId=demoUsers[2].Id
                },
                new Item {
                    Title="Red Dress", Description="Party wear",
                    Category="Dresses", Size="S", Condition="Like New", Tags="Red,Fashion",
                    ImagePath=img("red-dress.jpg"),
                    ModerationStatus="Approved", AvailabilityStatus="Available",
                    IsRedeemable=true, PointsCost=120,
                    UploadedByUserId=demoUsers[3].Id
                },
                new Item {
                    Title="Green T-Shirt", Description="Comfort fit",
                    Category="T-Shirts", Size="L", Condition="Good", Tags="Green,Cotton",
                    ImagePath=img("green-tshirt.jpg"),
                    ModerationStatus="Pending", AvailabilityStatus="Available",
                    IsRedeemable=false,
                    UploadedByUserId=demoUsers[4].Id
                },
                new Item {
                    Title="Sneakers", Description="Casual shoes",
                    Category="Footwear", Size="9", Condition="Good", Tags="Shoes,Sneakers",
                    ImagePath=img("sneakers.jpg"),
                    ModerationStatus="Approved", AvailabilityStatus="Available",
                    IsRedeemable=true, PointsCost=90,
                    UploadedByUserId=demoUsers[0].Id
                },
                new Item {
                    Title="Leather Belt", Description="Genuine leather",
                    Category="Accessories", Size="Free", Condition="New", Tags="Leather,Belt",
                    ImagePath=img("belt.jpg"),
                    ModerationStatus="Approved", AvailabilityStatus="Available",
                    IsRedeemable=true, PointsCost=40,
                    UploadedByUserId=demoUsers[1].Id
                },
                new Item {
                    Title="Hoodie", Description="Warm and cozy",
                    Category="Hoodies", Size="XL", Condition="Like New", Tags="Hoodie,Warm",
                    ImagePath=img("hoodie.jpg"),
                    ModerationStatus="Pending", AvailabilityStatus="Available",
                    IsRedeemable=true, PointsCost=70,
                    UploadedByUserId=demoUsers[2].Id
                },
                new Item {
                    Title="Formal Shirt", Description="Office wear",
                    Category="Shirts", Size="L", Condition="Good", Tags="Formal,White",
                    ImagePath=img("formal-shirt.jpg"),
                    ModerationStatus="Approved", AvailabilityStatus="Available",
                    IsRedeemable=false,
                    UploadedByUserId=demoUsers[3].Id
                },
                new Item {
                    Title="Sports Shorts", Description="Comfortable for gym",
                    Category="Shorts", Size="M", Condition="Good", Tags="Sports,Gym",
                    ImagePath=img("shorts.jpg"),
                    ModerationStatus="Pending", AvailabilityStatus="Available",
                    IsRedeemable=true, PointsCost=35,
                    UploadedByUserId=demoUsers[4].Id
                }
            };

            context.Items.AddRange(items);
            context.SaveChanges();

            // OPTIONAL: Seed additional images (uncomment after ItemImage implemented)
            /*
            var allItems = context.Items.ToList();
            foreach (var it in allItems)
            {
                context.ItemImages.Add(new ItemImage { ItemId = it.Id, ImagePath = it.ImagePath }); // primary
                // Add a dummy second image for demonstration
                context.ItemImages.Add(new ItemImage { ItemId = it.Id, ImagePath = "/Content/Images/placeholder-extra.jpg" });
            }
            context.SaveChanges();
            */
        }

        // If you migrated existing Items before adding new columns, ensure defaults:
        foreach (var item in context.Items.Where(i => i.ModerationStatus == null || i.AvailabilityStatus == null))
        {
            if (string.IsNullOrEmpty(item.ModerationStatus))
                item.ModerationStatus = "Approved"; // or "Pending" depending on your logic
            if (string.IsNullOrEmpty(item.AvailabilityStatus))
                item.AvailabilityStatus = "Available";
        }
        context.SaveChanges();
    }
}
