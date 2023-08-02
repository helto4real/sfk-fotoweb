namespace FotoApi.Model;

public record StBild : TimeTrackedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ImageReference { get; set; }
    public string OwnerReference { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime Time { get; set; }
    public string Description { get; set; } = default!;
    public string AboutThePhotographer { get; set; } = default!;
    public bool IsUsed { get; set; }
    public bool IsAccepted { get; set; }
}

