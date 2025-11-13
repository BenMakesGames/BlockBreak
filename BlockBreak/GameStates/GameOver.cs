using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.GraphicsExtensions;
using BenMakesGames.PlayPlayMini.Services;
using BlockBreak.Model.DbEntities;
using BlockBreak.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlockBreak.GameStates;

public sealed class GameOver: GameState<GameOverConfig>
{
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }
    private Db Db { get; }

    private AbstractGameState PreviousState { get; }
    private int Score { get; }
    private int NumberOfHigherScores { get; }
    private bool GotAHighScore => NumberOfHigherScores < 10;

    private string Initials { get; set; } = "";

    public GameOver(GameOverConfig config, GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard, Db db)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;
        Db = db;

        PreviousState = config.PreviousState;
        Score = config.Score;

        NumberOfHigherScores = Db.HighScoreEntries.Count(h => h.Score >= Score);
    }

    public override void Input(GameTime gameTime)
    {
        if (GotAHighScore)
        {
            DoNameInput();
        }
        else if (Keyboard.PressedAnyKey(new[] { Keys.Space, Keys.Enter }))
        {
            GSM.ChangeState<HighScoreTable>();
        }
    }

    private void DoNameInput()
    {
        if (Initials.Length == 3)
        {
            if (Keyboard.PressedKey(Keys.Enter))
            {
                var entryCount = Db.HighScoreEntries.Count();

                // we're about to add an entry; if there's more than 9 entries, then we need to delete the lowest one
                if(entryCount > 9)
                {
                    // if we've done everything right in the past, there should only be ONE entry to delete...
                    // ... but maybe we haven't done everything right in the past :P
                    var lowestScores = Db.HighScoreEntries.OrderByDescending(h => h.Score).Take(entryCount - 9);
                    Db.HighScoreEntries.RemoveRange(lowestScores);
                }

                Db.HighScoreEntries.Add(new HighScoreEntry
                {
                    Name = Initials,
                    Score = Score
                });

                Db.SaveChanges();

                GSM.ChangeState<HighScoreTable>();
            }
        }

        if (Initials.Length < 3)
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if(Keyboard.PressedKey(c - 'A' + Keys.A))
                    Initials += c;
            }
        }

        if (Initials.Length > 0)
        {
            if (Keyboard.PressedKey(Keys.Back))
                Initials = Initials[..^1];
        }
    }

    public override void Update(GameTime gameTime)
    {
        PreviousState.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        PreviousState.Draw(gameTime);

        Graphics.DrawSprite(
            "BigWords",
            (Graphics.Width - Graphics.SpriteSheets["BigWords"].SpriteWidth) / 2,
            Graphics.Height / 3 - Graphics.SpriteSheets["BigWords"].SpriteHeight,
            1
        );

        var score = $"Score: {Score:N0}";
        Graphics.DrawText("Font", Graphics.Width / 2 - score.Length * 3, Graphics.Height / 2 - 20, score, DawnBringers16.White);

        if (GotAHighScore)
        {
            const string text = "Congratulations! You got a high score!";
            Graphics.DrawText("Font", Graphics.Width / 2 - text.Length * 3, Graphics.Height / 2 - 10, text, DawnBringers16.White);

            DrawNameInput(gameTime);
        }
        else
        {
            const string text = "Not enough for a high score; sorry!";
            Graphics.DrawText("Font", Graphics.Width / 2 - text.Length * 3, Graphics.Height / 2 - 10, text, DawnBringers16.White);

            Graphics.DrawWavyText("Font", Graphics.Height / 2 + 10, gameTime, "Press SPACE to continue...", DawnBringers16.LightGray);
        }
    }

    private void DrawNameInput(GameTime gameTime)
    {
        var x = Graphics.Width / 2 - 15;
        var y = Graphics.Height / 2 + 10;

        for (int i = 0; i < 3; i++)
        {
            if (i < Initials.Length)
            {
                var color = ((int)(gameTime.TotalGameTime.TotalSeconds * 10 + i) % 4) switch
                {
                    0 => DawnBringers16.White,
                    1 => DawnBringers16.Orange,
                    2 => DawnBringers16.White,
                    _ => DawnBringers16.Yellow,
                };

                Graphics.DrawText("Font", x + i * 12, y, Initials[i], color);
            }
            else if (i == Initials.Length)
            {
                if((int)(gameTime.TotalGameTime.TotalSeconds * 4) % 2 == 0)
                    Graphics.DrawFilledRectangle(x + i * 12, y, 6, 8, DawnBringers16.White);
            }

            Graphics.DrawText("Font", x + i * 12, y + 2, "_", DawnBringers16.LightGray);
        }
    }
}

public sealed record GameOverConfig(AbstractGameState PreviousState, int Score);