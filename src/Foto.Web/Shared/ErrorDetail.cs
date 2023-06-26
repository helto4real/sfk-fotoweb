using FluentValidation.Results;

namespace Todo.Web.Shared;

public class ErrorDetail
{
    public string Title { get; set; } = default!;
    public int StatusCode { get; set; }
    public string Detail { get; set; } = default!;
    public IEnumerable<ValidationFailure>? Errors { get; set; }
}