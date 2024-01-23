using System.ComponentModel.DataAnnotations;

namespace Auth_WebApplication.ViewModels
{
    public class RegisterVM
    {
        [EmailAddress]
        [Required(ErrorMessage ="Please enter email")]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage ="Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }=default!;

        [Display(Name ="Confirm Password")]
        [Required(ErrorMessage ="Please enter confirm password")]
        [Compare("Password",ErrorMessage ="Password and Confirm Password not matched.")]
        public string ConfirmPassword { get; set; }=default!;

    }
}
