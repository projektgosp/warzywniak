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
            [Required]
            [Display(Name = "Username")]
            public string username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string password { get; set; }
        }

        public class registerModel
        {
            [Required(ErrorMessage = "Pole nazwa uzytkownika jest wymagane")]
            [Display(Name = "User name")]
            public string username { get; set; }

            [Required(ErrorMessage = "Pole haslo jest wymagane")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string password { get; set; }

            [Required(ErrorMessage = "Pole haslo jest wymagane")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
            public string confirmPassword { get; set; }

            [Required(ErrorMessage = "Pole adres email jest wymagane")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Your email address")]
            //[EmailAddress]
            public string email { get; set; }

            [Required(ErrorMessage = "Pole nr telefonu jest wymagane")]
            [Display(Name = "Nr telefonu")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Zły nr telefonu")]
            public string phonenumber { get; set; }

            [Required(ErrorMessage="Pole imie jest wymagane")]
            public string name { get; set; }

            [Required(ErrorMessage = "Pole nazwisko jest wymagane")]
            public string surname { get; set; }
        }

        public class registerShopModel
        {
            [Required]
            [Display(Name = "User name")]
            public string username { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
            public string confirmPassword { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Your email address")]
            //[EmailAddress]
            public string email { get; set; }

            //dodac validatory
            public string name { get; set; }
            public string surname { get; set; }
            public string privatePhoneNumber { get; set; }
            public string shopPhoneNumber { get; set; }
            public string city { get; set; }
            public string postalCode { get; set; }
            public string street { get; set; }
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

        public class smsOrderIsReady
        {
            public string phoneNumber { get; set; }
            public string shopAddress { get; set; }
            public string orderValue { get; set; }
        }
    }
}