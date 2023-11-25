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

    public override void Input(GameTime gameTime)
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
                    GSM.ChangeState<SettingsMenu, SettingsMenuConfig>(new SettingsMenuConfig(this));
                    break;

                case 2:
                    GSM.ChangeState<TitleMenu>();
                    break;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        PreviousState.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        PreviousState.Draw(gameTime);

        Graphics.DrawMenu(Menu, gameTime, Graphics.Height / 2, "Font");
    }
}

public sealed record PauseMenuConfig(GameState PreviousState);