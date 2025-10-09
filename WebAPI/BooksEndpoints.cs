using DataAccess.Repositories.Impl;

namespace WebAPI;

public static class BooksEndpoints
{
    public static void Map(WebApplication app, IBookRepository bookRepoService)
    {
        // app.MapGet("/", () => Results.Redirect("/index.html"));
        app.MapGet("/", () => Results.Redirect("swagger/index.html"));
        app.MapGet("/books", bookRepoService.GetAllAsync);
        app.MapGet("/books/{id:guid}", bookRepoService.GetByIdAsync);
        app.MapPost("/books", bookRepoService.CreateAsync);
        app.MapPut("/books/{id}", bookRepoService.UpdateAsync);
        app.MapDelete("/books/{id:guid}", bookRepoService.DeleteAsync);
    }
}