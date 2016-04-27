namespace projekt_gosp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Towary",
                c => new
                    {
                        ID_Towaru = c.Int(nullable: false, identity: true),
                        ID_produktu = c.Int(nullable: false),
                        ID_sklepu = c.Int(nullable: false),
                        Data_waznosci = c.DateTime(nullable: false),
                        Ilosc = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Towaru)
                .ForeignKey("dbo.Produkty", t => t.ID_produktu, cascadeDelete: true)
                .ForeignKey("dbo.Sklepy", t => t.ID_sklepu, cascadeDelete: true)
                .Index(t => t.ID_produktu)
                .Index(t => t.ID_sklepu);
            
            AddColumn("dbo.Klienci", "Punkty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropIndex("dbo.Towary", new[] { "ID_sklepu" });
            DropIndex("dbo.Towary", new[] { "ID_produktu" });
            DropForeignKey("dbo.Towary", "ID_sklepu", "dbo.Sklepy");
            DropForeignKey("dbo.Towary", "ID_produktu", "dbo.Produkty");
            DropColumn("dbo.Klienci", "Punkty");
            DropTable("dbo.Towary");
        }
    }
}
