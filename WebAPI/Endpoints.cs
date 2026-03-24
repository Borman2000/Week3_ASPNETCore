using System.Diagnostics;
using Application.Authors.Create;
using Application.Books.Create;
using Application.Books.GetById;
using Application.Books.GetPagedList;
using Application.Books.Update;
using Application.Books.UploadCsv;
using Application.Books.UploadCsvSpan;
using Application.DTOs;
using Application.Interfaces;
using AuthAPI.Domain;
using AutoMapper;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using CSVParser;
using Domain.Entities;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI;

public static class Endpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        app.MapGet("/books", (IBookRepository bookRepoService) => bookRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Books.Read);
        app.MapGet("/books/{id:guid}", (IBookRepository bookRepoService, [FromRoute] Guid id) =>  bookRepoService.GetByIdAsync(id)).RequireAuthorization(ClaimType.Books.Read);;
        app.MapPost("/books", (IBookRepository bookRepoService, Book book) => bookRepoService.AddAsync(book)).RequireAuthorization(ClaimType.Books.Create);
        app.MapPut("/books/", (IBookRepository bookRepoService, Book book) => bookRepoService.UpdateAsync(book)).RequireAuthorization(ClaimType.Books.Update);
        app.MapDelete("/books/{id:guid}", (IBookRepository bookRepoService, [FromRoute] Guid id) => bookRepoService.DeleteAsync(id)).RequireAuthorization(ClaimType.Books.Delete);
        app.MapGet("/books/search", (IBookRepository bookRepoService, [FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? category, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? page, [FromQuery] int? pageSize) => bookRepoService.Search(page, pageSize:10, title, author, category, minPrice, maxPrice)).RequireAuthorization(ClaimType.Books.Read);
        app.MapGet("/statistics", (IBookRepository bookRepoService) => bookRepoService.GetStatistics());

        app.MapGet("/authors", (IAuthorRepository authorRepoService) => authorRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Authors.Read);
//        app.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id));
        app.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id)).RequireAuthorization(ClaimType.Authors.Read);
        app.MapGet("/authors/{id:guid}/books", (IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdWithBooksAsync(id)).RequireAuthorization(ClaimType.Authors.Read);
        app.MapPost("/authors", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.AddAsync(authorDto)).RequireAuthorization(ClaimType.Authors.Create);
        app.MapPut("/authors/", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.UpdateAsync(authorDto)).RequireAuthorization(ClaimType.Authors.Update);
        app.MapDelete("/authors/{id:guid}", (IAuthorRepository authorRepoService, [FromRoute] Guid id) => authorRepoService.DeleteAsync(id)).RequireAuthorization(ClaimType.Authors.Delete);

        app.MapGet("/categories", (ICategoryRepository categoryRepoService) => categoryRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Categories.Read);
        app.MapGet("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) =>  categoryRepoService.GetByIdAsync(id)).RequireAuthorization(ClaimType.Categories.Read);
        app.MapPost("/categories", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.AddAsync(category)).RequireAuthorization(ClaimType.Categories.Create);
        app.MapPut("/categories/", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.UpdateAsync(category)).RequireAuthorization(ClaimType.Categories.Update);
        app.MapDelete("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) => categoryRepoService.DeleteAsync(id)).RequireAuthorization(ClaimType.Categories.Delete);
    }

    public static void MapCQRS(WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        app.MapGet("/books", async ([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? searchTerm, ISender mediatr) => {
	        var books = await mediatr.Send(new GetBooksQuery(page, pageSize, searchTerm));
	        return Results.Ok(books);
        }).RequireAuthorization(ClaimType.Books.Read);
        app.MapGet("/books/{id:guid}", async ([FromRoute] Guid id, ISender mediatr) => {
	        var bookDto = await mediatr.Send(new GetBookByIdQuery(id));
	        if (bookDto == null) return Results.NotFound();
	        return Results.Ok(bookDto);
        }).RequireAuthorization(ClaimType.Books.Read);
        app.MapPost("/books", async (CreateBookCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return book == null ? Results.Problem("") : Results.Created($"/products/{book.Id}", new { id = book.Id });
        }).RequireAuthorization(ClaimType.Books.Create);
        app.MapPut("/books", async (UpdateBookCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return Results.Accepted($"/products/{book.Id}", new { id = book.Id });
        }).RequireAuthorization(ClaimType.Books.Update);
        app.MapPost("/uploadCsv", async ([FromForm] UploadCsvCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return Results.Created($"/products/{book.Id}", new { id = book.Id });
        }).DisableAntiforgery().RequireAuthorization(ClaimType.Books.Create);
        app.MapPost("/uploadCsvMemory", async ([FromForm] UploadCsvSpanCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return Results.Created($"/products/{book.Id}", new { id = book.Id });
        }).DisableAntiforgery().RequireAuthorization(ClaimType.Books.Create);
        app.MapGet("/benchmark", () => {
	        var summary = BenchmarkRunner.Run<CsvParserBenchmark>();
	        var p = new Process();
	        p.StartInfo = new ProcessStartInfo(@"BenchmarkDotNet.Artifacts\\results\\CSVParser.CsvParserBenchmark-report.html")
	        {
		        UseShellExecute = true
	        };
	        p.Start();
	        return Results.Ok(GetSummaryAsString(summary));
        });

        app.MapPost("/authors", async (CreateAuthorCommand command, ISender mediatr) => {
	        var author = await mediatr.Send(command);
	        return Results.Created($"/products/{author.Id}", new { id = author.Id });
        }).RequireAuthorization(ClaimType.Authors.Create);

        app.MapGet("/authors", (IAuthorRepository authorRepoService) => authorRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Authors.Read);
        app.MapGet("/authors/{id:guid}/books", (IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdWithBooksAsync(id)).RequireAuthorization(ClaimType.Authors.Read);
        app.MapPost("/categories", (ICategoryRepository categoryRepoService, CategoryDto category, IMapper mapper) => categoryRepoService.AddAsync(mapper.Map<Category>(category))).RequireAuthorization(ClaimType.Authors.Create);
//        app.MapPost("/authors", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.AddAsync(authorDto));

	    app.MapGet("/categories", (ICategoryRepository categoryRepoService) => categoryRepoService.GetAllAsync()).AddEndpointFilter<HttpResponseEtagFiler>().AddResponseCacheHeader(30).RequireAuthorization(ClaimType.Categories.Read);
	    app.MapGet("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) =>  categoryRepoService.GetByIdAsync(id)).AddEndpointFilter<HttpResponseEtagFiler>().AddResponseCacheHeader(30).RequireAuthorization(ClaimType.Categories.Read);
	    app.MapPut("/categories/", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.UpdateAsync(category)).RequireAuthorization(ClaimType.Categories.Update);
    }

    private static string GetSummaryAsString(Summary summary)
    {
	    var stringWriter = new StringWriter();
	    var consoleLogger = new TextLogger(stringWriter);

	    // Use any of the built-in exporters, for example, MarkdownExporter.Default
	    // Other options include HtmlExporter.Default, CsvExporter.Default, etc.
	    var exporter = MarkdownExporter.Default;
//	    var exporter = HtmlExporter.Default;
//	    var exporter = CsvExporter.Default;
//exporter = XmlExporter.Default;

	    // The ExportToLog method writes the formatted summary to the logger (StringWriter)
	    exporter.ExportToLog(summary, consoleLogger);

	    return stringWriter.ToString();
    }

}