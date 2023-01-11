using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Services;
using BlockBreak.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlockBreak.GameStates;

public sealed class PauseMenu: GameState
{
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }

    private GameState PreviousState { get; }
    private Menu Menu { get; }

    public PauseMenu(PauseMenuConfig config, GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;

        PreviousState = config.PreviousState;

        Menu = new Menu()
        {
            Options = new()
            {
                "Resume",
                "Settings",
                "Quit"
            }
        };
    }

    public override void ActiveInput(GameTime gameTime)
    {
        Menu.ActiveInput(Keyboard);

        if(Keyboard.PressedAnyKey(new[] { Keys.Space, Keys.Enter }))
        {
            switch(Menu.Cursor)
            {
                case 0:
                    GSM.ChangeState(PreviousState);
                    break;

                case 1:
                    GSM.ChangeState<SettingsMenu, SettingsMenuConfig>(new(this));
                    break;

                case 2:
                    GSM.ChangeState<TitleMenu>();
                    break;
            }
        }
    }

    public override void AlwaysUpdate(GameTime gameTime)
    {
        PreviousState.AlwaysUpdate((gameTime));
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
        PreviousState.AlwaysDraw(gameTime);

        Graphics.DrawMenu(Menu, gameTime, Graphics.Height / 2, "Font");
    }
}

public sealed record PauseMenuConfig(GameState PreviousState);