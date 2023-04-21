using ConsoleApplication.Models;

namespace ConsoleApplication.Repositories;

public interface IAdminRepository
{
    public IEnumerable<Position> GetAllPositions();

    public IEnumerable<Class> GetAllClasses();

    public IEnumerable<Faculty> GetAllFaculties();

    public IEnumerable<Course> GetAllCourses();

    public IEnumerable<Grade> GetAllGrades();

    public Grade? AddGrade(Grade grade);
}
