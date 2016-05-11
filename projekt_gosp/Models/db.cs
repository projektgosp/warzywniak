using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projekt_gosp.Models
{
    public class db : DbContext
    {
        public db()
            : base("ShopDB")
        {

        }
        public DbSet<Adres> Adresy { get; set; }
        public DbSet<RodzajCeny> RodzajeCeny { get; set; }
        public DbSet<Kategoria> Kategorie { get; set; }
        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        public DbSet<Metoda_platnosci> Metody_platnosci { get; set; }
        public DbSet<Pozycja_zamowienia> Pozycje_zamowienia { get; set; }
        public DbSet<Produkt> Produkty { get; set; }
        public DbSet<Promocja> Promocje { get; set; }
        public DbSet<Sklep> Sklepy { get; set; }
        public DbSet<Zamowienie> Zamowienia { get; set; }
        public DbSet<Towar> Towary { get; set; }
        public DbSet<CartModel> Koszyk { get; set; }
    }


    [Table("Kategorie")]
    public class Kategoria
    {
        [Key] 
        public int ID_kategorii { get; set; }
        public Nullable<int> ID_kat_nadrzednej { get; set; }
        public string NameToLink { get; set; }
        public string NameToDisplay { get; set; }

        public virtual ICollection<Kategoria> Kategoria1 { get; set; }

        [ForeignKey("ID_kat_nadrzednej")]
        public virtual Kategoria Kategoria2 { get; set; }

        public virtual ICollection<Produkt> Produkty { get; set; }
        public virtual ICollection<Promocja> Promocje { get; set; }
    }

    public class Adres
    {
        [Key]
        public int ID_adresu { get; set; }
        public string Miasto { get; set; }
        public string Kod_pocztowy { get; set; }
        public string Ulica { get; set; }
        public string Nr_budynku { get; set; } // decimal -> string, bo np moze byc budynek 4A
        public Nullable<int> Nr_lokalu { get; set; } // decimal -> int bo nr lokalu jest liczba stala

        public virtual ICollection<Uzytkownik> Uzytkownicy { get; set; }
        public virtual ICollection<Sklep> Sklepy { get; set; }
    }

    [Table("Klienci")]
    public class Uzytkownik
    {
        [Key] 
        public int ID_klienta { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Email { get; set; }
        public string Nr_tel { get; set; }

        //new
        public string accountName { get; set; }

        //new v.3
        public int selectedShopId { get; set; }

        //new v.2
        public int Punkty { get; set; }

        public Nullable<int> ID_adresu { get; set; }

        [ForeignKey("ID_adresu")]
        public virtual Adres Adres { get; set; }

        public virtual ICollection<Zamowienie> Zamowienia { get; set; }
    }

    [Table("Metody_platnosci")]
    public class Metoda_platnosci
    {
        [Key] 
        public int ID_platnosci { get; set; }
        public string Kod { get; set; }
        public string Opis { get; set; }

        public virtual ICollection<Zamowienie> Zamowienia { get; set; }
    }

    [Table("Pozycje_zamowienia")]
    public class Pozycja_zamowienia
    {
        [Key] 
        public int ID_pozycji { get; set; }
        public int ID_zamowienia { get; set; }
        public int ID_produktu { get; set; }
        public decimal Ilosc { get; set; }

        [ForeignKey("ID_produktu")]
        public virtual Produkt Produkt { get; set; }

        [ForeignKey("ID_zamowienia")]
        public virtual Zamowienie Zamowienie { get; set; }
    }

    [Table("Produkty")]
    public class Produkt
    {
        [Key] 
        public int ID_produktu { get; set; }
        public int ID_kategorii { get; set; }
        [Required]
        [Display(Name = "Nazwa produktu")]
        public string Nazwa { get; set; }
        [Required]
        [Display(Name = "Cena produktu")]
        public double Cena { get; set; }

        //new
        public DateTime Data_dodania { get; set; }
        //new
        [Required]
        public string Opis { get; set; }
        //new
        public bool attachedImage { get; set; }
        //new 
        public string fullSizePath { get; set; }
        //new
        public string thumbPath { get; set; }
        //new
        public int ID_RodzajuCeny { get; set; }
        //new
        [ForeignKey("ID_RodzajuCeny")]
        public virtual RodzajCeny RodzajCeny { get; set; }

        [ForeignKey("ID_kategorii")]
        public virtual Kategoria Kategoria { get; set; }

        public virtual ICollection<Pozycja_zamowienia> Pozycje_zamowienia { get; set; }
        public virtual ICollection<Promocja> Promocje { get; set; }
        public virtual ICollection<Towar> Towary { get; set; }
    }

    //new v.2
    [Table("Towary")]
    public class Towar
    {
        [Key]
        public int ID_Towaru { get; set; }
        [Required]
        public int ID_produktu { get; set; }
        [Required]
        public int ID_sklepu { get; set; }
        [Required]
        public DateTime Data_waznosci { get; set; }
        [Required]
        public int Ilosc { get; set; }
        [Required]
        public double Cena { get; set; }

        [ForeignKey("ID_produktu")]
        public virtual Produkt Produkt { get; set; }

        [ForeignKey("ID_sklepu")]
        public virtual Sklep Sklep { get; set; }

    }

    //new
    [Table("RodzajCeny")]
    public class RodzajCeny
    {
        public int id { get; set; }
        public string opis { get; set; } // na wage, na sztuki
    }

    [Table("Promocje")]
    public class Promocja
    {
        [Key] 
        public int ID_promocji { get; set; }
        public int ID_sklepu { get; set; }
        public Nullable<int> ID_produktu { get; set; }
        public Nullable<int> ID_kategorii { get; set; }
        public string Opis { get; set; }
        public decimal Obnizka { get; set; }

        //new
        public string imagePath { get; set; } 

        [ForeignKey("ID_kategorii")]
        public virtual Kategoria Kategoria { get; set; }

        [ForeignKey("ID_produktu")]
        public virtual Produkt Produkt { get; set; }

        [ForeignKey("ID_sklepu")]
        public virtual Sklep Sklep { get; set; }
    }

    [Table("Sklepy")]
    public class Sklep
    {
        [Key] 
        public int ID_sklepu { get; set; }
        public string Nr_tel { get; set; }
        public string Email { get; set; }

        public int ownerID { get; set; }

        public int ID_adresu { get; set; }

        [ForeignKey("ID_adresu")]
        public virtual Adres Adres { get; set; }

        public virtual ICollection<Promocja> Promocje { get; set; }
        public virtual ICollection<Zamowienie> Zamowienia { get; set; }
        public virtual ICollection<Towar> Towary { get; set; }
    }

    [Table("Zamowienia")]
    public class Zamowienie
    {
        [Key] 
        public int ID_zamowienia { get; set; }
        public int ID_klienta { get; set; }
        public int ID_sklepu { get; set; }
        public int ID_platnosci { get; set; }
        public System.DateTime Data_zam { get; set; }
        public System.DateTime Data_real_od { get; set; }
        public System.DateTime Data_real_do { get; set; }
        public string Informacja { get; set; }

        [ForeignKey("ID_klienta")]
        public virtual Uzytkownik Klient { get; set; }

        [ForeignKey("ID_platnosci")]
        public virtual Metoda_platnosci Metoda_platnosci { get; set; }

        public virtual ICollection<Pozycja_zamowienia> Pozycje_zamowienia { get; set; }

        [ForeignKey("ID_sklepu")]
        public virtual Sklep Sklep { get; set; }
    }

    public class CartModel
    {
        [Key]
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string UserName { get; set; }
        public int ID_towaru { get; set; }
        public int Ilosc { get; set; }
    }
}