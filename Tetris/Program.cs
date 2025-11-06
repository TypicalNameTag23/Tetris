namespace Tetris;

//todo: prevent blocks from moving into one another when shifting right or left
//todo: table that has current positions of blocks
//todo: block rotations
//todo: load block layouts and rotations from file

class Program
{
    public static readonly char BlockCharacter = '+';
    public static readonly char EmptySpaceCharacter = '.';
    public static readonly int MillisBetweenGravityMovements = 1000;
    public static readonly int CanvasWidth = 20;
    public static readonly int CanvasHeight = 10;
    public static readonly TetrinoLayout[] TetrinoLayouts = [
        new([
            new(0, 0), 
            new(1, 0), 
            new(2, 0), 
            new(3, 0)
        ], 4, 1),
        new([
            new(0, 0), 
            new(1, 1), 
            new(2, 2)
        ], 3, 3),
        new([
            new(0, 0), 
            new(1, 0), 
            new(2, 0), 
            new(3, 0),
            new(3, 1)
        ], 4, 2),
        new([
            new(0, 0), 
            new(1, 0), 
            new(2, 0), 
            new(3, 0),
            new(3, 1)
        ], 4, 2),
    ];

    private static char[,] _renderBuffer = new char[CanvasWidth, CanvasHeight];
    private static int[] _columnTopOccupiedPosition = new int[CanvasWidth];
    private static List<Tetrino> _tetrinos = [];
    private static Tetrino? _currentTetrino;
    private static bool _moveBlock = true;
    
    static void Main()
    {
        Setup();
        while (true)
        {
            ProcessGameLogic();
            Render();
            Thread.Sleep(17);
        }
    }

    private static void Setup()
    {
        for (var i = 0; i < _columnTopOccupiedPosition.Length; i++)
        {
            _columnTopOccupiedPosition[i] = 0;
        }
        
        Console.CursorVisible = false;
        Console.Clear();
    }

    private static void ProcessGameLogic()
    {
        // Spawn a block if one doesn't already exist
        if (_currentTetrino is null)
        {
            var random = new Random();
            _currentTetrino = new Tetrino(TetrinoLayouts[random.NextInt64(0, 4)]);
            _tetrinos.Add(_currentTetrino);
            
            Task.Run(() =>
            {
                _moveBlock = false;
                Thread.Sleep(MillisBetweenGravityMovements);
                _moveBlock = true;
            });
        }

        // Check for user inputs
        if (Console.KeyAvailable)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.RightArrow:
                {
                    if (_currentTetrino.RightPosition == CanvasWidth)
                    {
                        break;
                    }

                    _currentTetrino.X++;
                    break;
                }
            }
        }
        
        // Gravity
        if (_moveBlock)
        {
            _currentTetrino.Y++;
            var canTetrinoContinueMoving = _currentTetrino.BottomPosition != CanvasHeight;
            if (canTetrinoContinueMoving)
            {
                foreach (var tetrinoPart in _currentTetrino.TetrinoLayout.RelativeLayout)
                {
                    if (_columnTopOccupiedPosition[_currentTetrino.X + tetrinoPart.RelativeX] == _currentTetrino.Y + tetrinoPart.RelativeY + 1)
                    {
                        canTetrinoContinueMoving = false;
                        break;
                    }
                }
            }
            
            if (!canTetrinoContinueMoving)
            {
                foreach (var tetrinoPart in _currentTetrino.TetrinoLayout.RelativeLayout)
                {
                    _columnTopOccupiedPosition[_currentTetrino.X + tetrinoPart.RelativeX] = _currentTetrino.Y + tetrinoPart.RelativeY;
                }
                
                _currentTetrino = null;
            }
            else
            {
                Task.Run(() =>
                {
                    _moveBlock = false;
                    Thread.Sleep(MillisBetweenGravityMovements);
                    _moveBlock = true;
                });
            }
        }
    }

    private static void Render()
    {
        foreach (var tetrino in _tetrinos)
        {
            foreach (var tetrinoPart in tetrino.TetrinoLayout.RelativeLayout)
            {
                _renderBuffer[tetrino.X + tetrinoPart.RelativeX, tetrino.Y + tetrinoPart.RelativeY] = '+';
            }
        }

        Console.SetCursorPosition(0, 0);
        for (var y = 0; y < _renderBuffer.GetLength(1); y++)
        {
            for (var x = 0; x < _renderBuffer.GetLength(0); x++)
            {
                if (_renderBuffer[x, y] == '\0')
                {
                    Console.Write(EmptySpaceCharacter);
                    continue;
                }
                
                Console.Write(_renderBuffer[x, y]);
            }

            Console.WriteLine();
        }
        Array.Clear(_renderBuffer);
    }
}