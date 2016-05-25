using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt_gosp.Models
{
    public class additionalModels
    {
        public class loginModel
        {
            [Required(ErrorMessage = "Pole Nazwa uzytkownika jest wymagane")]
            [Display(Name = "Nazwa uzytkownika")]
            public string username { get; set; }

            [Required(ErrorMessage = "Pole Haslo jest wymagane")]
            [DataType(DataType.Password)]
            [Display(Name = "Haslo")]
            public string password { get; set; }
        }

        public class registerModel
        {
            [Required(ErrorMessage = "Pole Nazwa Uzytkownika jest wymagane")]
            [Display(Name = "Nazwa Uzytkownika")]
            public string username { get; set; }

            [Required(ErrorMessage = "Pole haslo jest wymagane")]
            [StringLength(100, ErrorMessage = "Minimalna długość hasła to 6 znaków", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string password { get; set; }

            [Required(ErrorMessage = "Pole haslo jest wymagane")]
            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("password", ErrorMessage = "Podane hasła są różne")]
            public string confirmPassword { get; set; }

            [Required(ErrorMessage = "Pole adres email jest wymagane")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Twoj adres email")]
            //[EmailAddress]
            public string email { get; set; }

            [Required(ErrorMessage = "Pole nr telefonu jest wymagane")]
            [Display(Name = "Nr telefonu")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Zły nr telefonu")]
            public string phonenumber { get; set; }

            [Required(ErrorMessage="Pole imię jest wymagane")]
            public string name { get; set; }

            [Required(ErrorMessage = "Pole nazwisko jest wymagane")]
            public string surname { get; set; }
        }

        public class registerShopModel
        {
            [Required(ErrorMessage = "Pole Nazwa Uzytkownika jest wymagane")]
            [Display(Name = "Nazwa Uzytkownika")]
            public string username { get; set; }

            [Required(ErrorMessage = "Pole Haslo jest wymagane")]
            [StringLength(100, ErrorMessage = "Minimalna długość hasła to 6 znaków", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Haslo")]
            public string password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("password", ErrorMessage = "Podane hasła są różne")]
            public string confirmPassword { get; set; }

            [Required(ErrorMessage = "Pole adres email jest wymagane")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Twoj adres email")]
            //[EmailAddress]
            public string email { get; set; }

            //dodac validatory
            [Required(ErrorMessage = "Pole Imie jest wymagane")]
            public string name { get; set; }
            [Required(ErrorMessage = "Pole Nazwisko jest wymagane")]
            public string surname { get; set; }
            [Required(ErrorMessage = "Pole Telefon prywatny jest wymagane")]
            public string privatePhoneNumber { get; set; }
            [Required(ErrorMessage = "Pole Telefon firmowy jest wymagane")]
            public string shopPhoneNumber { get; set; }
            [Required(ErrorMessage = "Pole Miasto jest wymagane")]
            public string city { get; set; }
            [Required(ErrorMessage = "Pole Kod Pocztowy jest wymagane")]
            public string postalCode { get; set; }
            [Required(ErrorMessage = "Pole Ulica jest wymagane")]
            public string street { get; set; }
            [Required(ErrorMessage = "Pole Numer budynku jest wymagane")]
            public string streetNumber { get; set; }
            public Nullable<int> flatNumber { get; set; }
        }

        public class shopDisplayModel
        {
            public int shopID { get; set; }
            public string email { get; set; }
            public string phoneNumber { get; set; }
            public string city { get; set; }
            public string postalCode { get; set; }
            public string street { get; set; }
            public string streetNumber { get; set; }
            public Nullable<int> flatNumber { get; set; }
            public Boolean isSelected { get; set; }
        }

        public class OrderModel
        {
            public int merchendiseId { get; set; }
            public double price { get; set; }
            public string itemName { get; set; }
            public decimal quantity { get; set; }

            //public Towar product { get; set; }
            //public int quantity { get; set; }
        }

        public class emailContent
        {
            public string emailAddress { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
        }

        public class sms
        {
            public string phoneNumber { get; set; }
            public string message { get; set; }
        }
    }
}