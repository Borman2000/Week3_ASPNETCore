using AutoMapper;
using Domain.DTOs;
using Domain.Entities;

namespace Application.Mappings;

public class CategoryResolver : IValueResolver<BookCreateDto, Book, List<Category>>
{
    public List<Category> Resolve(BookCreateDto source, Book destination, List<Category> destMember, ResolutionContext context)
    {
Console.WriteLine($"CategoryResolver: {source}");
        // destination.Categories = source.Categories.ConvertAll(input => new Category(input));
        // return source.Categories.ConvertAll(input => new Category(input));
        return new List<Category>();
    }
}
