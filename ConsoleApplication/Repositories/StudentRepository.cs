using System.Data;
using ConsoleApplication.Data;
using ConsoleApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace ConsoleApplication.Repositories;

public class StudentRepository
{
    private readonly HighSchoolDBContext _dbContext;

    public StudentRepository(HighSchoolDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    //SELECT s.StudentId, s.PersonalIdentityNumber, s.FirstName, s.LastName, cl.ClassName FROM Student AS sLEFT JOIN Class AS cl ON s.ClassId = cl.ClassId
    public Student? GetStudent(int studentId)
    {
        using SqlConnection connection = new(_dbContext.Database.GetConnectionString());
        using SqlCommand cmd = connection.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = (
            """
            SELECT s.StudentId AS ID, s.PersonalIdentityNumber AS PID, s.FirstName, s.LastName, cl.ClassId AS CID, cl.ClassName
            FROM Student AS s
            LEFT JOIN Class AS cl ON s.ClassId = cl.ClassId
            WHERE s.StudentId = @ID;
            """
        );
        cmd.Parameters.AddWithValue("@ID", studentId);
        var adapter = new SqlDataAdapter(cmd);
        var table = new DataTable();
        adapter.Fill(table);
        if (table.Rows.Count == 0)
        {
            return null;
        }

        DataRow row = table.Rows[0];

        var student = new Student()
        {
            PersonId = row.Field<int>("ID"),
            PersonalIdentityNumber = row.Field<string?>("PID") ?? string.Empty,
            FirstName = row.Field<string>("FirstName") ?? string.Empty,
            LastName = row.Field<string>("LastName") ?? string.Empty,
        };

        var classId = row.Field<int?>("CID");
        var className = row.Field<string?>("ClassName");

        if (classId is not null && className is not null)
        {
            student.Class = new Class() { ClassId = classId, ClassName = className! };
        }

        return student;
    }

    public Student? GetAllStudentInfo(int studentId)
    {
        using SqlConnection connection = new(_dbContext.Database.GetConnectionString());
        using SqlCommand cmd = connection.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GetAllStudentInfo";

        SqlParameter parameter =
            new()
            {
                ParameterName = "ID",
                SqlDbType = SqlDbType.Int,
                SqlValue = studentId
            };
        cmd.Parameters.Add(parameter);

        connection.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        if (!reader.HasRows)
        {
            return null;
        }

        //DataRow row = table.Rows[0];
        reader.Read();
        var student = new Student()
        {
            PersonId = reader.GetFieldValue<int>("StudentId"),
            PersonalIdentityNumber =
                reader.GetFieldValue<string?>("PersonalIdentityNumber") ?? string.Empty,
            FirstName = reader.GetFieldValue<string>("FirstName") ?? string.Empty,
            LastName = reader.GetFieldValue<string>("LastName") ?? string.Empty,
        };

        var classId = reader.GetFieldValue<int?>("ClassId");
        var className = reader.GetFieldValue<string?>("ClassName");

        if (classId is not null && className is not null)
        {
            student.Class = new Class() { ClassId = classId, ClassName = className! };
        }

        // Teachers

        if (reader.NextResult())
        {
            var teachers = new List<Employee>();
            while (reader.Read())
            {
                var teacher = new Employee()
                {
                    PersonId = reader.GetFieldValue<int>("TeacherId"),
                    FirstName = reader.GetFieldValue<string?>("FirstName") ?? string.Empty,
                    LastName = reader.GetFieldValue<string?>("LastName") ?? string.Empty,
                };
                teachers.Add(teacher);
            }

            student.Teachers = teachers;
        }

        // Courses

        if (reader.NextResult())
        {
            var courses = new List<Course>();

            while (reader.Read())
            {
                int? teacherId = reader.GetFieldValue<int?>("TeacherId");
                var course = new Course()
                {
                    CourseId = reader.GetFieldValue<int>("CourseId"),
                    Name = reader.GetFieldValue<string>("CourseName"),
                    Active = reader.GetFieldValue<bool>("Active"),
                    Teacher = student.Teachers.FirstOrDefault(t => t?.PersonId == teacherId),
                };
                courses.Add(course);
            }

            student.Courses = courses;
        }

        // Grades

        if (reader.NextResult())
        {
            var grades = new List<Grade>();

            while (reader.Read())
            {
                int teacherId = reader.GetFieldValue<int>("TeacherId");
                int courseId = reader.GetFieldValue<int>("CourseId");
                var grade = new Grade()
                {
                    Student = student,
                    GradeId = reader.GetFieldValue<int>("GradeId"),
                    GradeValue = (short)reader.GetFieldValue<int>("GradeValue"),
                    DateGraded = DateOnly.FromDateTime(
                        reader.GetFieldValue<DateTime>("DateGraded")
                    ),
                    Course = student.Courses.FirstOrDefault(c => c?.CourseId == courseId)!,
                    Teacher = student.Teachers.FirstOrDefault(t => t?.PersonId == teacherId)!
                };
                grades.Add(grade);
            }

            student.Grades = grades;
        }

        return student;
    }

    public bool AddStudent(Student student)
    {
        FormattableString sql = $"""
            INSERT INTO Student(PersonalIdentityNumber, FirstName, LastName, ClassId)
            SELECT {student.PersonalIdentityNumber}, {student.FirstName}, {student.LastName}, {student.Class?.ClassId}
            """;

        using var context = _dbContext;

        var rowsModified = context.Database.ExecuteSql(sql);

        return rowsModified > 0;
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
                INSERT INTO Grade (StudentCourseId, TeacherId, GradeValue)
                SELECT sc.StudentCourseId, {grade.Teacher.PersonId}, {grade.GradeValue}
                FROM StudentCourse AS sc
                INNER JOIN Course AS c ON c.CourseId = sc.CourseId
                WHERE sc.StudentId = {grade.Student.PersonId} AND sc.CourseId = {grade.Course.CourseId}
                COMMIT TRANSACTION;
                """;

        AnsiConsole.WriteLine(sql.ToString());
        using var context = _dbContext;
        int rowsModified;
        try
        {
            rowsModified = context.Database.ExecuteSql(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return rowsModified > 0;
    }
}
