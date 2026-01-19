using FluentValidation;

namespace Application.Books.UploadCsvSpan;

internal sealed class UploadCsvSpanCommandValidator : AbstractValidator<UploadCsvSpanCommand>
{
    public UploadCsvSpanCommandValidator()
    {
//        RuleFor(c => c.Title)
//            .NotEmpty().WithErrorCode(BookErrorCodes.CreateBook.MissingTitle);
//
//        RuleFor(c => c.Price)
//	        .GreaterThan(0).WithErrorCode(BookErrorCodes.CreateBook.InvalidPrice);
//
//        RuleFor(b => b.ISBN)
//	        .Must(ISBNValidator.IsValid).WithErrorCode(BookErrorCodes.CreateBook.InvalidISBN);
    }
}
