namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableForInfusionSoftAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InfusionSoftAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ExpirationTime = c.Int(nullable: false),
                        AccessToken = c.String(),
                        RefreshToken = c.String(),
                        BegginingTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InfusionSoftAccounts", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.InfusionSoftAccounts", new[] { "Id" });
            DropTable("dbo.InfusionSoftAccounts");
        }
    }
}
