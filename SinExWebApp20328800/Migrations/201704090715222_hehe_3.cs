namespace SinExWebApp20328800.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hehe_3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipment", "Origin", c => c.String());
            AddColumn("dbo.Shipment", "Destination", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipment", "Destination");
            DropColumn("dbo.Shipment", "Origin");
        }
    }
}
