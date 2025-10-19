using AutoMapper;
using Domain.DTOs;
using Domain.Entities;

namespace Application.Mappings;

public class CategoryResolver2 : IValueResolver<string, Category, string>
{
    public string Resolve(string source, Category destination, string destMember, ResolutionContext context)
    {
Console.WriteLine($"CategoryResolver2: {source}");
        return source;
    }
}
