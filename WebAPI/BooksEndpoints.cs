namespace WebAPI;

public static class BooksEndpoints
{
    public static void Map(WebApplication app, IBookRepository bookRepoService)
    {
        app.MapGet("/", () =>
        {
            var html = $$"""
                             <html><body>
                             Available endpoints (each is with cUrl example):<br>
                                GET /books - get all available books (curl --location 'https://localhost:44392/books')<br>
                                GET /books/[GUID] - get book by id (curl --location 'https://localhost:44392/books/19c17fdd-51f4-4957-80bd-558fec19d3e5')<br>
                                POST /books - create new book (JSON-format) (curl --location 'https://localhost:44392/books' --header 'Content-Type: application/json' --data '{ "Title":"Great book", "Author":"John Dow", "Year": 2003, "Price":1.99 }')<br>
                                PUT /books/[GUID] - update book by id (JSON-format) (curl --location --request PUT 'https://localhost:44392/books/19c17fdd-51f4-4957-80bd-558fec19d3e5' --header 'Content-Type: application/json' --data '{"Price":4.99 }')<br>
                                DELETE /books/[GUID] - delete book (curl --location --request DELETE 'https://localhost:44392/books/19c17fdd-51f4-4957-80bd-558fec19d3e5')
                             </body></html>
                         """;
            return Results.Content(html, "text/html");
        });

        app.MapGet("/books", bookRepoService.GetAllAsync);
        app.MapGet("/books/{id:guid}", bookRepoService.GetByIdAsync);
        app.MapPost("/books", bookRepoService.CreateAsync);
        app.MapPut("/books/{id}", bookRepoService.UpdateAsync);
        app.MapDelete("/books/{id:guid}", bookRepoService.DeleteAsync);
    }
}