namespace Tetris;

//todo: prevent blocks from moving into one another when shifting right or left
//todo: table that has current positions of blocks

class Program
{
    public static readonly char BlockCharacter = '+';
    public static readonly char EmptySpaceCharacter = '\0';
    public static readonly int CanvasWidth = 10;
    public static readonly int CanvasHeight = 20;
    
    public static int TerminalWidth = Console.BufferWidth;
    public static int TerminalHeight = Console.BufferHeight;
    
    public static int FrameCount = 0;
    public static int FramesPerSecond = 0;

    public static Game Game; 

    private static char[,] _renderBuffer = new char[CanvasWidth, CanvasHeight];
    
    static void Main()
    {
        Setup();
        while (true)
        {
            Render();
            Thread.Sleep(16);
        }
    }

    private static void Setup()
    {
        Console.CursorVisible = false;
        Console.Clear();
        
        Game = new Game();
        new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(1000);
                FramesPerSecond = FrameCount;
                FrameCount = 0;
                
                Game.GameLogicUpdatesPerSecond = Game.GameLogicUpdates;
                Game.GameLogicUpdates = 0;
            }
        }).Start();

        Game.Start();
    }
    
    private static void Render()
    {
        FrameCount++;

        if (Console.BufferWidth != TerminalWidth || Console.BufferHeight != TerminalHeight)
        {
            TerminalWidth = Console.BufferWidth;
            TerminalHeight = Console.BufferHeight;
            Console.Clear();
        }
        
        Console.SetCursorPosition(0, 0);
        if (Console.BufferWidth <= CanvasWidth || Console.BufferHeight <= CanvasHeight + 2)
        {
            Console.WriteLine("Window too small.");
        }
        else
        {
            for (var y = 0; y < Game.GameBoard.GetLength(1); y++)
            {
                for (var x = 0; x < Game.GameBoard.GetLength(0); x++)
                {
                    if (Game.GameBoard[x, y])
                    {
                        Console.Write(BlockCharacter);
                        continue;
                    }
                
                    Console.Write('.');
                }

                Console.WriteLine();
            }
        }

        PrintAndClear($"FPS: {FramesPerSecond} | GLUPS: {Game.GameLogicUpdatesPerSecond}");
        Console.WriteLine($"W: {Console.BufferWidth} | H: {Console.BufferHeight}");
    }

    public static void PrintAndClear(string s)
    {
        var padding = TerminalWidth - s.Length;
        Console.WriteLine(padding > 0 ? s.PadRight(padding, ' ') : s);
    }
}
