using System.ComponentModel.DataAnnotations;

namespace Auth_WebApplication.ViewModels
{
    public class ForgetPasswordOrUsernameVM
    {
        [Required]
        public string Email { get; set; } = default!;
        public string UserId {  get; set; }=default!;
        public string Token {  get; set; }=default!;

        [Required(ErrorMessage ="Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password and confirm password not matched.")]
        [Display(Name ="Confirm Password")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
