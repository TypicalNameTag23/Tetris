using Painter;
using Painter.Components;
using Painter.Drawing;
using Painter.Exceptions;
using Tetris.Game;

namespace Tetris.UI;

public class TetrisGameView : IDrawable
{
    private const char EmptySpaceCharacter = '.';
    private const string GameWindowSizeTooSmallMessage = "Game window is too small.";
    
    private static int _frameCount;
    private static int _framesPerSecond;
    private TetrisGame _tetrisGame;

    public TetrisGameView()
    {
        _tetrisGame = new TetrisGame();
        new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(1000);
                _framesPerSecond = _frameCount;
                _frameCount = 0;

                _tetrisGame.GameLogicUpdatesPerSecond = _tetrisGame.GameLogicUpdates;
                _tetrisGame.GameLogicUpdates = 0;
            }
        }).Start();
        
        _tetrisGame.Start();
    }
    
    //todo: clean up this code
    public void DrawOnto(Canvas canvas)
    {
        var y = 0;
        if (canvas.Width <= _tetrisGame.BoardWidth * 2 + 4 || canvas.Height <= _tetrisGame.BoardHeight + 2)
        {
            DrawText(GameWindowSizeTooSmallMessage, canvas, y);
        }
        else
        {
            var xOffset = canvas.Width / 2 - _tetrisGame.BoardWidth;
            var yOffset = canvas.Height / 2 - _tetrisGame.BoardHeight / 2;
            for (; y < _tetrisGame.BoardHeight; y++)
            {
                for (var x = 0; x < _tetrisGame.BoardWidth * 2 - 1; x += 2)
                {
                    if (x == 0)
                    {
                        canvas.Draw(new Point(x + xOffset - 1, y + yOffset), new Pixel('>')
                        {
                            ForegroundColor = ConsoleColor.Green
                        }); 
                        canvas.Draw(new Point(x + xOffset - 2, y + yOffset), new Pixel('#')
                        {
                            ForegroundColor = ConsoleColor.Green
                        }); 
                    }
                    
                    if (x == _tetrisGame.BoardWidth * 2 - 2)
                    {
                        canvas.Draw(new Point(x + xOffset + 2, y + yOffset), new Pixel('<')
                        {
                            ForegroundColor = ConsoleColor.Green
                        }); 
                        canvas.Draw(new Point(x + xOffset + 3, y + yOffset), new Pixel('#')
                        {
                            ForegroundColor = ConsoleColor.Green
                        }); 
                    }
                    
                    if (_tetrisGame.GameBoard[x / 2, y])
                    {
                        canvas.Draw(new Point(x + xOffset, y + yOffset), new Pixel('[')
                        {
                            ForegroundColor = ConsoleColor.DarkGreen
                        });
                        canvas.Draw(new Point(x + xOffset + 1, y + yOffset), new Pixel(']')
                        {
                            ForegroundColor = ConsoleColor.DarkGreen
                        });
                        
                        continue;
                    }

                    canvas.Draw(new Point(x + xOffset, y + yOffset), new Pixel(EmptySpaceCharacter)
                    {
                        ForegroundColor = ConsoleColor.DarkGray
                    });
                }
            }
            y--;
        }


        DrawText($"FPS: {_framesPerSecond} | GLUPS: {_tetrisGame.GameLogicUpdatesPerSecond}", canvas, canvas.Height - 2);
        DrawText($"W: {Console.BufferWidth} | H: {Console.BufferHeight}", canvas, canvas.Height - 1);
        _frameCount++;
    }

    private void DrawText(string text, Canvas canvas, int y = 0, ConsoleColor? textColor = null)
    {
        try // hack workaround to stop drawing outside of canvas
        {
            for (var x = 0; x < text.Length; x++)
            {
                canvas.Draw(new Point(x, y), new Pixel(text[x])
                {
                    ForegroundColor = textColor
                });
            }
        }
        catch (PainterException) { /* ignored */ }
    }
}