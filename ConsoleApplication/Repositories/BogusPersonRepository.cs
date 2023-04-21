// using System.Collections;
// using Bogus;
// using Bogus.Extensions.Sweden;
// using ConsoleApplication.Models;
// using ConsoleApplication.Services;
// using Person = ConsoleApplication.Models.Person;
//
// namespace ConsoleApplication.Repositories;
//
// internal class BogusEmployeeRepository : IPersonRepository<Employee>
// {
//     private readonly IList<Employee> _employees;
//
//     public BogusEmployeeRepository(IAdminRepository adminRepository)
//     {
//         var employeeId = 1;
//
//         var personFaker = new Faker<Person>()
//             .RuleFor(p => p.PersonId, _ => employeeId++)
//             .RuleFor(p => p.PersonalIdentityNumber, f => f.Person.Personnummer()[2..])
//             .RuleFor(p => p.FirstName, f => f.Person.FirstName)
//             .RuleFor(p => p.LastName, f => f.Person.LastName);
//         var employeeFaker = new Faker<Employee>()
//             .CustomInstantiator(_ =>
//             {
//                 var (id, pin, fName, lName) = personFaker.Generate();
//                 return new Employee
//                 {
//                     PersonId = id,
//                     PersonalIdentityNumber = pin,
//                     FirstName = fName,
//                     LastName = lName
//                 };
//             })
//             .RuleFor(e => e.Position, f => f.PickRandom(adminRepository.GetAllPositions()))
//             .RuleFor(e => e.HireDate, f => f.Date.PastDateOnly(10))
//             .RuleFor(e => e.Salary, f => f.Random.Decimal(3000, 5000))
//             .FinishWith(
//                 (f, e) =>
//                 {
//                     e.Faculty = e.Position?.Title switch
//                     {
//                         "Teacher" => f.PickRandom(adminRepository.GetAllFaculties()),
//                         "Administrator" => f.PickRandom(adminRepository.GetAllFaculties()),
//                         _ => null
//                     };
//                     if (e.Faculty is not null)
//                     {
//                         e.Faculty.Employees = e.Faculty.Employees.Append(e);
//                     }
//
//                     if (e.Position?.Title == "Teacher")
//                     {
//                         e.Courses = e.Faculty?.Courses!;
//                     }
//                 }
//             );
//
//         _employees = employeeFaker.Generate(10);
//     }
//
//     /// <inheritdoc />
//     public Employee? GetByPIN(string personalIdentityNumber)
//     {
//         return _employees.First(e => e.PersonalIdentityNumber == personalIdentityNumber);
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<Employee> GetAll()
//     {
//         return _employees;
//     }
//
//     /// <inheritdoc />
//     public Employee? Add(StudentRecord person)
//     {
//         throw new NotImplementedException();
//     }
//
//     /// <inheritdoc />
//     public Employee? Add(Employee employee)
//     {
//         _employees.Add(employee);
//         return employee;
//     }
// }
//
// internal class BogusStudentRepository : IStudentRepository
// {
//     private readonly IList<Student> _students;
//     private readonly IAdminRepository _adminRepository;
//     private readonly IPersonRepository<Employee> _employeeRepository;
//
//     public BogusStudentRepository(
//         IAdminRepository adminRepository,
//         IPersonRepository<Employee> employeeRepository
//     )
//     {
//         _adminRepository = adminRepository;
//         _employeeRepository = employeeRepository;
//
//         var studentId = 1;
//
//         var personFaker = new Faker<Person>()
//             .RuleFor(p => p.PersonId, from => studentId++)
//             .RuleFor(p => p.PersonalIdentityNumber, f => f.Person.Personnummer()[2..])
//             .RuleFor(p => p.FirstName, f => f.Person.FirstName)
//             .RuleFor(p => p.LastName, f => f.Person.LastName);
//
//         var studentFaker = new Faker<Student>()
//             .CustomInstantiator(_ =>
//             {
//                 var (id, pin, fName, lName) = personFaker.Generate();
//                 return new Student
//                 {
//                     PersonId = id,
//                     PersonalIdentityNumber = pin,
//                     FirstName = fName,
//                     LastName = lName
//                 };
//             })
//             .RuleFor(s => s.Class, f => f.PickRandom(adminRepository.GetAllClasses()))
//             .RuleFor(
//                 s => s.Courses,
//                 f => f.PickRandom(adminRepository.GetAllCourses(), f.Random.Number(3))
//             )
//             .FinishWith(
//                 (_, s) =>
//                 {
//                     if (!s.Courses.Any())
//                         return;
//                     foreach (var c in s.Courses)
//                     {
//                         c!.Students = c.Students.Append(s);
//                     }
//                 }
//             );
//
//         _students = studentFaker.Generate(25);
//     }
//
//     /// <inheritdoc />
//     public Student? GetByPIN(string personalIdentityNumber)
//     {
//         return _students.First(s => s.PersonalIdentityNumber == personalIdentityNumber);
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<Student> GetAll()
//     {
//         return _students;
//     }
//
//     /// <inheritdoc />
//     public Student? Add(StudentRecord person)
//     {
//         throw new NotImplementedException();
//     }
//
//     /// <inheritdoc />
//     public Student? Add(Student student)
//     {
//         _students.Add(student);
//         return student;
//     }
//
//     // This is going to use the stored procedure requirement
//     /// <inheritdoc />
//     public Hashtable? GetStudentInfoTable(int personId)
//     {
//         Student? student = _students.FirstOrDefault(s => s.PersonId == personId);
//
//         if (student is null)
//         {
//             return null;
//         }
//
//         var studentRecord = new StudentRecord(
//             1,
//             student.PersonalIdentityNumber,
//             $"{student.FirstName} {student.LastName}",
//             student.Class?.ClassName ?? "None"
//         );
//
//         var teachers = student.Courses
//             .Select(
//                 c =>
//                     new TeacherCourseRecord(
//                         $"{c?.Teacher?.FirstName} {c?.Teacher?.LastName}",
//                         c?.Name!
//                     )
//             )
//             .ToArray();
//
//         var courses = student.Courses
//             .Select(c => new CourseRecord(c!.CourseId, c.Name, c.Faculty?.Name!))
//             .ToArray();
//
//         var grades = student.Grades
//             .Select(
//                 g =>
//                     new GradeRecord(
//                         g.Course.Name,
//                         g.GradeValue,
//                         g.DateGraded,
//                         $"{g.Teacher.FirstName} {g.Teacher.LastName}"
//                     )
//             )
//             .ToArray();
//
//         var studentInfo = new Hashtable()
//         {
//             ["student"] = studentRecord,
//             ["teachers"] = teachers,
//             ["courses"] = courses,
//             ["grades"] = grades,
//         };
//
//         return studentInfo;
//     }
// }
