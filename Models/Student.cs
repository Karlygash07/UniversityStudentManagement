using System.ComponentModel.DataAnnotations;

namespace UniversityStudentManagement.Models;

public sealed class Student
{
    public int Id { get; set; }

    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public int GroupId { get; set; }

    public Group Group { get; set; } = null!;
}
