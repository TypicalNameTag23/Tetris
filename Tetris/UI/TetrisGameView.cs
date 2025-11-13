using System.Drawing;
using Painter.Drawing;
using Painter.Widgets;
using Tetris.Game;

namespace Tetris.UI;

public class TetrisGameView : Widget
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
    public override void Draw(Canvas canvas)
    {
        var y = 0;
        if (canvas.Width <= _tetrisGame.BoardWidth * 2 + 4 || canvas.Height <= _tetrisGame.BoardHeight + 2)
        {
            canvas.Draw(0, y, GameWindowSizeTooSmallMessage);
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
                        canvas.Draw(x + xOffset - 1, y + yOffset, ">", Color.DarkGreen); 
                        canvas.Draw(x + xOffset - 2, y + yOffset, "#", Color.DarkGreen);
                    }
                    
                    if (x == _tetrisGame.BoardWidth * 2 - 2)
                    {
                        canvas.Draw(x + xOffset + 2, y + yOffset, "<", Color.DarkGreen);
                        canvas.Draw(x + xOffset + 3, y + yOffset, "#", Color.DarkGreen);
                    }
                    
                    if (_tetrisGame.GameBoard[x / 2, y])
                    {
                        canvas.Draw(x + xOffset, y + yOffset, "[", Color.DarkGreen);
                        canvas.Draw(x + xOffset + 1, y + yOffset, "]", Color.DarkGreen);
                        continue;
                    }

                    
                    canvas.Draw(x + xOffset, y + yOffset, ".", Color.DarkGray);
                    canvas.Draw(x + xOffset + 1, y + yOffset, " ", Color.DarkGray);
                }
            }
        }
        
        canvas.Draw(0, canvas.Height - 2, $"FPS: {_framesPerSecond} | GLUPS: {_tetrisGame.GameLogicUpdatesPerSecond}");
        canvas.Draw(0, canvas.Height - 1, $"W: {Console.BufferWidth} | H: {Console.BufferHeight}");
        _frameCount++;
    }
    
}