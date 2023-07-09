using System.ComponentModel.DataAnnotations;

namespace FotoApi.Model;

public record Image : TimeTrackedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerReference { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string LocalFilePath { get; set; } = default!;
}