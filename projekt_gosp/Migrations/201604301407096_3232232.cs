namespace projekt_gosp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3232232 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Towary", "Cena", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Towary", "Cena");
        }
    }
}
