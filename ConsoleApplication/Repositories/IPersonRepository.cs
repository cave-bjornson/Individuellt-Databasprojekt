using ConsoleApplication.Models;

namespace ConsoleApplication.Repositories;

public interface IPersonRepository<T> where T : Person
{
    /// <summary>
    /// Get a person by their personal identity number.
    /// </summary>
    /// <param name="personalIdentityNumber">
    /// The personal identity number of the person to get.
    /// </param>
    /// <typeparam name="T">
    /// The type of person to get.
    /// </typeparam>
    /// <returns>
    /// The person with the specified personal identity number.
    /// </returns>
    public T? GetByPIN(string personalIdentityNumber);

    /// <summary>
    /// Get all persons of a specific type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of persons to get.
    /// </typeparam>
    /// <returns>
    /// All persons of the specified type.
    /// </returns>
    public IEnumerable<T> GetAll();

    public T? Add(T person);
}
