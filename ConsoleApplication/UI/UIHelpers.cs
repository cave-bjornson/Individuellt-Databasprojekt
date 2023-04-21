using System.Text.RegularExpressions;
using Spectre.Console;

namespace ConsoleApplication.UI;

public static class UIHelpers
{
    public static (string firstName, string lastName, string pin) AskNamesAndPIN()
    {
        string firstName = AnsiConsole.Ask<string>("Enter first name: ");
        string lastName = AnsiConsole.Ask<string>("Enter last name: ");
        // Does not check if the PIN is valid more than the string is 10 digits long.
        string pin = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter personal identity number: ")
                .ValidationErrorMessage("Not a valid input")
                .Validate(
                    pin =>
                        Regex.IsMatch(pin, @"^\d{10}$") // && Personnummer.Personnummer.Valid(pin) // Can't remember the rules of SSN when inputting in console.
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Enter 10 digits that is a valid PIN")
                )
        );
        return (firstName, lastName, pin);
    }
}
