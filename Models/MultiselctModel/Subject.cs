using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth_WebApplication.Models.MultiselctModel
{
    public class Subject
    {
        public int SubjectId { get; set; }
        [MaxLength(100)]
        [Column(TypeName ="varchar(100)")]
        public string SubjectName { get; set; } = default!;
    }
}
