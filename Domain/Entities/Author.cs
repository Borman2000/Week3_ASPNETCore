using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Domain.Base;

namespace Domain.Entities;

public class Author : BaseEntity
{
    public Author()
    {
Console.WriteLine("Author constructor 0");
    }

    [SetsRequiredMembers]
    public Author(string firstName, string lastName)
    {
Console.WriteLine("Author constructor 1");
        FirstName = firstName;
        LastName = lastName;
    }

    [SetsRequiredMembers]
    public Author(string firstName, string lastName, DateOnly? birthDate)
    {
Console.WriteLine("Author constructor 2");
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate ?? DateOnly.MinValue;
    }

    [Required(ErrorMessage = "Author's first name is required")]
    [StringLength(20, ErrorMessage = "First name must not exceed 50 characters.")]
    public required string FirstName {get; set;}

    [Required(ErrorMessage = "Author's last name is required")]
    [StringLength(20, ErrorMessage = "Last name must not exceed 50 characters.")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Birth date is required")]
    public DateOnly BirthDate {get; set;}

    public string? Biography {get; set;}

    [JsonIgnore]
    public List<Book> Books {get; set;} = new();
}
