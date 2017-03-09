namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEnabledfieldtoaccountstables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropboxAccounts", "Enabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.ActiveCampaignAccounts", "Enabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AweberAccounts", "Enabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.InfusionSoftAccounts", "Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InfusionSoftAccounts", "Enabled");
            DropColumn("dbo.AweberAccounts", "Enabled");
            DropColumn("dbo.ActiveCampaignAccounts", "Enabled");
            DropColumn("dbo.DropboxAccounts", "Enabled");
        }
    }
}
