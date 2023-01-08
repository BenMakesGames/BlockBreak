using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini.Attributes.DI;
using BenMakesGames.PlayPlayMini.Services;
using Microsoft.Xna.Framework;

namespace BlockBreak.Services;

[AutoRegister(Lifetime.PerDependency)]
public class StarField
{
    private const int InnerRadius = 20;
    private const int Speed = 10;

    private GraphicsManager Graphics { get; }
    private Random RNG { get; }

    private Star[] Stars { get; }

    private static readonly Color[] StarColors = {
        DawnBringers16.DarkPurple,
        DawnBringers16.DarkGray,
        DawnBringers16.Gray,
    };

    public StarField(Random rng, GraphicsManager graphics)
    {
        RNG = rng;
        Graphics = graphics;
        Stars = new Star[100];

        for (int i = 0; i < 100; i++)
        {
            double angle = RNG.NextDouble() * Math.PI * 2;
            double distance = InnerRadius + RNG.NextDouble() * (Math.Min(Graphics.Width, Graphics.Height) / 2.0 - InnerRadius);
            Stars[i] = new()
            {
                Angle = angle,
                Speed = RNG.Next(3),
                X = Graphics.Width / 2.0 + Math.Cos(angle) * distance,
                Y = Graphics.Height / 2.0 + Math.Sin(angle) * distance,
            };
        }

        // roughly play out 20 seconds, to get stars into a more evenly-distributed state
        for(int i = 0; i < 40; i++)
            Update(new() { ElapsedGameTime = TimeSpan.FromSeconds(0.5) });
    }

    public void Update(GameTime gameTime)
    {
        foreach (var star in Stars)
        {
            star.X += Math.Cos(star.Angle) * (star.Speed + 1) * gameTime.ElapsedGameTime.TotalSeconds * Speed;
            star.Y += Math.Sin(star.Angle) * (star.Speed + 1) * gameTime.ElapsedGameTime.TotalSeconds * Speed;

            if (star.X < 0 || star.X >= Graphics.Width || star.Y < 0 || star.Y >= Graphics.Height)
            {
                star.Angle = RNG.NextDouble() * Math.PI * 2;
                star.X = Graphics.Width / 2.0 + Math.Cos(star.Angle) * InnerRadius;
                star.Y = Graphics.Height / 2.0 + Math.Sin(star.Angle) * InnerRadius;
            }
        }
    }

    public void Draw()
    {
        foreach (var star in Stars)
            Graphics.DrawPoint(star.PixelX, star.PixelY, StarColors[star.Speed]);
    }
}

public class Star
{
    public required double X { get; set; }
    public required double Y { get; set; }
    public required double Angle { get; set; }
    public int PixelX => (int) X;
    public int PixelY => (int) Y;
    public required int Speed { get; set; }
}