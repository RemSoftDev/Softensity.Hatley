namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccountNamefieldtoaccountstables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropboxAccounts", "AccountName", c => c.String(maxLength: 100));
            AddColumn("dbo.ActiveCampaignAccounts", "AccountName", c => c.String(maxLength: 100));
            AddColumn("dbo.AweberAccounts", "AccountName", c => c.String(maxLength: 100));
            AddColumn("dbo.InfusionSoftAccounts", "AccountName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InfusionSoftAccounts", "AccountName");
            DropColumn("dbo.AweberAccounts", "AccountName");
            DropColumn("dbo.ActiveCampaignAccounts", "AccountName");
            DropColumn("dbo.DropboxAccounts", "AccountName");
        }
    }
}
