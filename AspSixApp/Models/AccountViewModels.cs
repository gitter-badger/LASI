﻿using System.ComponentModel.DataAnnotations;

namespace AspSixApp.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [NameField]
        [StringLength(maximumLength: 200, ErrorMessage = Messages.LENGTH_INVALID_MESSAGE, MinimumLength = 1)]
        [Display(Name = "First Name", ShortName = "First")]
        public string FirstName { get; set; }

        [NameField]
        [StringLength(maximumLength: 200, ErrorMessage = Messages.LENGTH_INVALID_MESSAGE, MinimumLength = 1)]
        [Display(Name = "Last Name", ShortName = "Last")]
        public string LastName { get; set; }


    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address", ShortName = "Email")]
        public string Email { get; set; }


        [Required]
        [NameField]
        [StringLength(maximumLength: 200, ErrorMessage = Messages.LENGTH_INVALID_MESSAGE, MinimumLength = 1)]
        [Display(Name = "First Name", ShortName = "First")]
        public string FirstName { get; set; }

        [Required]
        [NameField]
        [StringLength(maximumLength: 200, ErrorMessage = Messages.LENGTH_INVALID_MESSAGE, MinimumLength = 1)]
        [Display(Name = "Last Name", ShortName = "Last")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = Messages.LENGTH_INVALID_MESSAGE, MinimumLength = 4)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = Messages.LENGTH_INVALID_MESSAGE, MinimumLength = 4)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    #region Validation Messages
    internal static class Messages
    {
        public const string LENGTH_INVALID_MESSAGE = "Must be between 1 and 200 characters";
    }
    #endregion

}