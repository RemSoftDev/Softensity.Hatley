namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Additionalfieldsforuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Time", c => c.Int());
            AddColumn("dbo.AspNetUsers", "DayOfWeek", c => c.Int());
            AddColumn("dbo.AspNetUsers", "DayOfMonth", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DayOfMonth");
            DropColumn("dbo.AspNetUsers", "DayOfWeek");
            DropColumn("dbo.AspNetUsers", "Time");
        }
    }
}
