using ConsoleApplication.Models;
using ConsoleApplication.Repositories;

namespace ConsoleApplication.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPersonRepository<Employee> _employeeRepository;

    public AdminService(
        IAdminRepository adminRepository,
        IPersonRepository<Employee> employeeRepository
    )
    {
        _adminRepository = adminRepository;
        _employeeRepository = employeeRepository;
    }

    /// <inheritdoc />
    public IEnumerable<Position> GetAllPositions()
    {
        return _adminRepository.GetAllPositions();
    }

    /// <inheritdoc />
    public IEnumerable<Class> GetAllClasses()
    {
        return _adminRepository.GetAllClasses();
    }

    public int GetNumberOfTeachers()
    {
        return _employeeRepository.GetAll().Count(e => e.Position?.Title == "Teacher");
    }

    /// <inheritdoc />
    public IEnumerable<Faculty> GetAllFaculties()
    {
        return _adminRepository.GetAllFaculties();
    }

    // This will be by sql and not tracked by EF
    // so we need to update the students grades list manually
    /// <inheritdoc />
    public Grade? AddGrade(Grade grade)
    {
        var newGrade = _adminRepository.AddGrade(grade);

        Student student = grade.Student;
        student.Grades = student.Grades.Append(newGrade);
        return newGrade;
    }

    /// <inheritdoc />
    public IEnumerable<Course> GetAllActiveCourses()
    {
        return _adminRepository.GetAllCourses().Where(c => c.Active);
    }
}
