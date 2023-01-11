using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.GraphicsExtensions;
using BenMakesGames.PlayPlayMini.Services;
using BlockBreak.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xna.Framework;

namespace BlockBreak.GameStates;

// sealed classes execute faster than non-sealed, so always seal your game states!
public sealed class Startup: GameState
{
    private GraphicsManager Graphics { get; }
    private GameStateManager GSM { get; }
    private Db Db { get; }

    private Task StartupTask { get; }

    public Startup(GraphicsManager graphics, GameStateManager gsm, Db db)
    {
        Graphics = graphics;
        GSM = gsm;
        Db = db;

        StartupTask = Task.Run(() =>
        {
            Db.Database.Migrate();

            if (Db.Settings.FirstOrDefault() == null)
            {
                Db.Settings.Add(new() { ZoomLevel = 3 });
                Db.SaveChanges();
            }
        });
    }
    
    public override void ActiveUpdate(GameTime gameTime)
    {
        if (Graphics.FullyLoaded && StartupTask.IsCompleted)
        {
            Graphics.SetZoom(Db.Settings.First().ZoomLevel);
            GSM.ChangeState<TitleMenu>();
        }
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
        Graphics.DrawWavyText("Font", gameTime, "Loading...");
    }
}