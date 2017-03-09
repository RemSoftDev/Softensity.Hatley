namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedgoogleAPIfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GoogleDriveAccounts", "AccessToken", c => c.String(maxLength: 50));
            AddColumn("dbo.GoogleDriveAccounts", "RefreshToken", c => c.String(maxLength: 50));
            AddColumn("dbo.GoogleDriveAccounts", "ExpiresInSeconds", c => c.Long());
            AddColumn("dbo.GoogleDriveAccounts", "Issued", c => c.DateTime(nullable: false));
            AddColumn("dbo.GoogleDriveAccounts", "Scope", c => c.String(maxLength: 50));
            AddColumn("dbo.GoogleDriveAccounts", "TokenType", c => c.String(maxLength: 50));
            DropColumn("dbo.GoogleDriveAccounts", "UserSecret");
            DropColumn("dbo.GoogleDriveAccounts", "UserToken");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GoogleDriveAccounts", "UserToken", c => c.String(maxLength: 30));
            AddColumn("dbo.GoogleDriveAccounts", "UserSecret", c => c.String(maxLength: 30));
            DropColumn("dbo.GoogleDriveAccounts", "TokenType");
            DropColumn("dbo.GoogleDriveAccounts", "Scope");
            DropColumn("dbo.GoogleDriveAccounts", "Issued");
            DropColumn("dbo.GoogleDriveAccounts", "ExpiresInSeconds");
            DropColumn("dbo.GoogleDriveAccounts", "RefreshToken");
            DropColumn("dbo.GoogleDriveAccounts", "AccessToken");
        }
    }
}
