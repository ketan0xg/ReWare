using System;
using System.Data.Entity.Migrations;

public partial class AddDescriptionToPointsTransaction : DbMigration
{
    public override void Up()
    {
        AddColumn("dbo.PointsTransactions", "Description", c => c.String());
    }
    
    public override void Down()
    {
        DropColumn("dbo.PointsTransactions", "Description");
    }
}
