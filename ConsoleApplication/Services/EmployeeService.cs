using ConsoleApplication.Models;
using ConsoleApplication.Repositories;
using Personnummer;
using Personnummer.Exceptions;

namespace ConsoleApplication.Services;

class EmployeeService : IPersonService<Employee>
{
    private readonly IPersonRepository<Employee> _personRepository;

    public EmployeeService(IPersonRepository<Employee> personRepository)
    {
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public Employee? GetByPIN(string personalIdentityNumber)
    {
        return _personRepository.GetByPIN(personalIdentityNumber);
    }

    /// <inheritdoc />
    public IEnumerable<Employee> GetAll()
    {
        return _personRepository.GetAll();
    }

    /// <inheritdoc />
    public Employee? Add(Employee person)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Employee? Add(Student person)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    // public Employee? Add(Employee newEmployee)
    // {
    //     return _personRepository.Add(newEmployee);
    // }
}
