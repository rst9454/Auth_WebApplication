using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth_WebApplication.Models.MultiselctModel
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        [Column(TypeName ="varchar(50)")]
        [MaxLength(50)]
        public string Name { get; set; } = default!;
        [Column(TypeName = "varchar(5)")]
        //[MaxLength(5)]
        public string Class { get; set; } = default!;
    }
}
