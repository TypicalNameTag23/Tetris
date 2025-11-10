using Painter;
using Painter.Components;
using Tetris.UI;

namespace Tetris;

static class Program
{
    public static void Main()
    {
        var window = new Window
        {
            Drawable = new TetrisGameView()
        };

        Application.Window = window;
        Application.TargetFps = 10;
        Application.Start();
    }
    
}