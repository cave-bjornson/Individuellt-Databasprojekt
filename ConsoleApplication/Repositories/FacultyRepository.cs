using ConsoleApplication.Data;
using ConsoleApplication.Models;

namespace ConsoleApplication.Repositories;

public class FacultyRepository
{
    private readonly HighSchoolDBContext _dbContext;

    public FacultyRepository(HighSchoolDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Faculty> GetAllFaculties()
    {
        return _dbContext.Faculties.ToList();
    }
}
