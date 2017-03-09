namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGoogleDriveaccountinfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GoogleDriveAccounts",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        UserSecret = c.String(maxLength: 30),
                        UserToken = c.String(maxLength: 30),
                        ConnectingDate = c.DateTime(nullable: false),
                        AccountName = c.String(maxLength: 100),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GoogleDriveAccounts", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.GoogleDriveAccounts", new[] { "UserId" });
            DropTable("dbo.GoogleDriveAccounts");
        }
    }
}
