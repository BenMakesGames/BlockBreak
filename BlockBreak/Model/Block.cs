namespace BlockBreak.Model;

public sealed record Block(int BlockType)
{
    public const int Width = 24;
    public const int Height = 8;
}