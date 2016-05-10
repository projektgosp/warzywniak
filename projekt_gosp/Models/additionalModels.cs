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
    }
}