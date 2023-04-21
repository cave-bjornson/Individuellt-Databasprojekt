// using Bogus;
// using ConsoleApplication.Models;
//
// namespace ConsoleApplication.Repositories;
//
// internal class BogusAdminRepository : IAdminRepository
// {
//     private readonly Position[] _positions;
//     private readonly Class[] _classes;
//     private readonly Faculty[] _faculties;
//     private readonly Course[] _courses;
//     private readonly List<Grade> _grades = new();
//
//     private readonly string[] _positionTitles =
//     {
//         "Administrator",
//         "Custodian",
//         "Guidance counselor",
//         "Librarian",
//         "Nurse",
//         "Principal",
//         "Teacher"
//     };
//
//     // Go from 9A grade to 12C grade
//     private readonly string[] classNames =
//     {
//         "9A",
//         "9B",
//         "9C",
//         "10A",
//         "10B",
//         "10C",
//         "11A",
//         "11B",
//         "11C",
//         "12A",
//         "12B",
//         "12C"
//     };
//
//     private readonly Dictionary<string, string[]> facultyCourseDict =
//         new()
//         {
//             ["Science"] = new[] { "Biology", "Chemistry" },
//             ["Maths"] = new[] { "Algebra", "Computer Science" },
//             ["Physical Education"] = new[] { "Physical Education" },
//             ["Social Science"] = new[] { "History", "Contemporary American Heroes" },
//             ["Humanities"] = new[] { "English", "Literature" }
//         };
//
//     public BogusAdminRepository()
//     {
//         _positions = _positionTitles.Select(pT => new Position() { Title = pT }).ToArray();
//         var classId = 1;
//         _classes = classNames
//             .Select(cN => new Class() { ClassId = classId++, ClassName = cN })
//             .ToArray();
//
//         var courseId = 0;
//         _faculties = facultyCourseDict.Keys
//             .ToArray()
//             .Select(
//                 (
//                     k =>
//                     {
//                         var f = new Faculty() { Name = k };
//                         // f.Courses = facultyCourseDict[k]
//                         //     .Select(
//                         //         c =>
//                         //             new Course()
//                         //             {
//                         //                 CourseId = courseId++,
//                         //                 Name = c,
//                         //                 Active = new Faker().Random.Bool(),
//                         //                 Faculty = f
//                         //             }
//                         //     )
//                         //     .ToArray();
//                         // return f;
//                     }
//                 )
//             )
//             .ToArray();
//
//         //_courses = _faculties.SelectMany(f => f.Courses).ToArray();
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<Position> GetAllPositions()
//     {
//         return _positions;
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<Class> GetAllClasses()
//     {
//         return _classes;
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<Faculty> GetAllFaculties()
//     {
//         return _faculties;
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<Course> GetAllCourses()
//     {
//         return _courses;
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<Grade> GetAllGrades()
//     {
//         return _grades;
//     }
//
//     /// <inheritdoc />
//     public Grade? AddGrade(Grade grade)
//     {
//         _grades.Add(grade);
//         return grade;
//     }
// }
