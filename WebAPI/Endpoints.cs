using DataAccess.Repositories.Impl;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
//        app.MapGet("/", () => Results.Redirect("/index.html"));
        app.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        app.MapGet("/books", (IBookRepository bookRepoService) => bookRepoService.GetAllAsync());
        app.MapGet("/books/{id:guid}",(IBookRepository bookRepoService, [FromRoute] Guid id) =>  bookRepoService.GetByIdAsync(id));
        app.MapPost("/books", (IBookRepository bookRepoService, BookCreateDto bookDto) => bookRepoService.AddAsync(bookDto));
        app.MapPut("/books/", (IBookRepository bookRepoService, BookDto bookDto) => bookRepoService.UpdateAsync(bookDto));
        app.MapDelete("/books/{id:guid}", (IBookRepository bookRepoService, [FromRoute] Guid id) => bookRepoService.DeleteAsync(id));
        app.MapGet("/statistics", (IBookRepository bookRepoService) => bookRepoService.GetStatistics());

        app.MapGet("/authors", (IAuthorRepository authorRepoService) => authorRepoService.GetAllAsync());
//        app.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id));
        app.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id));
        app.MapGet("/authors/{id:guid}/books",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdWithBooksAsync(id));
        app.MapPost("/authors", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.AddAsync(authorDto));
        app.MapPut("/authors/", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.UpdateAsync(authorDto));
        app.MapDelete("/authors/{id:guid}", (IAuthorRepository authorRepoService, [FromRoute] Guid id) => authorRepoService.DeleteAsync(id));

        app.MapGet("/categories", (ICategoryRepository categoryRepoService) => categoryRepoService.GetAllAsync());
        app.MapGet("/categories/{id:guid}",(ICategoryRepository categoryRepoService, [FromRoute] Guid id) =>  categoryRepoService.GetByIdAsync(id));
        app.MapPost("/categories", (ICategoryRepository categoryRepoService, CategoryDto categoryDto) => categoryRepoService.AddAsync(categoryDto));
        app.MapPut("/categories/", (ICategoryRepository categoryRepoService, CategoryDto categoryDto) => categoryRepoService.UpdateAsync(categoryDto));
        app.MapDelete("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) => categoryRepoService.DeleteAsync(id));
    }
}