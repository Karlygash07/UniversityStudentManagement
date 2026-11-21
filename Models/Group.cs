using System.ComponentModel.DataAnnotations;

namespace UniversityStudentManagement.Models;

public sealed class Group
{
    public int Id { get; set; }

    [Required, StringLength(40)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
