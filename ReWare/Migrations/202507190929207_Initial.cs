using System;
using System.Data.Entity.Migrations;

public partial class Initial : DbMigration
{
    public override void Up()
    {
        CreateTable(
            "dbo.ItemImages",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ItemId = c.Int(nullable: false),
                    ImagePath = c.String(),
                })
            .PrimaryKey(t => t.Id)
            .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
            .Index(t => t.ItemId);
        
        CreateTable(
            "dbo.Items",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(),
                    Description = c.String(),
                    Category = c.String(),
                    Type = c.String(),
                    Size = c.String(),
                    Condition = c.String(),
                    Tags = c.String(),
                    ImagePath = c.String(),
                    ModerationStatus = c.String(),
                    AvailabilityStatus = c.String(),
                    IsRedeemable = c.Boolean(nullable: false),
                    PointsCost = c.Int(),
                    UploadedByUserId = c.String(),
                    UploadedBy_Id = c.String(maxLength: 128),
                })
            .PrimaryKey(t => t.Id)
            .ForeignKey("dbo.AspNetUsers", t => t.UploadedBy_Id)
            .Index(t => t.UploadedBy_Id);
        
        CreateTable(
            "dbo.AspNetUsers",
            c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Points = c.Int(nullable: false),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
            .PrimaryKey(t => t.Id)
            .Index(t => t.UserName, unique: true, name: "UserNameIndex");
        
        CreateTable(
            "dbo.AspNetUserClaims",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
            .PrimaryKey(t => t.Id)
            .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            .Index(t => t.UserId);
        
        CreateTable(
            "dbo.AspNetUserLogins",
            c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
            .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
            .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            .Index(t => t.UserId);
        
        CreateTable(
            "dbo.PointsTransactions",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(maxLength: 128),
                    PointsAdded = c.Int(nullable: false),
                    PointsDeducted = c.Int(nullable: false),
                    Date = c.DateTime(nullable: false),
                })
            .PrimaryKey(t => t.Id)
            .ForeignKey("dbo.AspNetUsers", t => t.UserId)
            .Index(t => t.UserId);
        
        CreateTable(
            "dbo.AspNetUserRoles",
            c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
            .PrimaryKey(t => new { t.UserId, t.RoleId })
            .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
            .Index(t => t.UserId)
            .Index(t => t.RoleId);
        
        CreateTable(
            "dbo.AspNetRoles",
            c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
            .PrimaryKey(t => t.Id)
            .Index(t => t.Name, unique: true, name: "RoleNameIndex");
        
        CreateTable(
            "dbo.Swaps",
            c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    RequesterId = c.String(maxLength: 128),
                    ItemId = c.Int(nullable: false),
                    Status = c.String(),
                })
            .PrimaryKey(t => t.Id)
            .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
            .ForeignKey("dbo.AspNetUsers", t => t.RequesterId)
            .Index(t => t.RequesterId)
            .Index(t => t.ItemId);
        
    }
    
    public override void Down()
    {
        DropForeignKey("dbo.Swaps", "RequesterId", "dbo.AspNetUsers");
        DropForeignKey("dbo.Swaps", "ItemId", "dbo.Items");
        DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
        DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.PointsTransactions", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.Items", "UploadedBy_Id", "dbo.AspNetUsers");
        DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
        DropForeignKey("dbo.ItemImages", "ItemId", "dbo.Items");
        DropIndex("dbo.Swaps", new[] { "ItemId" });
        DropIndex("dbo.Swaps", new[] { "RequesterId" });
        DropIndex("dbo.AspNetRoles", "RoleNameIndex");
        DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
        DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
        DropIndex("dbo.PointsTransactions", new[] { "UserId" });
        DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
        DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
        DropIndex("dbo.AspNetUsers", "UserNameIndex");
        DropIndex("dbo.Items", new[] { "UploadedBy_Id" });
        DropIndex("dbo.ItemImages", new[] { "ItemId" });
        DropTable("dbo.Swaps");
        DropTable("dbo.AspNetRoles");
        DropTable("dbo.AspNetUserRoles");
        DropTable("dbo.PointsTransactions");
        DropTable("dbo.AspNetUserLogins");
        DropTable("dbo.AspNetUserClaims");
        DropTable("dbo.AspNetUsers");
        DropTable("dbo.Items");
        DropTable("dbo.ItemImages");
    }
}
