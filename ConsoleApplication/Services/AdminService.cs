using System.Data;
using ConsoleApplication.Data;
using ConsoleApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace ConsoleApplication.Services;

public class AdminService
{
    private readonly HighSchoolDBContext _dbContext;

    public AdminService(HighSchoolDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Class> GetAllClasses()
    {
        return _dbContext.Classes;
    }

    public IEnumerable<Position> GetAllPositions()
    {
        return _dbContext.Positions;
    }

    public IEnumerable<Faculty> GetAllFaculties()
    {
        return _dbContext.Faculties;
    }

    public IEnumerable<Faculty> GetAllFacultiesWithTeachers()
    {
        return _dbContext.Faculties
            .Include(
                faculty =>
                    faculty.Employees.Where(employee => employee!.Position!.Title == "Teacher")
            )
            .ThenInclude(employee => employee!.Position);
    }

    public IEnumerable<Course> GetCourses(bool isActive = true)
    {
        return _dbContext.Courses
            .Where(course => course.Active == isActive)
            .Include(course => course.Faculty)
            .Include(course => course.Teacher);
    }

    public IEnumerable<Course?> GetCourseByTeacher(Employee teacher)
    {
        if (teacher.Position?.Title != "Teacher")
        {
            throw new ArgumentException("Employee is not a teacher");
        }

        return _dbContext.Courses
            .Include(course => course.Teacher)
            .Where(course => course.Teacher == teacher);
    }

    /// <summary>
    /// Get the total or average salary for each faculty as a tuple of (FacultyId, Name, TotalSalary).
    /// </summary>
    /// <returns>
    /// IEnumerable of tuples of (FacultyId, Name, TotalSalary).
    /// </returns>
    public List<(int? FacultyId, string Name, decimal SalaryValue)> GetFacultySalaries(
        bool average = false
    )
    {
        using SqlConnection connection = new(_dbContext.Database.GetConnectionString());
        using SqlCommand cmd = connection.CreateCommand();
        var function = average ? "AVG(e.Salary)" : "SUM(e.Salary)";

        cmd.CommandType = CommandType.Text;
        cmd.CommandText = (
            $"""
            SELECT f.FacultyId, f.Name, {function} AS TotalSalary
            FROM Employee AS e
            LEFT JOIN Faculty as f ON e.FacultyId = f.FacultyId
            GROUP BY f.FacultyId, f.Name;
            """
        );

        connection.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        var totalSalaries = new List<(int? FacultyId, string Name, decimal SalaryValue)>();
        if (!reader.HasRows)
        {
            return totalSalaries;
        }

        while (reader.Read())
        {
            var salaryTuple = (
                FacultyId: reader.IsDBNull("FacultyId") ? (int?)null : reader.GetInt32("FacultyId"),
                Name: reader.IsDBNull("Name") ? "Faculty-Less" : reader.GetString("Name"),
                SalaryValue: reader.IsDBNull("TotalSalary") ? 0M : reader.GetDecimal("TotalSalary")
            );
            totalSalaries.Add(salaryTuple);
        }

        return totalSalaries;
    }

    /// <summary>
    /// Adds a grade to a student in a course. by sql;
    /// </summary>
    /// <param name="grade">
    /// The grade to add.
    /// </param>
    /// <returns>
    /// True if the grade was added successfully, false otherwise.
    /// </returns>
    public bool AddGrade(Grade grade)
    {
        FormattableString sql = $"""
                BEGIN TRANSACTION
                INSERT INTO Grade (StudentId, CourseId, TeacherId, GradeValue)
                VALUES({grade.Student.StudentId}, {grade.Course.CourseId}, {grade.Teacher.EmployeeId}, {grade.GradeValue})
                COMMIT TRANSACTION;
                """;

        AnsiConsole.WriteLine(sql.ToString());
        int rowsModified;
        try
        {
            rowsModified = _dbContext.Database.ExecuteSql(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return rowsModified > 0;
    }
}
