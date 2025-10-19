namespace Domain.Validations;

using System.ComponentModel.DataAnnotations;

public class IsbnValidationAttribute : ValidationAttribute
{
    public IsbnValidationAttribute() : base("Invalid ISBN format.") { }

    public override bool IsValid(object? value)
    {
        string isbn = value.ToString();

        // Implement your ISBN validation logic here.
        // This could involve:
        // - Checking length (10 or 13 digits)
        // - Removing hyphens or spaces
        // - Calculating and verifying the check digit
        // - Using regular expressions for pattern matching

        // Example (simplified check for ISBN-13 length):
        if (isbn.Replace("-", "").Length == 13)
        {
            return true; // Placeholder for actual ISBN-13 validation
        }
        // Example (simplified check for ISBN-10 length):
        if (isbn.Replace("-", "").Length == 10)
        {
            return true; // Placeholder for actual ISBN-10 validation
        }

        return false; // ISBN is not valid
    }
}