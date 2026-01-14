using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Books.UploadCsv;

public record UploadCsvCommand(IFormFile file) : IRequest<Task>;