namespace FotoApi.Model;

public abstract record TimeTrackedEntity
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }    
}