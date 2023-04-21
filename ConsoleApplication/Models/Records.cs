namespace ConsoleApplication.Models;

public record StudentRecord(
    int StudentId,
    string PersonalIdentityNumber,
    string Name,
    string Class
);

public record TeacherCourseRecord(string TeacherName, string CourseName);

public record CourseRecord(int CourseId, string Name, string FacultyName);

public record GradeRecord(
    string CourseName,
    int GradeValue,
    DateOnly DateGraded,
    string TeacherName
);

// public static class RecordMapper
// {
//     public static StudentRecord? ToStudentRecord(Student? s)
//     {
//         if (s is null)
//             return null;
//         return new StudentRecord(
//             s.PersonId,
//             s.PersonalIdentityNumber,
//             $"{s.FirstName} {s.LastName}",
//             s.Class?.ClassName ?? "None"
//         );
//     }
// }
