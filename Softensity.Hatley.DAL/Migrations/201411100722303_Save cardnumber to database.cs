namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Savecardnumbertodatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "CardNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "CardNumber");
        }
    }
}
