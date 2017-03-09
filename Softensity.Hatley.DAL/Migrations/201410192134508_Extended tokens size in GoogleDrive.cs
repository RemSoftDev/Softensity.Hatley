namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendedtokenssizeinGoogleDrive : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GoogleDriveAccounts", "AccessToken", c => c.String(maxLength: 100));
            AlterColumn("dbo.GoogleDriveAccounts", "RefreshToken", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GoogleDriveAccounts", "RefreshToken", c => c.String(maxLength: 50));
            AlterColumn("dbo.GoogleDriveAccounts", "AccessToken", c => c.String(maxLength: 50));
        }
    }
}
