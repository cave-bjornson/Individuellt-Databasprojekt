using System.Data;
using ConsoleApplication.Data;
using ConsoleApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Services;

public class StudentService
{
    private readonly HighSchoolDBContext _dbContext;

    public StudentService(HighSchoolDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Student?> GetAll()
    {
        return _dbContext.Students.Include(student => student.Class);
    }

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
            StudentId = row.Field<int>("ID"),
            PersonalIdentityNumber = row.Field<string?>("PID") ?? string.Empty,
            FirstName = row.Field<string>("FirstName") ?? string.Empty,
            LastName = row.Field<string>("LastName") ?? string.Empty,
        };

        var classId = row.Field<int?>("CID");
        var className = row.Field<string?>("ClassName");

        if (classId is not null && className is not null)
        {
            student.Class = new Class() { ClassId = classId, ClassName = className };
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
            StudentId = reader.GetFieldValue<int>("StudentId"),
            PersonalIdentityNumber = reader.GetFieldValue<string>("PersonalIdentityNumber"),
            FirstName = reader.GetFieldValue<string>("FirstName"),
            LastName = reader.GetFieldValue<string>("LastName"),
        };

        var classId = reader.IsDBNull("ClassId") ? null : reader.GetFieldValue<int?>("ClassId");
        var className = reader.IsDBNull("ClassName")
            ? null
            : reader.GetFieldValue<string?>("ClassName") ?? null;

        if (classId is not null && className is not null)
        {
            student.Class = new Class() { ClassId = classId, ClassName = className };
        }

        // Teachers

        if (reader.NextResult())
        {
            while (reader.Read())
            {
                var teacher = new Employee()
                {
                    EmployeeId = reader.GetFieldValue<int>("TeacherId"),
                    PersonalIdentityNumber = reader.GetFieldValue<string>("PersonalIdentityNumber"),
                    FirstName = reader.GetFieldValue<string?>("FirstName") ?? string.Empty,
                    LastName = reader.GetFieldValue<string?>("LastName") ?? string.Empty,
                };
                student.Teachers.Add(teacher);
            }
        }

        // Courses

        if (reader.NextResult())
        {
            while (reader.Read())
            {
                int? teacherId = reader.GetFieldValue<int?>("TeacherId");
                var course = new Course()
                {
                    CourseId = reader.GetFieldValue<int>("CourseId"),
                    Name = reader.GetFieldValue<string>("CourseName"),
                    Active = reader.GetFieldValue<bool>("Active"),
                    Teacher = student.Teachers.FirstOrDefault(t => t?.EmployeeId == teacherId),
                };
                student.Courses.Add(course);
            }
        }

        // Grades

        if (reader.NextResult())
        {
            while (reader.Read())
            {
                int teacherId = reader.GetFieldValue<int>("TeacherId");
                int courseId = reader.GetFieldValue<int>("CourseId");
                var grade = new Grade()
                {
                    Student = student,
                    GradeId = reader.GetFieldValue<int>("GradeId"),
                    GradeValue = reader.GetFieldValue<int>("GradeValue"),
                    DateGraded = DateOnly.FromDateTime(
                        reader.GetFieldValue<DateTime>("DateGraded")
                    ),
                    Course = student.Courses.FirstOrDefault(c => c?.CourseId == courseId)!,
                    Teacher = student.Teachers.FirstOrDefault(t => t?.EmployeeId == teacherId)!
                };
                student.Grades.Add(grade);
            }
        }

        return student;
    }

    public IEnumerable<Student?> GetUngradedStudentsInCourse(Course course)
    {
        return _dbContext.Students
            .AsSplitQuery()
            .Include(student => student.Courses)
            .Include(student => student.Grades)
            .Where(
                student =>
                    student.Courses.Contains(course)
                    && student.Grades.All(grade => grade!.Course != course)
            );
    }

    public bool AddStudent(Student student)
    {
        FormattableString sql = $"""
            INSERT INTO Student(PersonalIdentityNumber, FirstName, LastName, ClassId)
            SELECT {student.PersonalIdentityNumber}, {student.FirstName}, {student.LastName}, {student.Class?.ClassId}
            """;

        var rowsModified = _dbContext.Database.ExecuteSql(sql);

        return rowsModified > 0;
    }
}
