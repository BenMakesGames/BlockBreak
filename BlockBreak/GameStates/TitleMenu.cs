using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Services;
using BlockBreak.Model;
using BlockBreak.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlockBreak.GameStates;

public sealed class TitleMenu: GameState
{
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }
    private StarField StarField { get; }

    private Menu Menu { get; }

    public TitleMenu(GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard, StarField starField)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;
        StarField = starField;

        Menu = new Menu()
        {
            Options = new List<string>()
            {
                "New Game",
                "High Scores",
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
                case 0: GSM.ChangeState<Playing>(); break;
                case 1: GSM.ChangeState<HighScoreTable>(); break;
                case 2: GSM.ChangeState<SettingsMenu, SettingsMenuConfig>(new SettingsMenuConfig(this)); break;
                case 3: GSM.Exit(); break;
            }
        }
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
            0
        );
        
        Graphics.DrawMenu(Menu, gameTime, 100, "Font");
    }
}