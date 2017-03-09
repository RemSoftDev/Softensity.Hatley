namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableForActiveCampaignAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActiveCampaignAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ApiUrl = c.String(),
                        ApiKey = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActiveCampaignAccounts", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.ActiveCampaignAccounts", new[] { "Id" });
            DropTable("dbo.ActiveCampaignAccounts");
        }
    }
}
