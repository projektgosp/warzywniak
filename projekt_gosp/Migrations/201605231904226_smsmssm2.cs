namespace projekt_gosp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class smsmssm2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Towary", "isDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Produkty", "isDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Produkty", "isDeleted");
            DropColumn("dbo.Towary", "isDeleted");
        }
    }
}
