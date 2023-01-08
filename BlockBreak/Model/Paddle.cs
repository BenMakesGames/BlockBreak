namespace BlockBreak.Model;

public sealed class Paddle
{
    public double X { get; set; }
    public double Y { get; set; }
    public int PixelX => (int) X;
    public int PixelY => (int) Y;

    public double SpeedX { get; set; }

    public const int Width = 32;
    public const int Height = 8;
}