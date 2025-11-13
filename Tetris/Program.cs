using Painter;
using Tetris.UI;

namespace Tetris;

static class Program
{
    public static void Main()
    {
        Application.Drawable = new TetrisGameView();
        // Application.TargetFramerate = 120;
        Application.Start();
    }
    
}