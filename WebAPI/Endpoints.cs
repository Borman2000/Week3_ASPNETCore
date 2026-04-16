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
using Asp.Versioning;
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ActionResult = Application.DTOs.ActionResult;

namespace WebAPI;

public static class Endpoints
{
	public static void MapAll(WebApplication app)
	{
		MapV1(app);
		MapCQRS(app);
	}

    private static void MapV1(WebApplication app)
    {
	    var v1 = app.NewVersionedApi()
		    .MapGroup("/api/v{version:apiVersion}")
		    .HasDeprecatedApiVersion(new ApiVersion(1,0))
		    .ReportApiVersions();

        v1.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        v1.MapGet("/statistics", (IBookRepository bookRepoService) => bookRepoService.GetStatistics());
        v1.MapGet("/books", (IBookRepository bookRepoService) => bookRepoService.GetAllAsync()).WithName("GetBooksV1");
        v1.MapGet("/books/{id:guid}", (IBookRepository bookRepoService, [FromRoute] Guid id) =>  bookRepoService.GetByIdAsync(id)).RequireAuthorization(ClaimType.Books.Read);;
        v1.MapPost("/books", (IBookRepository bookRepoService, Book book) => bookRepoService.AddAsync(book)).RequireAuthorization(ClaimType.Books.Create);
        v1.MapPut("/books/", (IBookRepository bookRepoService, Book book) => bookRepoService.UpdateAsync(book)).RequireAuthorization(ClaimType.Books.Update);
        v1.MapDelete("/books/{id:guid}", (IBookRepository bookRepoService, [FromRoute] Guid id) => bookRepoService.DeleteAsync(id)).RequireAuthorization(ClaimType.Books.Delete);
        v1.MapGet("/books/search", (IBookRepository bookRepoService, [FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? category, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? page, [FromQuery] int? pageSize) => bookRepoService.Search(page, pageSize:10, title, author, category, minPrice, maxPrice)).RequireAuthorization(ClaimType.Books.Read);

        v1.MapGet("/authors", (IAuthorRepository authorRepoService) => authorRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Authors.Read);
//        v2.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id));
        v1.MapGet("/authors/{id:guid}",(IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdAsync(id)).RequireAuthorization(ClaimType.Authors.Read);
        v1.MapGet("/authors/{id:guid}/books", (IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdWithBooksAsync(id)).RequireAuthorization(ClaimType.Authors.Read);
        v1.MapPost("/authors", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.AddAsync(authorDto)).RequireAuthorization(ClaimType.Authors.Create);
        v1.MapPut("/authors/", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.UpdateAsync(authorDto)).RequireAuthorization(ClaimType.Authors.Update);
        v1.MapDelete("/authors/{id:guid}", (IAuthorRepository authorRepoService, [FromRoute] Guid id) => authorRepoService.DeleteAsync(id)).RequireAuthorization(ClaimType.Authors.Delete);

        v1.MapGet("/categories", (ICategoryRepository categoryRepoService) => categoryRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Categories.Read);
        v1.MapGet("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) =>  categoryRepoService.GetByIdAsync(id)).RequireAuthorization(ClaimType.Categories.Read);
        v1.MapPost("/categories", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.AddAsync(category)).RequireAuthorization(ClaimType.Categories.Create);
        v1.MapPut("/categories/", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.UpdateAsync(category)).RequireAuthorization(ClaimType.Categories.Update);
        v1.MapDelete("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) => categoryRepoService.DeleteAsync(id)).RequireAuthorization(ClaimType.Categories.Delete);
    }

    private static void MapCQRS(WebApplication app)
    {
	    var v2 = app.NewVersionedApi()
		    .MapGroup("/api/v{version:apiVersion}")
		    .HasApiVersion(new ApiVersion(2,0))
		    .ReportApiVersions()
		    .WithOpenApi();

	    v2.MapGet("/", () => Results.Redirect("swagger/index.html")).ExcludeFromDescription();

        v2.MapGet("/books", async ([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? searchTerm, ISender mediatr) => {
	        var books = await mediatr.Send(new GetBooksQuery(page, pageSize, searchTerm));
	        return TypedResults.Ok(books);
        }).ProducesProblem(StatusCodes.Status401Unauthorized);

        v2.MapGet("/books/{id:guid}", async ([FromRoute] Guid id, ISender mediatr) => {
		    BookDto? bookDto = await mediatr.Send(new GetBookByIdQuery(id));
//	        if (bookDto == null) return Results.NotFound();
	        return bookDto != null ? (IResult)TypedResults.NotFound() : TypedResults.Ok(bookDto);
        })
	        .RequireAuthorization(ClaimType.Books.Read)
	        .Produces<BookDto>()
	        .ProducesProblem(StatusCodes.Status404NotFound)
	        .ProducesProblem(StatusCodes.Status401Unauthorized);

	    v2.MapPost("/books", async (CreateBookCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return book == null ? (IResult)TypedResults.Problem("") : TypedResults.Created($"/products/{book.Id}", new ActionResult(book.Id));
        })
		    .RequireAuthorization(ClaimType.Books.Create)
			.Produces<ActionResult>(StatusCodes.Status201Created)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesValidationProblem();

	    v2.MapPut("/books", async (UpdateBookCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return TypedResults.Accepted($"/products/{book.Id}", new ActionResult(book.Id));
        })
		    .RequireAuthorization(ClaimType.Books.Update)
		    .ProducesProblem(StatusCodes.Status401Unauthorized);

	    v2.MapPost("/uploadCsv", async ([FromForm] UploadCsvCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return TypedResults.Created($"/products/{book.Id}", new { id = book.Id });
        })
		    .DisableAntiforgery()
		    .RequireAuthorization(ClaimType.Books.Create)
		    .ProducesProblem(StatusCodes.Status401Unauthorized);

	    v2.MapPost("/uploadCsvMemory", async ([FromForm] UploadCsvSpanCommand command, ISender mediatr) => {
	        var book = await mediatr.Send(command);
	        return TypedResults.Created($"/products/{book.Id}", new { id = book.Id });
        })
		    .DisableAntiforgery()
		    .RequireAuthorization(ClaimType.Books.Create)
		    .ProducesProblem(StatusCodes.Status401Unauthorized);

	    v2.MapGet("/benchmark", () => {
	        var summary = BenchmarkRunner.Run<CsvParserBenchmark>();
	        var p = new Process();
	        p.StartInfo = new ProcessStartInfo(@"BenchmarkDotNet.Artifacts\\results\\CSVParser.CsvParserBenchmark-report.html")
	        {
		        UseShellExecute = true
	        };
	        p.Start();
	        return TypedResults.Ok(GetSummaryAsString(summary));
        });

        v2.MapPost("/authors", async (CreateAuthorCommand command, ISender mediatr) => {
	        var author = await mediatr.Send(command);
	        return TypedResults.Created($"/products/{author.Id}", new ActionResult(author.Id));
        })
	        .RequireAuthorization(ClaimType.Authors.Create)
	        .ProducesProblem(StatusCodes.Status401Unauthorized)
	        .ProducesValidationProblem();

        v2.MapGet("/authors", (IAuthorRepository authorRepoService) => authorRepoService.GetAllAsync()).RequireAuthorization(ClaimType.Authors.Read);
        v2.MapGet("/authors/{id:guid}/books", (IAuthorRepository authorRepoService, [FromRoute] Guid id) =>  authorRepoService.GetByIdWithBooksAsync(id))
	        .RequireAuthorization(ClaimType.Authors.Read)
	        .ProducesProblem(StatusCodes.Status401Unauthorized)
	        .ProducesProblem(StatusCodes.Status404NotFound);

        v2.MapPost("/categories", (ICategoryRepository categoryRepoService, CategoryDto category, IMapper mapper) => categoryRepoService.AddAsync(mapper.Map<Category>(category))).RequireAuthorization(ClaimType.Authors.Create);
//        v2.MapPost("/authors", (IAuthorRepository authorRepoService, AuthorDto authorDto) => authorRepoService.AddAsync(authorDto));

	    v2.MapGet("/categories", (ICategoryRepository categoryRepoService) => categoryRepoService.GetAllAsync()).AddEndpointFilter<HttpResponseEtagFiler>().AddResponseCacheHeader(30).RequireAuthorization(ClaimType.Categories.Read);
	    v2.MapGet("/categories/{id:guid}", (ICategoryRepository categoryRepoService, [FromRoute] Guid id) =>  categoryRepoService.GetByIdAsync(id)).AddEndpointFilter<HttpResponseEtagFiler>().AddResponseCacheHeader(30).RequireAuthorization(ClaimType.Categories.Read);
	    v2.MapPut("/categories/", (ICategoryRepository categoryRepoService, Category category) => categoryRepoService.UpdateAsync(category)).RequireAuthorization(ClaimType.Categories.Update);
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