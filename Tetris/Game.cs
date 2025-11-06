namespace Tetris;

public class Game
{
    public static int GameLogicUpdates = 0;
    public static int GameLogicUpdatesPerSecond = 0;
    public static bool[,] GameBoard = new bool[Program.CanvasWidth, Program.CanvasHeight];
    
    private const int MillisBetweenGravityMovements = 1000;
    private static readonly bool[][,] TetrinoLayouts =
    [
        new[,]
        {
            { false, true , false, false },
            { false, true , false, false },
            { false, true , false, false },
            { false, true , false, false }
        },
        // new[,]
        // {
        //     { true , false, false },
        //     { true , true , true  },
        //     { false, false, false }
        // }
    ];
    
    private static Tetromino _currentTetrino;
    private static bool _moveBlock = true;
    private static Lock _lock = new();

    public static void StartGameLoop()
    {
        new Thread(() =>
        {
            SpawnTetromino();
            using var timer = new Timer(_ => Gravity(), null, 1000, 1000);
            while (true)
            {
                ProcessUserInputs();
            }
        }).Start();
    }
    
    private static void SpawnTetromino()
    {
        _currentTetrino = new Tetromino(TetrinoLayouts[0]);
        AddCurrentTetrominoToGameBoard();
    }

    private static void ProcessUserInputs()
    {
        GameLogicUpdates++;
        
        // Check for user inputs
        if (Console.KeyAvailable)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.RightArrow:
                {
                    RemoveCurrentTetrominoFromGameBoard();
                    if (IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) => 
                            x + 1 < Program.CanvasWidth &&
                            !GameBoard[x + 1, y]))
                    {
                        _currentTetrino.X++;
                    }
                    AddCurrentTetrominoToGameBoard();
                    
                    break;
                }
                
                case ConsoleKey.LeftArrow:
                {
                    RemoveCurrentTetrominoFromGameBoard();
                    if (IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) =>
                            x - 1 > -1 && 
                            !GameBoard[x - 1, y]))
                    {
                        _currentTetrino.X--;
                    }
                    AddCurrentTetrominoToGameBoard();
                    
                    break;
                }
                
                case ConsoleKey.UpArrow:
                {
                    RemoveCurrentTetrominoFromGameBoard();
                    var rotationLayoutPreview = _currentTetrino.TetrominoLayout.PreviewLayoutAfterRotation();
                    if (IsValidPosition(rotationLayoutPreview.RelativeBlockLayout, (x, y) =>
                            x < Program.CanvasWidth &&
                            x > -1 &&
                            y < Program.CanvasHeight && 
                            !GameBoard[x, y]))
                    {
                        _currentTetrino.TetrominoLayout = rotationLayoutPreview;
                    }
                    AddCurrentTetrominoToGameBoard();
                    
                    break;
                }
            }
        }
    }

    private static void Gravity()
    {
        GameLogicUpdates++;

        RemoveCurrentTetrominoFromGameBoard();
        if (!IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) =>
                y + 1 < Program.CanvasHeight
                && !GameBoard[x, y + 1]))
        {
            AddCurrentTetrominoToGameBoard();
            SpawnTetromino();
        }
        else
        {
            _currentTetrino.Y++;
            AddCurrentTetrominoToGameBoard();
        }
    }
    
    private static bool IsValidPosition(List<int[]> relativeBlockLayout, Func<int, int, bool> canMove)
    {
        foreach (var relativeBlockPosition in relativeBlockLayout)
        {
            var absoluteXPosition = _currentTetrino.X + relativeBlockPosition[0];
            var absoluteYPosition = _currentTetrino.Y + relativeBlockPosition[1];

            if (!canMove(absoluteXPosition, absoluteYPosition))
            {
                return false;
            }
        }

        return true;
    }

    private static void RemoveCurrentTetrominoFromGameBoard()
    {
        _currentTetrino.RelativeBlockLayout
            .ForEach(relativeBlockPosition =>
            {
                GameBoard[_currentTetrino.X + relativeBlockPosition[0], _currentTetrino.Y + relativeBlockPosition[1]] = false;
            });
    }
    
    private static void AddCurrentTetrominoToGameBoard()
    {
        _currentTetrino.RelativeBlockLayout
            .ForEach(relativeBlockPosition =>
            {
                GameBoard[_currentTetrino.X + relativeBlockPosition[0], _currentTetrino.Y + relativeBlockPosition[1]] = true;
            });
    }
}