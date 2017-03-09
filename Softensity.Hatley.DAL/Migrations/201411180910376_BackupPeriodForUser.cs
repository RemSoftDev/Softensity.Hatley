namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BackupPeriodForUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "BackupPeriod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "BackupPeriod");
        }
    }
}
