using Microsoft.EntityFrameworkCore;

namespace WebAPI;

public static class BooksEndpoints
{
    public static void Map(WebApplication app)
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

        app.MapGet("/books", async (BooksDb db) => { var bks = await db.Books.ToListAsync();
            return Results.Ok(bks);
        });

        app.MapGet("/books/{id:guid}", async (Guid id, BooksDb db) =>
            await db.Books.FindAsync(id) is Book book ? Results.Ok(book) : Results.NotFound($"Book with id {id} was not found."));

        app.MapPost("/books", async (Book book, BooksDb db) =>
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
            Console.WriteLine(book);

            return Results.Created($"/books/{book.Id}", book);
        });

        app.MapPut("/books/{id}", async (HttpContext context, string id, BooksDb db) =>
        {
             Guid.TryParse(id, out Guid guidId);
            if(Guid.Empty == guidId) return Results.NotFound($"Book with id {id} was not found: invalid id format");

            var book = await db.Books.FindAsync(guidId);
            if (book is null) return Results.NotFound($"Book with id {guidId} was not found.");

            Book? source;

            if (context.Request.HasJsonContentType())
                source = await context.Request.ReadFromJsonAsync<Book>();
            else
                return Results.BadRequest("No data for update");

            if (source is not null)
            {
                book.Author = source.Author ?? book.Author;
                book.Title = source.Title ?? book.Title;
                book.ISBN = source.ISBN ?? book.ISBN;
                book.Year = source.Year > 1900 ? source.Year : book.Year;
                book.Price = source.Price > 0 ? source.Price : book.Price;
            }

            await db.SaveChangesAsync();

            return Results.Ok($"Book with id {book.Id} was updated.");
        });

        app.MapDelete("/books/{id:guid}", async (Guid id, BooksDb db) =>
        {
            if (await db.Books.FindAsync(id) is not Book book) return Results.NotFound($"Book with id {id} was not found.");

            db.Books.Remove(book);
            await db.SaveChangesAsync();
            return Results.NoContent();

        });
    }
}