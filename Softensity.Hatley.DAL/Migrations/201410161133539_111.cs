namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _111 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ActiveCampaignAccounts", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.AweberAccounts", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DropboxAccounts", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.InfusionSoftAccounts", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.Payments", name: "Id", newName: "UserId");
            RenameIndex(table: "dbo.DropboxAccounts", name: "IX_Id", newName: "IX_UserId");
            RenameIndex(table: "dbo.ActiveCampaignAccounts", name: "IX_Id", newName: "IX_UserId");
            RenameIndex(table: "dbo.AweberAccounts", name: "IX_Id", newName: "IX_UserId");
            RenameIndex(table: "dbo.InfusionSoftAccounts", name: "IX_Id", newName: "IX_UserId");
            RenameIndex(table: "dbo.Payments", name: "IX_Id", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Payments", name: "IX_UserId", newName: "IX_Id");
            RenameIndex(table: "dbo.InfusionSoftAccounts", name: "IX_UserId", newName: "IX_Id");
            RenameIndex(table: "dbo.AweberAccounts", name: "IX_UserId", newName: "IX_Id");
            RenameIndex(table: "dbo.ActiveCampaignAccounts", name: "IX_UserId", newName: "IX_Id");
            RenameIndex(table: "dbo.DropboxAccounts", name: "IX_UserId", newName: "IX_Id");
            RenameColumn(table: "dbo.Payments", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.InfusionSoftAccounts", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DropboxAccounts", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.AweberAccounts", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.ActiveCampaignAccounts", name: "UserId", newName: "Id");
        }
    }
}
