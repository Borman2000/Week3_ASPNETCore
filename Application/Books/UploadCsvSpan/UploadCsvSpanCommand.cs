using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Books.UploadCsvSpan;

public record UploadCsvSpanCommand(IFormFile file) : IRequest<Task>;