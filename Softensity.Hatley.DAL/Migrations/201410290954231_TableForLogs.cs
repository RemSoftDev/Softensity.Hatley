namespace Softensity.Hatley.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class TableForLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorLogger",
                c => new
                     {
                         Id = c.Int(nullable: false, identity: true),
                         Date = c.DateTime(nullable: false),
                         Level = c.String(nullable: false, maxLength: 50),
                         Logger = c.String(nullable: false, maxLength: 255),
                         Message = c.String(nullable: false),
                         Exception = c.String(),
                     })
                    .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.InfoLogger",
                c => new
                     {
                         Id = c.Int(nullable: false, identity: true),
                         Date = c.DateTime(nullable: false),
                         Level = c.String(nullable: false, maxLength: 50),
                         Logger = c.String(nullable: false, maxLength: 255),
                         Message = c.String(nullable: false),
                     })
                    .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
        }
    }
}
