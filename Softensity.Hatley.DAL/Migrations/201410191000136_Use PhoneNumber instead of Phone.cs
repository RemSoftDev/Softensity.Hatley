namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsePhoneNumberinsteadofPhone : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Phone", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
