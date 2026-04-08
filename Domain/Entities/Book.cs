using System.ComponentModel.DataAnnotations;
using Domain.Base;
using Domain.Events;

namespace Domain.Entities;

public class Book : BookBase
{
    [Required(ErrorMessage = "Book author is required")]
    public Author Author {get; set;}

    [Required(ErrorMessage = "Category is required")]
    [Length(1, 10, ErrorMessage = "Should have at least one category")]
    public List<Category> Categories { get; set; }

    public Guid AuthorId {get; set;}

    public Book() {}

    public Book(Guid authorId, string title, decimal price, string isbn) : this()
    {
	    Id = new Guid();
	    AuthorId = authorId;
	    Title = title;
	    Price = price;
	    ISBN = isbn;

	    AddBookCreatedEvent(Id, title, authorId);
    }

    private void AddBookCreatedEvent(Guid id, string title, Guid authorId)
    {
	    var domainEvent = new BookCreatedEvent(id, title, authorId);
	    AddDomainEvent(domainEvent);
    }

    public void AddPriceUpdatedEvent(decimal oldPrice)
    {
	    var domainEvent = new PriceChangedEvent(Id, oldPrice, Price);
	    AddDomainEvent(domainEvent);
    }
}
