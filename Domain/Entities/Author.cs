using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Domain.Base;
using Domain.Events;

namespace Domain.Entities;

public class Author : BaseEntity
{
    public Author()
    {
    }

    [SetsRequiredMembers]
    public Author(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    [SetsRequiredMembers]
    public Author(string firstName, string lastName, DateOnly? birthDate)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate ?? DateOnly.MinValue;
    }

    [SetsRequiredMembers]
    public Author(string firstName, string lastName, DateOnly? birthDate, string? email, string? biography)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate ?? DateOnly.MinValue;
        Email = email;
        Biography = biography;

        AddAuthorCreatedEvent(Id, firstName, lastName);
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

    public string? Email {get; set;}

    [JsonIgnore]
    public List<Book> Books {get; set;} = new();

    private void AddAuthorCreatedEvent(Guid id, string fName, string lName)
    {
	    var domainEvent = new AuthorCreatedEvent(id, fName, lName);
	    this.AddDomainEvent(domainEvent);
    }
}
