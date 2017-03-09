namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTableForAweberAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AweberAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccessToken = c.String(),
                        TokenSecret = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AweberAccounts", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.AweberAccounts", new[] { "Id" });
            DropTable("dbo.AweberAccounts");
        }
    }
}
