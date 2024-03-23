using Auth_WebApplication.Models.MultiselctModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Auth_WebApplication.ViewModels.MultiselectVM
{
    public class AddStudentSubjectVM
    {
        public Student Student { get; set; } = default!;
        [Display(Name ="Subject List")]
        public List<SelectListItem> SubjectList { get; set; } = default!;
        public List<int> SubjectIds { get; set; } = default!;
    }
}
