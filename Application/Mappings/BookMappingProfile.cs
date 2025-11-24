using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class BookMappingProfile : Profile
{
	private List<string> MapCategories(dynamic obj)
    {
        return new List<string>(obj);
    }

    public BookMappingProfile()
    {
	    CreateMap<BookCreateDto, Book>().ConstructUsing((source, dst) =>
	    {
		    return new Book{Title = source.Title, Author = new Author(source.FirstName, source.LastName), Categories = source.Categories.Select(c => new Category(c)).ToList(), ISBN = source.ISBN, Price = source.Price};
	    });

	    CreateMap<Category, String>().ConvertUsing(r => r.Name);
	    CreateMap<Category, CategoryDto>().ReverseMap();
	    CreateMap<Book, BookDto>()
		    .ForMember(dest => dest.DomainEvents, opt => opt.Ignore())
		    .ReverseMap();
	    CreateMap<Author, AuthorDto>()
		    .ForMember(dest => dest.BooksRaw, opt => opt.MapFrom(src => src.Books))
		    .ForMember(dest => dest.DomainEvents, opt => opt.Ignore())
		    .ReverseMap();
    }
}