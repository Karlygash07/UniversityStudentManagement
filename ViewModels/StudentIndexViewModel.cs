using UniversityStudentManagement.Models;

namespace UniversityStudentManagement.ViewModels;

public sealed class StudentIndexViewModel
{
    public IReadOnlyList<Student> Students { get; init; } = Array.Empty<Student>();
    public string? SearchTerm { get; init; }
    public int TotalStudents { get; init; }
    public int TotalGroups { get; init; }
}
