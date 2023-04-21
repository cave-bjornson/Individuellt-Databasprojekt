using System.Collections;
using ConsoleApplication.Models;

namespace ConsoleApplication.Services;

public interface IStudentService : IPersonService<StudentRecord>
{
    public Hashtable? GetStudentInfoTable(int personId);
}
