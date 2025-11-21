using Domain.Validations;
using FluentValidation;

namespace Application.Authors.Create;

internal sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty().WithErrorCode(AuthorErrorCodes.CreateAuthor.MissingFirstName);

        RuleFor(c => c.LastName)
            .NotEmpty().WithErrorCode(AuthorErrorCodes.CreateAuthor.MissingLastName);

        RuleFor(c => c.Email).EmailAddress().WithErrorCode(AuthorErrorCodes.CreateAuthor.InvalidEmail);

        RuleFor(c => c.BirthDate).NotNull().WithErrorCode(AuthorErrorCodes.CreateAuthor.InvalidBirthDate);
        RuleFor(c => c.BirthDate).LessThan(DateOnly.FromDateTime(DateTime.Today)).WithErrorCode(AuthorErrorCodes.CreateAuthor.FutureDate);

//        RuleFor(b => b.Email)
//	        .Must(ISBNValidator.IsValid).WithErrorCode(AuthorErrorCodes.CreateBook.InvalidISBN);
    }
}
