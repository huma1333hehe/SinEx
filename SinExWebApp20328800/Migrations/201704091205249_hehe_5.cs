namespace SinExWebApp20328800.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hehe_5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Package", "PackageTypeSize_PackageTypeSizeID", "dbo.PackageTypeSize");
            DropIndex("dbo.Package", new[] { "PackageTypeSize_PackageTypeSizeID" });
            RenameColumn(table: "dbo.Package", name: "PackageTypeSize_PackageTypeSizeID", newName: "PackageTypeSizeID");
            AlterColumn("dbo.Package", "PackageTypeSizeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Package", "PackageTypeSizeID");
            AddForeignKey("dbo.Package", "PackageTypeSizeID", "dbo.PackageTypeSize", "PackageTypeSizeID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Package", "PackageTypeSizeID", "dbo.PackageTypeSize");
            DropIndex("dbo.Package", new[] { "PackageTypeSizeID" });
            AlterColumn("dbo.Package", "PackageTypeSizeID", c => c.Int());
            RenameColumn(table: "dbo.Package", name: "PackageTypeSizeID", newName: "PackageTypeSize_PackageTypeSizeID");
            CreateIndex("dbo.Package", "PackageTypeSize_PackageTypeSizeID");
            AddForeignKey("dbo.Package", "PackageTypeSize_PackageTypeSizeID", "dbo.PackageTypeSize", "PackageTypeSizeID");
        }
    }
}
