using Application.DTOs;
using MediatR;

namespace Application.Authors.Create;

public record CreateAuthorCommand(string FirstName, string LastName, DateOnly BirthDate, string? Email, string? Biography) : IRequest<AuthorDto?>;