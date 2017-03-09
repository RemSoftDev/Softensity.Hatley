namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedbackupfieldsandpaymentstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastBackupDate", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "NextBackupDate", c => c.DateTime());
            AddColumn("dbo.Payments", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "Status");
            DropColumn("dbo.AspNetUsers", "NextBackupDate");
            DropColumn("dbo.AspNetUsers", "LastBackupDate");
        }
    }
}
