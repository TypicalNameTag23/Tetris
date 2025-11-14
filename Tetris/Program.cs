using Painter;
using Painter.Layouts;
using Tetris.Game;
using Tetris.UI;

namespace Tetris;

static class Program
{
    public static void Main()
    {
        var tetrisGame = new TetrisGame();
        var panel = new Panel();
        panel.Add(new StatisticsView(tetrisGame));
        panel.Add(new TetrisGameView(tetrisGame));
        var application = new Application
        {
            Drawable = panel
        };
        
        tetrisGame.Start();
        application.Start();
    }
    
}