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

        [Display(Name ="First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        [Display(Name = "Active")]
        public bool Status { get; set; }
        public string Username { get; set; } = default!;
    }
}
