using FluentValidation.Results;

namespace Foto.WebServer.Dto;

public class ErrorDetail
{
    public string Title { get; set; } = default!;
    public int StatusCode { get; set; }
    public string Detail { get; set; } = default!;
    public IEnumerable<ValidationFailure>? Errors { get; set; }
}