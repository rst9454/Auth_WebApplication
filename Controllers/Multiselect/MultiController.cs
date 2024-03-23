using Auth_WebApplication.Data;
using Auth_WebApplication.Models.MultiselctModel;
using Auth_WebApplication.ViewModels.MultiselectVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Auth_WebApplication.Controllers.Multiselect
{
    public class MultiController : Controller
    {
        private readonly ApplicationContext context;

        public MultiController(ApplicationContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            var data = (from s in context.Students
                        join
                       sub in context.StudentSubjects on s.StudentId equals sub.StudentId
                       group new { s, sub } by new {s.StudentId,s.Name,s.Class} into g
                        select new StudentSubjectVM
                        {
                            StudnetId = g.Key.StudentId,
                            Name = g.Key.Name,
                            Class = g.Key.Class,
                            Subject = string.Join(", ",g.Select(x=>x.sub.Subject.SubjectName))
                        }).ToList();
            return View(data);
        }
        public IActionResult AddStudent()
        {
            //var subjectList = context.Subjects.ToList();
            //ViewBag.subjects = new SelectList(subjectList, "SubjectId", "SubjectName");
            AddStudentSubjectVM svm = new AddStudentSubjectVM();
            svm.SubjectList = context.Subjects.Select(x => new SelectListItem
            {
                Value=x.SubjectId.ToString(),
                Text=x.SubjectName
            }).ToList();
            return View(svm);
        }
        [HttpPost]
        public IActionResult AddStudent(AddStudentSubjectVM model)
        {
            Student std = model.Student;
            context.Add(std);
            context.SaveChanges();
            foreach(var selectId in model.SubjectIds)
            {
                context.StudentSubjects.Add(new StudentSubject
                {
                    StudentId=std.StudentId,
                    SubjectId=selectId
                });
            }
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
