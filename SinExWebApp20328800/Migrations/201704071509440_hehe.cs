namespace SinExWebApp20328800.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hehe : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Shipment", name: "ShippingAccountId", newName: "SenderShippingAccountID");
            RenameIndex(table: "dbo.Shipment", name: "IX_ShippingAccountId", newName: "IX_SenderShippingAccountID");
            CreateTable(
                "dbo.Package",
                c => new
                    {
                        PackageID = c.Int(nullable: false, identity: true),
                        WaybillId = c.Int(nullable: false),
                        PackageTypeID = c.Int(nullable: false),
                        Description = c.String(),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrencyCode = c.String(maxLength: 128),
                        DeclaredWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PackageID)
                .ForeignKey("dbo.Currency", t => t.CurrencyCode)
                .ForeignKey("dbo.PackageType", t => t.PackageTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Shipment", t => t.WaybillId, cascadeDelete: true)
                .Index(t => t.WaybillId)
                .Index(t => t.PackageTypeID)
                .Index(t => t.CurrencyCode);
            
            CreateTable(
                "dbo.PickupLocation",
                c => new
                    {
                        PickupLocationID = c.Int(nullable: false, identity: true),
                        ShippingAccountId = c.Int(nullable: false),
                        Nickname = c.String(),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.PickupLocationID)
                .ForeignKey("dbo.ShippingAccount", t => t.ShippingAccountId, cascadeDelete: true)
                .Index(t => t.ShippingAccountId);
            
            CreateTable(
                "dbo.Recipient",
                c => new
                    {
                        RecipientID = c.Int(nullable: false, identity: true),
                        ShippingAccountId = c.Int(nullable: false),
                        FullName = c.String(),
                        CompanyName = c.String(),
                        DepartmentName = c.String(),
                        DeliveryAddress = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.RecipientID)
                .ForeignKey("dbo.ShippingAccount", t => t.ShippingAccountId, cascadeDelete: true)
                .Index(t => t.ShippingAccountId);
            
            CreateTable(
                "dbo.Tracking",
                c => new
                    {
                        TrackingID = c.Int(nullable: false, identity: true),
                        WaybillId = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Description = c.String(),
                        Location = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TrackingID)
                .ForeignKey("dbo.Shipment", t => t.WaybillId, cascadeDelete: true)
                .Index(t => t.WaybillId);
            
            AddColumn("dbo.Shipment", "ShipmentPayer", c => c.Int(nullable: false));
            AddColumn("dbo.Shipment", "TaxPayer", c => c.Int(nullable: false));
            AddColumn("dbo.Shipment", "Duty", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Shipment", "Tax", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Shipment", "ConfirmOrNot", c => c.Boolean(nullable: false));
            AddColumn("dbo.Shipment", "PickupOrNot", c => c.Boolean(nullable: false));
            AddColumn("dbo.Shipment", "PickupDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shipment", "RecipientID", c => c.Int(nullable: false));
            AddColumn("dbo.Shipment", "ServiceTypeID", c => c.Int(nullable: false));
            AddColumn("dbo.Shipment", "PickupLocationID", c => c.Int(nullable: false));
            AddColumn("dbo.Shipment", "RecipientShippingAccountID", c => c.Int(nullable: false));
            AlterColumn("dbo.ShippingAccount", "PostalCode", c => c.String(maxLength: 6));
            CreateIndex("dbo.Shipment", "RecipientID");
            CreateIndex("dbo.Shipment", "ServiceTypeID");
            CreateIndex("dbo.Shipment", "PickupLocationID");
            CreateIndex("dbo.Shipment", "RecipientShippingAccountID");
            AddForeignKey("dbo.Shipment", "PickupLocationID", "dbo.PickupLocation", "PickupLocationID", cascadeDelete: false);
            AddForeignKey("dbo.Shipment", "RecipientID", "dbo.Recipient", "RecipientID", cascadeDelete: false);
            AddForeignKey("dbo.Shipment", "RecipientShippingAccountID", "dbo.ShippingAccount", "ShippingAccountId", cascadeDelete: false);
            AddForeignKey("dbo.Shipment", "ServiceTypeID", "dbo.ServiceType", "ServiceTypeID", cascadeDelete: false);
            DropColumn("dbo.Shipment", "ServiceType");
            DropColumn("dbo.Shipment", "ShippedDate");
            DropColumn("dbo.Shipment", "DeliveredDate");
            DropColumn("dbo.Shipment", "RecipientName");
            DropColumn("dbo.Shipment", "Origin");
            DropColumn("dbo.Shipment", "Destination");
            DropColumn("dbo.Shipment", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shipment", "Status", c => c.String());
            AddColumn("dbo.Shipment", "Destination", c => c.String());
            AddColumn("dbo.Shipment", "Origin", c => c.String());
            AddColumn("dbo.Shipment", "RecipientName", c => c.String());
            AddColumn("dbo.Shipment", "DeliveredDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shipment", "ShippedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Shipment", "ServiceType", c => c.String());
            DropForeignKey("dbo.Tracking", "WaybillId", "dbo.Shipment");
            DropForeignKey("dbo.Shipment", "ServiceTypeID", "dbo.ServiceType");
            DropForeignKey("dbo.Shipment", "RecipientShippingAccountID", "dbo.ShippingAccount");
            DropForeignKey("dbo.Shipment", "RecipientID", "dbo.Recipient");
            DropForeignKey("dbo.Recipient", "ShippingAccountId", "dbo.ShippingAccount");
            DropForeignKey("dbo.Shipment", "PickupLocationID", "dbo.PickupLocation");
            DropForeignKey("dbo.PickupLocation", "ShippingAccountId", "dbo.ShippingAccount");
            DropForeignKey("dbo.Package", "WaybillId", "dbo.Shipment");
            DropForeignKey("dbo.Package", "PackageTypeID", "dbo.PackageType");
            DropForeignKey("dbo.Package", "CurrencyCode", "dbo.Currency");
            DropIndex("dbo.Tracking", new[] { "WaybillId" });
            DropIndex("dbo.Recipient", new[] { "ShippingAccountId" });
            DropIndex("dbo.PickupLocation", new[] { "ShippingAccountId" });
            DropIndex("dbo.Package", new[] { "CurrencyCode" });
            DropIndex("dbo.Package", new[] { "PackageTypeID" });
            DropIndex("dbo.Package", new[] { "WaybillId" });
            DropIndex("dbo.Shipment", new[] { "RecipientShippingAccountID" });
            DropIndex("dbo.Shipment", new[] { "PickupLocationID" });
            DropIndex("dbo.Shipment", new[] { "ServiceTypeID" });
            DropIndex("dbo.Shipment", new[] { "RecipientID" });
            AlterColumn("dbo.ShippingAccount", "PostalCode", c => c.String(nullable: false, maxLength: 6));
            DropColumn("dbo.Shipment", "RecipientShippingAccountID");
            DropColumn("dbo.Shipment", "PickupLocationID");
            DropColumn("dbo.Shipment", "ServiceTypeID");
            DropColumn("dbo.Shipment", "RecipientID");
            DropColumn("dbo.Shipment", "PickupDate");
            DropColumn("dbo.Shipment", "PickupOrNot");
            DropColumn("dbo.Shipment", "ConfirmOrNot");
            DropColumn("dbo.Shipment", "Tax");
            DropColumn("dbo.Shipment", "Duty");
            DropColumn("dbo.Shipment", "TaxPayer");
            DropColumn("dbo.Shipment", "ShipmentPayer");
            DropTable("dbo.Tracking");
            DropTable("dbo.Recipient");
            DropTable("dbo.PickupLocation");
            DropTable("dbo.Package");
            RenameIndex(table: "dbo.Shipment", name: "IX_SenderShippingAccountID", newName: "IX_ShippingAccountId");
            RenameColumn(table: "dbo.Shipment", name: "SenderShippingAccountID", newName: "ShippingAccountId");
        }
    }
}
