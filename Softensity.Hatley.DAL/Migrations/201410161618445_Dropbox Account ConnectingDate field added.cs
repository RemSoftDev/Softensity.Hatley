namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropboxAccountConnectingDatefieldadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropboxAccounts", "ConnectingDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DropboxAccounts", "ConnectingDate");
        }
    }
}
