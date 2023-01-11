using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Services;
using BlockBreak.Model;
using BlockBreak.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlockBreak.GameStates;

public sealed class SettingsMenu: GameState
{
    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }
    private StarField StarField { get; }
    private Db Db { get; }

    private GameState PreviousState { get; }
    private Menu Menu { get; }

    public SettingsMenu(
        SettingsMenuConfig config, GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard,
        StarField starField, Db db
    )
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;
        StarField = starField;
        Db = db;

        PreviousState = config.PreviousState;

        Menu = new Menu()
        {
            Options = new List<string>()
            {
                "No Zoom",
                "2x Zoom",
                "3x Zoom",
                "4x Zoom",
                "5x Zoom",
                "Return",
            }
        };

        Menu.Cursor = Graphics.Zoom - 1;
    }

    public override void ActiveInput(GameTime gameTime)
    {
        Menu.ActiveInput(Keyboard);

        if(Keyboard.PressedAnyKey(new[] { Keys.Space, Keys.Enter }))
        {
            switch(Menu.Cursor)
            {
                case >= 0 and < 5:
                    Graphics.SetZoom(Menu.Cursor + 1);
                    Db.Settings.First().ZoomLevel = Menu.Cursor + 1;
                    Db.SaveChanges();
                    break;

                case 5:
                    GSM.ChangeState(PreviousState);
                    break;
            }
        }
    }

    public override void ActiveUpdate(GameTime gameTime)
    {
        StarField.Update(gameTime);
    }

    public override void AlwaysUpdate(GameTime gameTime)
    {
    }

    public override void ActiveDraw(GameTime gameTime)
    {
    }

    public override void AlwaysDraw(GameTime gameTime)
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

public sealed record SettingsMenuConfig(GameState PreviousState);