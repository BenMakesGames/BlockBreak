using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Services;
using BenMakesGames.RandomHelpers;
using BlockBreak.Model;
using BlockBreak.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlockBreak.GameStates;

public sealed class Playing: GameState
{
    private const int BlockColumns = 16;
    private const int BlockRows = 6;
    private const int BlockYOffset = 40;
    private const int PaddleYOffset = 20;

    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }
    private Random RNG { get; }
    private StarField StarField { get; }

    private int Lives { get; set; } = 3;
    private int Score { get; set; }
    private Ball Ball { get; } = new();
    private Paddle Paddle { get; } = new();
    private Block?[,] Blocks { get; } = new Block[BlockColumns, BlockRows];

    public Playing(GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard, Random rng, StarField starField)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;
        RNG = rng;
        StarField = starField;

        ResetBoard();
    }

    private void ResetBoard()
    {
        Paddle.X = Graphics.Width / 2.0 - Paddle.Width / 2.0;
        Paddle.Y = Graphics.Height - PaddleYOffset;

        ResetBall();

        for (int x = 0; x < BlockColumns; x++)
        {
            for (int y = 0; y < BlockRows; y++)
                Blocks[x, y] = new Block(y);
        }
    }

    private void ResetBall()
    {
        Ball.X = Paddle.X + Paddle.Width / 2.0;
        Ball.Y = Paddle.Y - Ball.Radius;
        Ball.Speed = 1;

        Ball.State = BallState.StuckToPaddle;
    }

    public override void ActiveInput(GameTime gameTime)
    {
        Paddle.SpeedX = 0;

        if (Keyboard.AnyKeyDown(new[] { Keys.A, Keys.Left, Keys.NumPad4 }))
            Paddle.SpeedX--;

        if (Keyboard.AnyKeyDown(new[] { Keys.D, Keys.Right, Keys.NumPad6 }))
            Paddle.SpeedX++;

        if (Keyboard.KeyDown(Keys.Space) && Ball.State == BallState.StuckToPaddle)
        {
            Ball.State = BallState.Moving;

            Ball.SpeedX = 0;
            Ball.SpeedY = -Ball.Speed;
        }
    }

    public override void ActiveUpdate(GameTime gameTime)
    {
        // if the game is lagging this hard, bail (the player is probably dragging the screen around, or something)
        if (gameTime.ElapsedGameTime.TotalSeconds >= 0.1)
            return;

        MovePaddle(gameTime);
        MoveBall(gameTime);
    }

    private void MovePaddle(GameTime gameTime)
    {
        var oldX = Paddle.X;

        Paddle.X = Math.Clamp(
            Paddle.X + Paddle.SpeedX * gameTime.ElapsedGameTime.TotalSeconds * 200,
            0,
            Graphics.Width - Paddle.Width
        );

        if (Ball.State == BallState.StuckToPaddle)
            Ball.X += Paddle.X - oldX;
    }

    private void MoveBall(GameTime gameTime)
    {
        if (Ball.State != BallState.Moving)
            return;

        var newY = Ball.Y + Ball.SpeedY * gameTime.ElapsedGameTime.TotalSeconds * 200;

        if (Ball.PixelY != (int) newY)
        {
            // if it hit the top of the screen, bounce
            if ((int) newY < Ball.Radius)
            {
                Ball.SpeedY *= -1;
                newY += Ball.Radius - newY;
            }

            // if it went past the bottom of the screen, lose a life
            else if ((int) newY >= Graphics.Height + Ball.Radius)
            {
                if (Lives > 0)
                {
                    ResetBall();
                    Lives--;
                    return;
                }
                else
                    GSM.ChangeState<GameOver, GameOverConfig>(new(this, Score));
            }

            // if it hit the paddle, bounce
            else if (Ball.Y + Ball.Radius < Paddle.Y && newY + Ball.Radius >= Paddle.Y)
            {
                if (Ball.X + Ball.Radius >= Paddle.X && Ball.X - Ball.Radius < Paddle.X + Paddle.Width)
                {
                    Ball.SpeedY *= -1;
                    newY -= newY - (Paddle.PixelY - Ball.Radius);
                    Ball.SpeedX = (Ball.X - (Paddle.X + Paddle.Width / 2.0)) / 10.0;
                    Ball.Speed += 0.05;

                    NormalizeBallSpeed();
                }
            }

            for(int x = 0; x < BlockColumns; x++)
            {
                for (int y = 0; y < BlockRows; y++)
                {
                    var block = Blocks[x, y];

                    if (block == null)
                        continue;

                    var (blockX, blockY) = BlockCoordinates(x, y);

                    if (Ball.X + Ball.Radius >= blockX && Ball.X - Ball.Radius < blockX + Block.Width)
                    {
                        // hit from above
                        if(Ball.Y + Ball.Radius < blockY && newY + Ball.Radius >= blockY)
                        {
                            Ball.SpeedY *= -1;
                            newY -= newY - (blockY - Ball.Radius);
                            Score += block.Points;
                            Blocks[x, y] = null;
                        }
                        // hit from below
                        else if(Ball.Y - Ball.Radius > blockY + Block.Height && newY - Ball.Radius <= blockY + Block.Height)
                        {
                            Ball.SpeedY *= -1;
                            newY += blockY + Block.Height + Ball.Radius - newY;
                            Score += block.Points;
                            Blocks[x, y] = null;
                        }
                    }
                }
            }
        }

        Ball.Y = newY;

        var newX = Ball.X + Ball.SpeedX * gameTime.ElapsedGameTime.TotalSeconds * 200;

        if (Ball.PixelX != (int) newX)
        {
            // if hit the left side of the screen, bounce
            if ((int)newX < Ball.Radius)
            {
                Ball.SpeedX *= -1;
                newX += Ball.Radius - newX;
            }

            // if hit the right side of the screen, bounce
            else if ((int) newX > Graphics.Width - Ball.Radius - 1)
            {
                Ball.SpeedX *= -1;
                newX -= newX - (Graphics.Width - Ball.Radius - 1);
            }

            // if it hit the paddle, bounce
            else if (Ball.Y + Ball.Radius >= Paddle.Y && Ball.Y - Ball.Radius < Paddle.Y + Paddle.Height)
            {
                // hit on the left
                if (Ball.X + Ball.Radius < Paddle.X && newX + Ball.Radius >= Paddle.X)
                {
                    Ball.SpeedX *= -1;
                    newX -= newX - (Paddle.X - Ball.Radius);
                }
                // hit on the right
                else if(Ball.X - Ball.Radius >= Paddle.X + Paddle.Width && newX - Ball.Radius < Paddle.X + Paddle.Width)
                {
                    Ball.SpeedX *= -1;
                    newX += Paddle.X + Paddle.Width + Ball.Radius - newX;
                }
            }
        }

        Ball.X = newX;
    }

    private void NormalizeBallSpeed()
    {
        var speed = Math.Sqrt(Ball.SpeedX * Ball.SpeedX + Ball.SpeedY * Ball.SpeedY);

        Ball.SpeedX = Ball.SpeedX / speed * Ball.Speed;
        Ball.SpeedY = Ball.SpeedY / speed * Ball.Speed;
    }

    public override void AlwaysUpdate(GameTime gameTime)
    {
        StarField.Update(gameTime);
    }

    public override void ActiveDraw(GameTime gameTime)
    {
    }

    public override void AlwaysDraw(GameTime gameTime)
    {
        DrawBackground();
        DrawScore();
        DrawBlocks();
        DrawPaddle();
        DrawBall();
    }

    private void DrawBackground()
    {
        Graphics.Clear(DawnBringers16.Black);
        StarField.Draw();
    }

    private void DrawScore()
    {
        Graphics.DrawText("Font", 2, Graphics.Height - 10, $"Score: {Score}", DawnBringers16.White);

        for (int i = 1; i <= Lives; i++)
            Graphics.DrawPicture("Life", Graphics.Width - 10 * i, Graphics.Height - 10);
    }

    private static (int, int) BlockCoordinates(int column, int row)
        => (column * Block.Width, row * Block.Height + BlockYOffset);

    private void DrawBlocks()
    {
        for (int x = 0; x < BlockColumns; x++)
        {
            for (int y = 0; y < BlockRows; y++)
            {
                if (Blocks[x, y] is Block block)
                {
                    var (pixelX, pixelY) = BlockCoordinates(x, y);
                    Graphics.DrawSprite("Blocks", pixelX, pixelY, block.BlockType);
                }
            }
        }
    }

    private void DrawPaddle()
    {
        Graphics.DrawPicture("Paddle", Paddle.PixelX, Paddle.PixelY);
    }

    private void DrawBall()
    {
        Graphics.DrawPicture("Ball", Ball.PixelX - Ball.Radius, Ball.PixelY - Ball.Radius);
    }
}