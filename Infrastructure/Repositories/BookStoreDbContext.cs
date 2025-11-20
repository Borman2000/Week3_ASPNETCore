using Domain.Entities;
using Infrastructure.Repositories.Impl;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class BookStoreDbContext : DbContext, IUnitOfWork
{
	private readonly IMediator _mediator;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
	    base.OnConfiguring(optionsBuilder);
		optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();

// Need net9+ to seed data this way
//		optionsBuilder.UseSeeding((context, _) =>
//			{
//				var testBlog = context.Set<Book>().FirstOrDefault(b => b.Title == "Fahrenheit 451");
//				if (testBlog == null)
//				{
//					context.Set<Book>().Add(
//						new Book {
//							Title = "Fahrenheit 451", ISBN = "978-0-7432-4722-1", Author = new Author("Ray", "Bradbury"),
//							Categories = new List<Category>{new("Dystopian")}, Price = (decimal)9.99, Year = 1953
//						});
//					context.Set<Book>().Add(
//						new Book {
//							Title = "Dandelion Wine", ISBN = "0-553-27753-7", Author = new Author("Ray", "Bradbury"),
//							Categories = new List<Category>{new("Fiction"), new ("Fantasy"), new ("Classics")}, Price = (decimal)7.99, Year = 1957
//						});
//					context.SaveChanges();
//				}
//			})
//			.UseAsyncSeeding(async (context, _, cancellationToken) =>
//			{
//				var testBlog = await context.Set<Book>().FirstOrDefaultAsync(b => b.Title == "Fahrenheit 451", cancellationToken);
//				if (testBlog == null)
//				{
//					context.Set<Book>().Add(
//						new Book {
//							Title = "Fahrenheit 451", ISBN = "978-0-7432-4722-1", Author = new Author("Ray", "Bradbury"),
//							Categories = new List<Category>{new("Dystopian")}, Price = (decimal)9.99, Year = 1953
//						});
//					context.Set<Book>().Add(
//						new Book {
//							Title = "Dandelion Wine", ISBN = "0-553-27753-7", Author = new Author("Ray", "Bradbury"),
//							Categories = new List<Category>{new("Fiction"), new ("Fantasy"), new ("Classics")}, Price = (decimal)7.99, Year = 1957
//						});
//					await context.SaveChangesAsync(cancellationToken);
//				}
//			});
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder.Entity<Category>(category =>
		{
			category.Ignore(c => c.DomainEvents);
			category.HasIndex(c => new { c.Name }).IsUnique();
			category.HasMany(c => c.Books).WithMany(b => b.Categories);
			category.HasData(
				new Category{Name = "Dystopian", Id = new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E751")},
				new Category{Name = "Fantasy", Id = new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E752")},
				new Category{Name = "Classics", Id = new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E753")},
				new Category{Name = "Mystery", Id = new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E754")},
				new Category{Name = "Horror", Id = new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E755")},
				new Category{Name = "Science Fiction", Id = new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E756"),
					Description = "The genre of speculative fiction that imagines advanced and futuristic scientific progress and typically includes " +
					              "elements like information technology and robotics, biological manipulations, space exploration, time travel, " +
					              "parallel universes, and extraterrestrial life."},
				new Category{Name = "Military", Id = new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E757")});
		});

		modelBuilder.Entity<Author>(author =>
		{
			author.Ignore(a => a.DomainEvents);
			author.HasIndex(a => new { a.FirstName, a.LastName }).IsUnique();
			author.HasMany(a => a.Books).WithOne(b => b.Author).HasForeignKey(b => b.AuthorId);
			author.HasData(
				new Author{FirstName = "Isaac", LastName = "Asimov", Id = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D1"), BirthDate = new DateOnly(1920, 1, 2)},
				new Author{FirstName = "Ray", LastName = "Bradbury", Id = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D2"), BirthDate = new DateOnly(1920, 8, 22)},
				new Author{FirstName = "Robert", LastName = "Heinlein", Id = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D3"), BirthDate = new DateOnly(1907, 7, 7), Biography = "Robert Anson Heinlein (July 7, 1907 – May 8, 1988) was an American science fiction author, aeronautical engineer, and naval officer. Sometimes called the \"dean of science fiction writers\", he was among the first to emphasize scientific accuracy in his fiction and was thus a pioneer of the subgenre of hard science fiction. His published works, both fiction and non-fiction, express admiration for competence and emphasize the value of critical thinking. His plots often posed provocative situations which challenged conventional social mores. His work continues to have an influence on the science-fiction genre and on modern culture more generally."},
				new Author{FirstName = "Stephen", LastName = "King", Id = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D4"), BirthDate = new DateOnly(1947, 9, 21)});
		});

	    modelBuilder.Entity<Book>(book =>
	    {
		    book.Ignore(b => b.DomainEvents);
		    book.HasIndex(a => new { a.Title }).IsUnique();
		    book.Property(e => e.Price).HasColumnType("decimal(5, 2)");
		    book.HasData(
			    new Book{Title = "Fahrenheit 451", ISBN = "978-0-7432-4722-1", Id = new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C01"),
				    AuthorId = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D2"), Price = (decimal)9.99, Year = 1953},
			    new Book{Title = "Dandelion Wine", ISBN = "0-553-27753-7", Id = new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C02"),
					AuthorId = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D2"), Price = (decimal)7.99, Year = 1957},
			    new Book{Title = "The Mist", ISBN = "978-0451223296", Id = new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C03"),
					AuthorId = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D4"), Price = (decimal)5.99, Year = 1976},
			    new Book{Title = "The Gods Themselves", ISBN = "0-385-02701-X", Id = new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C04"),
					AuthorId = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D1"), Price = (decimal)8.99, Year = 1972},
			    new Book{Title = "Starship Troopers", ISBN = "978-0450044496", Id = new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C05"),
					AuthorId = new Guid("AB29FC40-CA47-1067-B31D-00DD010662D3"), Price = (decimal)5.99, Year = 1959}

		    );
		    book.HasOne(b => b.Author).WithMany(a => a.Books).HasForeignKey(b => b.AuthorId);
		    book.HasMany(b => b.Categories).WithMany(c => c.Books)
			    .UsingEntity(
				    "BookCategory",
				    r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryId").HasPrincipalKey(nameof(Category.Id)),
				    l => l.HasOne(typeof(Book)).WithMany().HasForeignKey("BookId").HasPrincipalKey(nameof(Book.Id)),
				    je =>
				    {
					    je.HasKey("BookId", "CategoryId");
					    je.HasData(
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E751") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C01") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E752") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C02") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E753") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C02") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E756") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C02") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E755") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C03") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E756") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C03") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E756") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C04") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E756") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C05") }},
							new Dictionary<string, object>{
							    { "CategoryId", new Guid("C4CD02B7-5D56-403D-9041-CC4F3851E757") },
							    { "BookId", new Guid("BE61D971-5EBC-4F02-A3A9-6C82895E5C05") }}
								);
				    });
	    });

	    base.OnModelCreating(modelBuilder);
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options, IMediator mediator) : base(options)
    {
	    _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Commit()
    {
        await SaveChangesAsync();
        await _mediator.DispatchDomainEventsAsync(this);
    }

    public async Task Rollback()
    {
        throw new NotImplementedException();
    }
}