using System.ComponentModel.DataAnnotations;

namespace Auth_WebApplication.ViewModels
{
    public class LoginVM
    {
     
        //[Required(ErrorMessage = "Please enter username")]
        //public string Username { get; set; } = default!;
        [EmailAddress]
        [Required(ErrorMessage = "Please enter ")]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}
