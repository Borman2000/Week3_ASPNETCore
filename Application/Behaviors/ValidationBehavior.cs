using FluentValidation;
using MediatR;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
		var context = new ValidationContext<TRequest>(request);

		var validationFailures = await Task.WhenAll(
			validators.Select(validator => validator.ValidateAsync(context)));

		var errors = validationFailures
			.Where(validationResult => !validationResult.IsValid)
			.SelectMany(validationResult => validationResult.Errors)
			.ToList();

		if (errors.Any())
			throw new ValidationException(errors);

		return await next();
	}
}
