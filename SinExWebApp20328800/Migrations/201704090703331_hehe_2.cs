namespace SinExWebApp20328800.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hehe_2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Shipment", "PickupLocationID", "dbo.PickupLocation");
            DropForeignKey("dbo.Shipment", "RecipientID", "dbo.Recipient");
            DropIndex("dbo.Shipment", new[] { "RecipientID" });
            DropIndex("dbo.Shipment", new[] { "PickupLocationID" });
            AddColumn("dbo.Shipment", "RecipientFullName", c => c.String());
            AddColumn("dbo.Shipment", "RecipientCompanyName", c => c.String());
            AddColumn("dbo.Shipment", "RecipientDepartmentName", c => c.String());
            AddColumn("dbo.Shipment", "RecipientDeliveryAddress", c => c.String());
            AddColumn("dbo.Shipment", "RecipientPhoneNumber", c => c.String());
            AddColumn("dbo.Shipment", "RecipientEmail", c => c.String());
            AddColumn("dbo.Shipment", "PickupLocationNickname", c => c.String());
            AddColumn("dbo.Shipment", "PickupLocation", c => c.String());
            DropColumn("dbo.Shipment", "RecipientID");
            DropColumn("dbo.Shipment", "PickupLocationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shipment", "PickupLocationID", c => c.Int(nullable: false));
            AddColumn("dbo.Shipment", "RecipientID", c => c.Int(nullable: false));
            DropColumn("dbo.Shipment", "PickupLocation");
            DropColumn("dbo.Shipment", "PickupLocationNickname");
            DropColumn("dbo.Shipment", "RecipientEmail");
            DropColumn("dbo.Shipment", "RecipientPhoneNumber");
            DropColumn("dbo.Shipment", "RecipientDeliveryAddress");
            DropColumn("dbo.Shipment", "RecipientDepartmentName");
            DropColumn("dbo.Shipment", "RecipientCompanyName");
            DropColumn("dbo.Shipment", "RecipientFullName");
            CreateIndex("dbo.Shipment", "PickupLocationID");
            CreateIndex("dbo.Shipment", "RecipientID");
            AddForeignKey("dbo.Shipment", "RecipientID", "dbo.Recipient", "RecipientID", cascadeDelete: true);
            AddForeignKey("dbo.Shipment", "PickupLocationID", "dbo.PickupLocation", "PickupLocationID", cascadeDelete: true);
        }
    }
}
