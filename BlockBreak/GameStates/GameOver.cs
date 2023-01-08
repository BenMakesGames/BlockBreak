using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.GraphicsExtensions;
using BenMakesGames.PlayPlayMini.Services;
using Microsoft.Xna.Framework;

namespace BlockBreak.GameStates;

public sealed class GameOver: GameState
{
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }

    private GameState PreviousState { get; }
    private int Score { get; }

    public GameOver(GameOverConfig config, GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;

        PreviousState = config.PreviousState;
        Score = config.Score;
    }

    public override void ActiveInput(GameTime gameTime)
    {
        if (Keyboard.PressedAnyKey())
        {
            // TODO: if you got a high score, go to the high score screen; else, go to "sorry, no high score" screen
            GSM.ChangeState<TitleMenu>();
        }
    }

    public override void ActiveUpdate(GameTime gameTime)
    {
    }

    public override void AlwaysUpdate(GameTime gameTime)
    {
        PreviousState.AlwaysUpdate(gameTime);
    }

    public override void ActiveDraw(GameTime gameTime)
    {
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
        PreviousState.AlwaysDraw(gameTime);

        Graphics.DrawPicture("GameOver", (Graphics.Width - Graphics.Pictures["GameOver"].Width) / 2, Graphics.Height / 3 - Graphics.Pictures["GameOver"].Height);

        var text = "Press any key to continue...";

        Graphics.DrawWavyText("Font", gameTime, text, DawnBringers16.White);
    }
}

public sealed record GameOverConfig(GameState PreviousState, int Score);