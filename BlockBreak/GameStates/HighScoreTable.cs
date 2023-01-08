using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Services;
using Microsoft.Xna.Framework;

namespace BlockBreak.GameStates;

public sealed class HighScoreTable: GameState
{
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }

    public HighScoreTable(GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;
    }

    public override void ActiveInput(GameTime gameTime)
    {
    }

    public override void ActiveUpdate(GameTime gameTime)
    {
    }

    public override void AlwaysUpdate(GameTime gameTime)
    {
    }

    public override void ActiveDraw(GameTime gameTime)
    {
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
    }
}