namespace BlockBreak.Model;

public sealed record Block(int BlockType)
{
    public const int Width = 24;
    public const int Height = 8;

    public int Points => BlockType switch
    {
        0 => 1000,
        1 => 500,
        2 => 300,
        3 => 200,
        4 => 100,
        _ => 50,
    };
}