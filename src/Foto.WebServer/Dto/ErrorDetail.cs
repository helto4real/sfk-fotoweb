using FluentValidation.Results;

namespace Foto.WebServer.Dto;

public class ErrorDetail
{
    public string Title { get; set; } = default!;
    public int StatusCode { get; init; }
    public string Detail { get; init; } = default!;
    public IEnumerable<ValidationFailure>? Errors { get; set; }
}