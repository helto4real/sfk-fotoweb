using Microsoft.AspNetCore.Components.Forms;

namespace Foto.WebServer.Dto;

public record StBildInfo
{
    public Guid Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime Time { get; set; }
    public string Description { get; set; } = default!;
    public Guid ImageReference { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string AboutThePhotographer { get; set; } = default!;
    public bool IsUsed { get; set; } = default!;
    public bool IsAccepted { get; set; }
}

public record NewStBildInfo
{
    public string Title { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime Time { get; set; } = default;
    public string Description { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string AboutThePhotographer { get; set; } = default!;
    public  IBrowserFile? ImageFile { get; set; }
    public string ImageName { get; set; } = default!;
}

