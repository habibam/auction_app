using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Auctions.Models
{
    public class User: BaseEntity
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public double Wallet { get; set; }
    }

        public class LoginUser
    {
        [Required(ErrorMessage="Username is required")]
        // [RegularExpression()]
        [Display(Name= "Username")]
        public string LogUsername { get; set; }

        [Required(ErrorMessage="Password is required")]
        [Display(Name = "Password")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string LogPassword { get; set; }
    }

        public class RegisterUser
    {
        // private Regex reg;

        [Required(ErrorMessage="First Name is required")]
        [Display(Name = "First Name")]
        [MinLength(2, ErrorMessage="Names must be at least 2 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last Name is required")]
        [Display(Name = "Last Name")]
        [MinLength(2, ErrorMessage="Names must be at least 2 characters")]
        
        public string LastName { get; set; }

        [Required(ErrorMessage="Username is required")]
        [MinLength(3, ErrorMessage="Usernames must be between 3 and 20 characters")]
        [MaxLength(20, ErrorMessage="Usernames must be between 3 and 20 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage="Password is required")]
        [MinLength(8, ErrorMessage ="Passwords must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage="Password Confirmation is required")]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage="Passwords do not match")]
        [DataType(DataType.Password)]
        public string C_Password { get; set; }
    }
}