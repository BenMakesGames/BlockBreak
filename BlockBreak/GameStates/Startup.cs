using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Services;
using Microsoft.Xna.Framework;

namespace BlockBreak.GameStates;

// sealed classes execute faster than non-sealed, so always seal your game states!
public sealed class Startup: GameState
{
    private GraphicsManager Graphics { get; }
    private GameStateManager GSM { get; }

    public Startup(GraphicsManager graphics, GameStateManager gsm)
    {
        Graphics = graphics;
        GSM = gsm;
    }
    
    public override void ActiveUpdate(GameTime gameTime)
    {
        if (Graphics.FullyLoaded)
        {
            GSM.ChangeState<Playing>();
        }
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
    }
}