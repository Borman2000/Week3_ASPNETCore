using FluentValidation;

namespace Application.Books.Update;

internal sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
	    RuleFor(b => b.Title).NotEmpty().Unless(b => b.Price > 0)
		    .WithErrorCode(BookErrorCodes.UpdateBook.EmptyValues).WithMessage("Both Title and Price cannot be empty.");
    }
}
