namespace BlockBreak.Model;

public sealed class Ball
{
    public const int Radius = 6;

    public double X { get; set; }
    public double Y { get; set; }
    public int PixelX => (int) X;
    public int PixelY => (int) Y;

    public BallState State { get; set; } = BallState.StuckToPaddle;

    public double SpeedX { get; set; }
    public double SpeedY { get; set; }

    public double Speed { get; set; } = 1;
}

public enum BallState
{
    Moving,
    StuckToPaddle
}