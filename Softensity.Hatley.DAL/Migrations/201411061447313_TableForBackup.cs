namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableForBackup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Backups",
                c => new
                    {
                        BackupId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        TimeOfBackup = c.DateTime(nullable: false),
                        BackupedFrom = c.String(),
                        BackupedTo = c.String(),
                    })
                .PrimaryKey(t => t.BackupId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Backups", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Backups", new[] { "UserId" });
            DropTable("dbo.Backups");
        }
    }
}
