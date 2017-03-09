namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropboxAccountModelAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DropboxAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserSecret = c.String(maxLength: 30),
                        UserToken = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DropboxAccounts", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.DropboxAccounts", new[] { "Id" });
            DropTable("dbo.DropboxAccounts");
        }
    }
}
