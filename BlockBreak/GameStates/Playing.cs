using BenMakesGames.MonoGame.Palettes;
using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.GraphicsExtensions;
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

    public int GameSpeed = 200;

    private GraphicsManager Graphics { get; }
    private KeyboardManager Keyboard { get; }
    private GameStateManager GSM { get; }
    private Random RNG { get; }
    private StarField StarField { get; }
    private SoundManager Sounds { get; }

    private int Lives { get; set; } = 3;
    private int Score { get; set; }
    private Ball Ball { get; } = new();
    private Paddle Paddle { get; } = new();
    private Block?[,] Blocks { get; } = new Block[BlockColumns, BlockRows];

    public Playing(GraphicsManager graphics, GameStateManager gsm, KeyboardManager keyboard, Random rng, StarField starField, SoundManager sounds)
    {
        Graphics = graphics;
        GSM = gsm;
        Keyboard = keyboard;
        RNG = rng;
        StarField = starField;
        Sounds = sounds;

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
        Ball.Speed = Ball.InitialSpeed;

        Ball.State = BallState.StuckToPaddle;
    }

    public override void ActiveInput(GameTime gameTime)
    {
        Paddle.SpeedX = 0;

        if (Keyboard.AnyKeyDown(new[] { Keys.A, Keys.Left, Keys.NumPad4 }))
            Paddle.SpeedX--;

        if (Keyboard.AnyKeyDown(new[] { Keys.D, Keys.Right, Keys.NumPad6 }))
            Paddle.SpeedX++;

        if (Keyboard.PressedKey(Keys.Space) && Ball.State == BallState.StuckToPaddle)
        {
            Ball.State = BallState.Moving;

            Ball.SpeedX = (RNG.NextDouble() - 0.5) / 10;
            Ball.SpeedY = -Ball.Speed;
            
            NormalizeBallSpeed();
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
            Paddle.X + Paddle.SpeedX * gameTime.ElapsedGameTime.TotalSeconds * GameSpeed,
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

        // up/down movement
        var newY = Ball.Y + Ball.SpeedY * gameTime.ElapsedGameTime.TotalSeconds * GameSpeed;

        if (Ball.PixelY != (int) newY)
        {
            // if it hit the top of the screen, bounce
            if ((int) newY < Ball.Radius)
            {
                Ball.SpeedY *= -1;
                newY += Ball.Radius - newY;
                PlayBounce(0);
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
                    
                    PlayBounce(-0.2f);
                }
            }

            bool movingUp = newY < Ball.Y;

            var oldRow = ((int)Ball.Y + (movingUp ? -Ball.Radius : Ball.Radius) - BlockYOffset) / Block.Height;
            var newRow = ((int)newY + (movingUp ? -Ball.Radius : Ball.Radius) - BlockYOffset) / Block.Height;

            if(newRow != oldRow && newRow is >= 0 and < BlockRows)
            {
                var hitAnything = false;
                var (_, blockY) = BlockCoordinates(0, newRow);
                
                for(int x = 0; x < BlockColumns; x++)
                {
                    var block = Blocks[x, newRow];

                    if(block == null)
                        continue;

                    var (blockX, _) = BlockCoordinates(x, newRow);

                    if(Ball.X + Ball.Radius >= blockX && Ball.X - Ball.Radius < blockX + Block.Width)
                    {
                        hitAnything = true;

                        Score += block.Points;
                        Blocks[x, newRow] = null;
                    }
                }

                if(hitAnything)
                {
                    Ball.SpeedY *= -1;
                    
                    if(movingUp)
                        newY += blockY + Block.Height - (newY - Ball.Radius);
                    else
                        newY -= newY + Ball.Radius - blockY;

                    PlayBounce(newRow * 0.2f);
                }
            }
        }

        Ball.Y = newY;

        // left/right movement
        var newX = Ball.X + Ball.SpeedX * gameTime.ElapsedGameTime.TotalSeconds * GameSpeed;

        if (Ball.PixelX != (int) newX)
        {
            // if hit the left side of the screen, bounce
            if ((int)newX < Ball.Radius)
            {
                Ball.SpeedX *= -1;
                newX += Ball.Radius - newX;
                PlayBounce(0);
            }

            // if hit the right side of the screen, bounce
            else if ((int) newX > Graphics.Width - Ball.Radius - 1)
            {
                Ball.SpeedX *= -1;
                newX -= newX - (Graphics.Width - Ball.Radius - 1);
                PlayBounce(0);
            }

            // if it hit the paddle, bounce
            else if (Ball.Y + Ball.Radius >= Paddle.Y && Ball.Y - Ball.Radius < Paddle.Y + Paddle.Height)
            {
                // hit on the left
                if (Ball.X + Ball.Radius < Paddle.X && newX + Ball.Radius >= Paddle.X)
                {
                    Ball.SpeedX = -Math.Abs(Ball.SpeedX);
                    newX -= newX - (Paddle.X - Ball.Radius);
                    PlayBounce(-0.2f);
                }
                // hit on the right
                else if(Ball.X - Ball.Radius >= Paddle.X + Paddle.Width && newX - Ball.Radius < Paddle.X + Paddle.Width)
                {
                    Ball.SpeedX = Math.Abs(Ball.SpeedX);
                    newX += Paddle.X + Paddle.Width - (newX - Ball.Radius);
                    PlayBounce(-0.2f);
                }
            }
            
            bool movingLeft = newX < Ball.X;

            var oldColumn = ((int)Ball.X + (movingLeft ? -Ball.Radius : Ball.Radius)) / Block.Width;
            var newColumn = ((int)newX + (movingLeft ? -Ball.Radius : Ball.Radius)) / Block.Width;

            if(newColumn != oldColumn && newColumn is >= 0 and < BlockColumns)
            {
                var hitAnything = false;
                var (blockX, _) = BlockCoordinates(newColumn, 0);
                var highestRow = 0;
                
                for(int y = 0; y < BlockRows; y++)
                {
                    var block = Blocks[newColumn, y];

                    if(block == null)
                        continue;

                    var (_, blockY) = BlockCoordinates(newColumn, y);

                    if(Ball.Y + Ball.Radius >= blockY && Ball.Y - Ball.Radius < blockY + Block.Height)
                    {
                        hitAnything = true;
                        highestRow = Math.Max(highestRow, y);

                        Score += block.Points;
                        Blocks[newColumn, y] = null;
                    }
                }

                if(hitAnything)
                {
                    Ball.SpeedX *= -1;
                    
                    if(movingLeft)
                        newX += blockX + Block.Width - (newX - Ball.Radius);
                    else
                        newX -= newX + Ball.Radius - blockX;
                    
                    PlayBounce(0.2f * highestRow);
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
        if(Ball.State == BallState.StuckToPaddle)
            Graphics.DrawWavyText("Font", gameTime, "Press SPACE to launch...", DawnBringers16.LightGray);
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

    public void PlayBounce(float pitch)
    {
        Sounds.PlaySound("Bounce", pitch: pitch);        
    }
}