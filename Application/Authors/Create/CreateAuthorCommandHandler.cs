using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Authors.Create;

public class CreateAuthorCommandHandler(IAuthorRepository authorRepository, IMapper dtoMapper) : IRequestHandler<CreateAuthorCommand, AuthorDto?>
{
	public async Task<AuthorDto?> Handle(CreateAuthorCommand command, CancellationToken cancellationToken)
	{
		var author = await authorRepository.AddAsync(new Author(command.FirstName, command.LastName, command.BirthDate, command.Email, command.Biography));
		return dtoMapper.Map<AuthorDto>(author);
	}
}