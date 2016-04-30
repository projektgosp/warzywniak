namespace projekt_gosp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _12312213123 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Klienci", "selectedShopId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Klienci", "selectedShopId");
        }
    }
}
