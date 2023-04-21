using ConsoleApplication.Models;

namespace ConsoleApplication.Services;

public interface IAdminService
{
    public IEnumerable<Position> GetAllPositions();

    public IEnumerable<Class> GetAllClasses();

    /// <summary>
    /// Get the number of teachers in the school
    /// </summary>
    /// <returns>
    /// The number of teachers in the school
    /// </returns>
    public int GetNumberOfTeachers();

    public IEnumerable<Faculty> GetAllFaculties();

    public Grade? AddGrade(Grade grade);

    public IEnumerable<Course> GetAllActiveCourses();
}
