using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth_WebApplication.Models.MultiselctModel
{
    public class StudentSubject
    {
        [Key]
        public int StudentSubjectId { get; set; }
        public Student Student { get; set; } = default!;
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Subject Subject { get; set; } = default!;
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }

    }
}
