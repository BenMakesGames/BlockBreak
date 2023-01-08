namespace BlockBreak.Model.DbEntities;

public sealed class HighScoreEntry: IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset? CreatedOn { get; set; }

    public required string Name { get; set; }
    public required long Score { get; set; }
}