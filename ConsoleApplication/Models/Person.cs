namespace ConsoleApplication.Models;

public class Person
{
    public int? PersonId { get; set; }
    public string PersonalIdentityNumber { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Person() { }

    public Person(int? personId, string personalIdentityNumber, string firstName, string lastName)
    {
        PersonId = personId;
        PersonalIdentityNumber = personalIdentityNumber;
        FirstName = firstName;
        LastName = lastName;
    }

    public void Deconstruct(
        out int? personId,
        out string personalIdentityNumber,
        out string firstName,
        out string lastName
    )
    {
        personId = PersonId;
        personalIdentityNumber = PersonalIdentityNumber;
        firstName = FirstName;
        lastName = LastName;
    }
}
