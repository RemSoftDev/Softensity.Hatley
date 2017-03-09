namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedconnectingtimetoaweberandactivecampaign : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActiveCampaignAccounts", "ConnectingDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.AweberAccounts", "ConnectingDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AweberAccounts", "ConnectingDate");
            DropColumn("dbo.ActiveCampaignAccounts", "ConnectingDate");
        }
    }
}
