namespace SinExWebApp20328800.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hehe_4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipment", "PickupType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipment", "PickupType");
        }
    }
}
