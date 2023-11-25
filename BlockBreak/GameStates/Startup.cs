using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.GraphicsExtensions;
using BenMakesGames.PlayPlayMini.Services;
using BlockBreak.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Xna.Framework;

namespace BlockBreak.GameStates;

public sealed class Startup: GameState
{
    private GraphicsManager Graphics { get; }
    private SoundManager Sounds { get; }
    private GameStateManager GSM { get; }
    private Db Db { get; }

    private Task StartupTask { get; }

    public Startup(GraphicsManager graphics, SoundManager sounds, GameStateManager gsm, Db db)
    {
        Graphics = graphics;
        Sounds = sounds;
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
    
    public override void Update(GameTime gameTime)
    {
        if (Graphics.FullyLoaded && Sounds.FullyLoaded && StartupTask.IsCompleted)
        {
            Graphics.SetZoom(Db.Settings.First().ZoomLevel);
            GSM.ChangeState<TitleMenu>();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Graphics.DrawWavyText("Font", gameTime, "Loading...");
    }
}