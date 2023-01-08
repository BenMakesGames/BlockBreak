using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini.GraphicsExtensions;
using BenMakesGames.PlayPlayMini.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlockBreak.Model;

public sealed class Menu
{
    public required List<string> Options { get; set; }
    public int Cursor { get; set; }

    public int LongestOptionLength => Options.Max(o => o.Length);

    public void ActiveInput(KeyboardManager keyboard)
    {
        if(keyboard.PressedAnyKey(new[] { Keys.W, Keys.Up, Keys.NumPad8 }))
            Cursor = (Cursor - 1 + Options.Count) % Options.Count;
        
        if(keyboard.PressedAnyKey(new[] { Keys.S, Keys.Down, Keys.NumPad2 }))
            Cursor = (Cursor + 1) % Options.Count;
    }
}

public static class MenuExtensions
{
    public static void DrawMenu(this GraphicsManager graphics, Menu menu, GameTime gameTime, int y, string fontName)
    {
        var x = (graphics.Width - graphics.Fonts[fontName].CharacterWidth * menu.LongestOptionLength) / 2;
        var verticalSpacing = graphics.Fonts[fontName].CharacterHeight + 4;
        
        for (var i = 0; i < menu.Options.Count; i++)
        {
            var option = menu.Options[i];
            var isSelected = menu.Cursor == i;
            
            if(isSelected)
                graphics.DrawWavyText(fontName, x, y + i * verticalSpacing, gameTime, option, DawnBringers16.Yellow);
            else
                graphics.DrawText(fontName, x, y + i * verticalSpacing, option, DawnBringers16.White);
        }
        
        graphics.DrawPicture("MenuCursor", x - (int)(10 + Math.Cos(gameTime.TotalGameTime.TotalSeconds * 10) * 1.5), y - 1 + menu.Cursor * verticalSpacing);
    }
}