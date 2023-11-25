using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.GraphicsExtensions;
using BenMakesGames.PlayPlayMini.Services;
using BlockBreak.Model.DbEntities;
using BlockBreak.Services;
using Microsoft.Xna.Framework;

namespace BlockBreak.GameStates;

public sealed class HighScoreTable: GameState
{
    public const int HighScoresX = 120;
    public const int HighScoresY = 60;
    public const int HighScoreLineHeight = 12;
    
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }
    private Db Db { get; }
    private StarField StarField { get; }

    private List<HighScoreEntry> Top10 { get; }

    public HighScoreTable(GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard, Db db, StarField starField)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;
        Db = db;
        StarField = starField;

        Top10 = Db.HighScoreEntries.OrderByDescending(h => h.Score).ToList();
    }

    public override void Input(GameTime gameTime)
    {
        if(Keyboard.PressedAnyKey())
            GSM.ChangeState<TitleMenu>();
    }

    public override void Update(GameTime gameTime)
    {
        StarField.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Graphics.Clear(DawnBringers16.Black);
        StarField.Draw();
        
        Graphics.DrawSprite(
            "BigWords",
            (Graphics.Width - Graphics.SpriteSheets["BigWords"].SpriteWidth) / 2,
            20,
            2
        );

        for(int i = 0; i < Top10.Count; i++)
        {
            var color = i switch
            {
                0 => DawnBringers16.Yellow,
                1 => DawnBringers16.LightGray,
                >= 2 and < 5 => DawnBringers16.Gray,
                _ => DawnBringers16.DarkGray
            };
            
            Graphics.DrawText("Font", HighScoresX, HighScoresY + (i * HighScoreLineHeight), $"{i + 1,2}", color);
            Graphics.DrawText("Font", HighScoresX + 20, HighScoresY + (i * HighScoreLineHeight), Top10[i].Name, color);
            Graphics.DrawText("Font", HighScoresX + 90, HighScoresY + (i * HighScoreLineHeight), $"{Top10[i].Score,9:N0}", color);
        }
        
        Graphics.DrawWavyText("Font", Graphics.Height - 20, gameTime, "Press any key to return to the main menu", DawnBringers16.White);
    }
}