namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Accountsfornewservices : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GetResponseAccounts",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ApiKey = c.String(maxLength: 50),
                        ConnectingDate = c.DateTime(nullable: false),
                        AccountName = c.String(maxLength: 100),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.MailChimpAccounts",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ApiKey = c.String(maxLength: 50),
                        ConnectingDate = c.DateTime(nullable: false),
                        AccountName = c.String(maxLength: 100),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.OntraportAccounts",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        AppID = c.String(maxLength: 50),
                        ApiKey = c.String(maxLength: 50),
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
            DropForeignKey("dbo.OntraportAccounts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MailChimpAccounts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GetResponseAccounts", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.OntraportAccounts", new[] { "UserId" });
            DropIndex("dbo.MailChimpAccounts", new[] { "UserId" });
            DropIndex("dbo.GetResponseAccounts", new[] { "UserId" });
            DropTable("dbo.OntraportAccounts");
            DropTable("dbo.MailChimpAccounts");
            DropTable("dbo.GetResponseAccounts");
        }
    }
}
