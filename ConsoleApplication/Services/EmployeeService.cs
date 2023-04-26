using System.Data;
using ConsoleApplication.Data;
using ConsoleApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Services;

public class EmployeeService
{
    private readonly HighSchoolDBContext _dbContext;

    public EmployeeService(HighSchoolDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool AddEmployee(Employee e)
    {
        FormattableString sql = $"""
            INSERT INTO Employee(PersonalIdentityNumber, FirstName, LastName, PositionId, HireDate, FacultyId, Salary)
            VALUES({e.PersonalIdentityNumber}, {e.FirstName}, {e.LastName}, {e.Position?.PositionId}, {e.HireDate.ToDateTime(new TimeOnly())}, {e.Faculty?.FacultyId}, {e.Salary})
            """;

        var rowsModified = _dbContext.Database.ExecuteSql(sql);

        return rowsModified > 0;
    }

    public IEnumerable<Employee?> GetAllEmployees()
    {
        using SqlConnection connection = new(_dbContext.Database.GetConnectionString());
        using SqlCommand cmd = connection.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = (
            """
            SELECT e.EmployeeId AS ID, e.PersonalIdentityNumber AS personId, e.FirstName, e.LastName, e.HireDate, e.Salary,
            p.PositionId, p.Title,
            f.FacultyId, f.Name As FacultyName
            FROM Employee AS e
            LEFT JOIN Position AS p ON e.PositionId = p.PositionId
            LEFT JOIN Faculty AS f ON e.FacultyId = f.FacultyId
            """
        );

        SqlDataAdapter adapter = new(cmd);
        DataTable employeeTable = new();
        adapter.Fill(employeeTable);

        var employees = new List<Employee>();

        if (employeeTable.Rows.Count == 0)
        {
            return employees;
        }

        foreach (DataRow row in employeeTable.Rows)
        {
            var employee = new Employee()
            {
                EmployeeId = row.Field<int>("ID"),
                PersonalIdentityNumber = row.Field<string?>("personId") ?? string.Empty,
                FirstName = row.Field<string>("FirstName") ?? string.Empty,
                LastName = row.Field<string>("LastName") ?? string.Empty,
                HireDate = DateOnly.FromDateTime(row.Field<DateTime>("HireDate")),
                Salary = row.Field<decimal>("Salary"),
            };

            var posId = row.Field<int?>("PositionId");
            var posTitle = row.Field<string?>("Title");

            if (posId is not null && posTitle is not null)
            {
                employee.Position = new Position() { PositionId = posId, Title = posTitle };
            }

            var facId = row.Field<int?>("FacultyId");
            var facTitle = row.Field<string?>("FacultyName");

            if (facId is not null && facTitle is not null)
            {
                employee.Faculty = new Faculty() { FacultyId = facId, Name = facTitle };
            }

            employees.Add(employee);
        }

        return employees;
    }
}
