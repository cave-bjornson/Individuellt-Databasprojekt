using System.Collections;
using ConsoleApplication.Models;
using ConsoleApplication.Repositories;

namespace ConsoleApplication.Services;

public class StudentService
{
    private readonly StudentRepository _studentRepository;
    //private readonly IAdminRepository _adminRepository;

    public StudentService(StudentRepository studentRepository) //IAdminRepository adminRepository)
    {
        _studentRepository = studentRepository;
        //_adminRepository = adminRepository;
    }

    /// <inheritdoc />
    public StudentRecord? GetByPIN(string personalIdentityNumber)
    {
        return null;
    }

    /// <inheritdoc />
    public IEnumerable<StudentRecord?> GetAll()
    {
        return null;
    }

    /// <inheritdoc />
    public StudentRecord? Add(StudentRecord person)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public StudentRecord? Add(Student person)
    {
        throw new NotImplementedException();
    }

    // /// <inheritdoc />
    // public Student? Add(StudentRecord studentRecord)
    // {
    //     return _studentRepository.Add(null);
    // }

    /// <inheritdoc />
    // public Hashtable? GetStudentInfoTable(int personId)
    // {
    //     return _studentRepository.GetStudentInfoTable(personId);
    // }

    public Student? GetStudentByID(int id)
    {
        return _studentRepository.GetAllStudentInfo(id);
    }
}
