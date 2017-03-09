namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Validationattributesadded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ActiveCampaignAccounts", "ApiUrl", c => c.String(maxLength: 100));
            AlterColumn("dbo.ActiveCampaignAccounts", "ApiKey", c => c.String(maxLength: 100));
            AlterColumn("dbo.AweberAccounts", "AccessToken", c => c.String(maxLength: 100));
            AlterColumn("dbo.AweberAccounts", "TokenSecret", c => c.String(maxLength: 100));
            AlterColumn("dbo.InfusionSoftAccounts", "AccessToken", c => c.String(maxLength: 100));
            AlterColumn("dbo.InfusionSoftAccounts", "RefreshToken", c => c.String(maxLength: 100));
            AlterColumn("dbo.Payments", "PaymentId", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Payments", "PaymentId", c => c.String());
            AlterColumn("dbo.InfusionSoftAccounts", "RefreshToken", c => c.String());
            AlterColumn("dbo.InfusionSoftAccounts", "AccessToken", c => c.String());
            AlterColumn("dbo.AweberAccounts", "TokenSecret", c => c.String());
            AlterColumn("dbo.AweberAccounts", "AccessToken", c => c.String());
            AlterColumn("dbo.ActiveCampaignAccounts", "ApiKey", c => c.String());
            AlterColumn("dbo.ActiveCampaignAccounts", "ApiUrl", c => c.String());
        }
    }
}
