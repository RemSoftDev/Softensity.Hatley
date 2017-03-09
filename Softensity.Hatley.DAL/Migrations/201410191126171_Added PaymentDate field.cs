namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPaymentDatefield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "PaymentDateUTC", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "PaymentDateUTC");
        }
    }
}
