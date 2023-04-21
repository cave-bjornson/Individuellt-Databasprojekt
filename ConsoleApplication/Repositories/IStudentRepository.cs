using System.Collections;
using ConsoleApplication.Models;

namespace ConsoleApplication.Repositories;

public interface IStudentRepository : IPersonRepository<Student>
{
    public Hashtable? GetStudentInfoTable(int personId);
}
