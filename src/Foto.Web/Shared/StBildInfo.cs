namespace Todo.Web.Shared;

public record StBildInfo
{
    public Guid Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime Time { get; set; } = default;
    public string Description { get; set; } = default!;
    public Guid ImageReference { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string AboutThePhotograper { get; set; } = default!;
    public bool IsUsed { get; set; } = default!;
    public bool IsAccepted { get; set; }
}
