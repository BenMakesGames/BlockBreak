namespace BlockBreak.Model.DbEntities;

public sealed class Settings: IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required int ZoomLevel { get; set; }
}