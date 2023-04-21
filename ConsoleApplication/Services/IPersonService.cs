using ConsoleApplication.Models;

namespace ConsoleApplication.Services;

public interface IPersonService<T>
{
    public T? GetByPIN(string personalIdentityNumber);

    public IEnumerable<T?> GetAll();

    public T? Add(T person);
}
