using Domain.Validations;
using FluentValidation;

namespace Application.Books.Create;

internal sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithErrorCode(BookErrorCodes.CreateBook.MissingTitle);

        RuleFor(c => c.Price)
	        .GreaterThan(0).WithErrorCode(BookErrorCodes.CreateBook.InvalidPrice);

        RuleFor(b => b.ISBN)
	        .Must(ISBNValidator.IsValid).WithErrorCode(BookErrorCodes.CreateBook.InvalidISBN);
    }
}
