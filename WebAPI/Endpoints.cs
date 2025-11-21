using Application.Authors.Create;
using Application.Books.Create;
using Application.Books.GetById;
using Application.Books.GetPagedList;
using Application.Books.Update;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
//        app.MapGet("/", () => Results.Redirect("/index.html"));
        app.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        app.MapGet("/books", (IBookRepository bookRepoService) => bookRepoService.GetAllAsync());
        app.MapGet("/books/{id:guid}", (IBookRepository bookRepoService, [FromRoute] Guid id) =>  bookRepoService.GetByIdAsync(id));
        app.MapPost("/books", (IBookRepository bookRepoService, Book book) => bookRepoService.AddAsync(book));
        app.MapPut("/books/", (IBookRepository bookRepoService, Book book) => bookRepoService.UpdateAsync(book));
        app.MapDelete("/books/{id:guid}", (IBookRepository bookRepoService, [FromRoute] Guid id) => bookRepoService.DeleteAsync(id));
        app.MapGet("/books/search", (IBookRepository bookRepoService, [FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? category, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? page, [FromQuery] int? pageSize) => bookRepoService.Search(page, pageSize:10, title, author, category, minPrice, maxPrice));
        app.MapGet("/statistics", (IBookRepository bookRepoService) => bookRepoService.GetStatistics());

        app.MapGet("/authors", (IAuthorRepository authorRepoService) => authorRepoService.GetAllAsync());
//        app.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id));
        app.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id));
        app.MapGet("/authors/{id:guid}/books", (IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdWithBooksAsync(id));
        app.MapPost("/authors", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.AddAsync(authorDto));
        app.MapPut("/authors/", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.UpdateAsync(authorDto));
        app.MapDelete("/authors/{id:guid}", (IAuthorRepository authorRepoService, [FromRoute] Guid id) => authorRepoService.DeleteAsync(id));

        app.MapGet("/categories", (ICategoryRepository categoryRepoService) => categoryRepoService.GetAllAsync());
        app.MapGet("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) =>  categoryRepoService.GetByIdAsync(id));
        app.MapPost("/categories", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.AddAsync(category));
        app.MapPut("/categories/", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.UpdateAsync(category));
        app.MapDelete("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) => categoryRepoService.DeleteAsync(id));
    }

    public static void MapCQRS(WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        app.MapGet("/books", async ([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? searchTerm, ISender mediatr) => {
	        var books = await mediatr.Send(new GetBooksQuery(page, pageSize, searchTerm));
	        return Results.Ok(books);
        });
        app.MapGet("/books/{id:guid}", async ([FromRoute] Guid id, ISender mediatr) => {
	        var bookDto = await mediatr.Send(new GetBookByIdQuery(id));
	        if (bookDto == null) return Results.NotFound();
	        return Results.Ok(bookDto);
        });
        app.MapPost("/books", async (CreateBookCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return Results.Created($"/products/{book.Id}", new { id = book.Id });
        });
        app.MapPut("/books", async (UpdateBookCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return Results.Accepted($"/products/{book.Id}", new { id = book.Id });
        });

        app.MapPost("/authors", async (CreateAuthorCommand command, ISender mediatr) => {
	        var author = await mediatr.Send(command);
	        return Results.Created($"/products/{author.Id}", new { id = author.Id });
        });
    }
}