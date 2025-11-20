using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class BookMappingProfile : Profile
{
	private List<string> MapCategories(dynamic obj)
    {
Console.WriteLine($"MapCategories: ${obj}");
        return new List<string>(obj);
    }

    public BookMappingProfile()
    {
//	    CreateMap<BookCreateDto, Book>();
	    CreateMap<BookCreateDto, Book>().ConstructUsing((source, dst) =>
	    {
		    return new Book{Title = source.Title, Author = new Author(source.FirstName, source.LastName), Categories = source.Categories.Select(c => new Category(c)).ToList(), ISBN = source.ISBN, Price = source.Price};
	    });

	    // CreateMap<dynamic, Book>().ConstructUsing((source, dst) =>
	    // {
	    //     return new Book{Title = source.Title, Author = new Author(source.FirstName, source.LastName), Categories = source.Categories.Select(new Func<object, Category>(c => new Category((string)c))).ToList(), ISBN = source.ISBN, Price = source.Price};
	    // });
	    CreateMap<Category, String>().ConvertUsing(r => r.Name);
	    CreateMap<Category, CategoryDto>().ReverseMap();
	    CreateMap<Book, BookDto>().ForMember(dest => dest.DomainEvents, opt => opt.Ignore()).ReverseMap();
//	        .ForSourceMember(src => src.Author, opt => opt.DoNotValidate());
//	        .ForMember(dest => dest.FirstName, opt => opt.Ignore())
//	        .ForMember(dest => dest.LastName, opt => opt.Ignore());
	    CreateMap<Author, AuthorDto>()
		    .ForMember(dest => dest.BooksRaw, opt => opt.MapFrom(src => src.Books))
		    .ReverseMap();
    }
}