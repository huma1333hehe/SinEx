namespace SinExWebApp20328800.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PackageType",
                c => new
                    {
                        PackageTypeID = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PackageTypeID);
            
            CreateTable(
                "dbo.ServicePackageFee",
                c => new
                    {
                        ServicePackageFeeID = c.Int(nullable: false, identity: true),
                        Fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinimumFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PackageTypeID = c.Int(nullable: false),
                        ServiceTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ServicePackageFeeID)
                .ForeignKey("dbo.PackageType", t => t.PackageTypeID, cascadeDelete: true)
                .ForeignKey("dbo.ServiceType", t => t.ServiceTypeID, cascadeDelete: true)
                .Index(t => t.PackageTypeID)
                .Index(t => t.ServiceTypeID);
            
            CreateTable(
                "dbo.ServiceType",
                c => new
                    {
                        ServiceTypeID = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        CutoffTime = c.String(),
                        DeliveryTime = c.String(),
                    })
                .PrimaryKey(t => t.ServiceTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServicePackageFee", "ServiceTypeID", "dbo.ServiceType");
            DropForeignKey("dbo.ServicePackageFee", "PackageTypeID", "dbo.PackageType");
            DropIndex("dbo.ServicePackageFee", new[] { "ServiceTypeID" });
            DropIndex("dbo.ServicePackageFee", new[] { "PackageTypeID" });
            DropTable("dbo.ServiceType");
            DropTable("dbo.ServicePackageFee");
            DropTable("dbo.PackageType");
        }
    }
}
