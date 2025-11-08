namespace Tetris.Game;

public class Game
{
    private const int MillisBetweenGravityMovements = 1000;
    private static readonly bool[][,] TetrinoLayouts =
    [
        new[,]
        {     // X Coords
/*Y Coords*/{ false, true , false, false },
            { false, true , false, false },
            { false, true , false, false },
            { false, true , false, false }
        },
        new[,]
        {
            { true , true, false },
            { false, true, false },
            { false, true, false }
        },
        new[,]
        {
            { false, true, false },
            { false, true, false },
            { true , true, false }
        },
        new[,]
        {
            { true, true},
            { true, true}
        },
        new[,]
        {
            { false, true , false },
            { true , true , false },
            { true , false, false }
        },
        new[,]
        {
            { false, true , false },
            { true , true , false },
            { false, true , false }
        },
        new[,]
        {
            { true , false, false },
            { true , true , false },
            { false, true , false }
        }
    ];

    private readonly bool[,] _gameBoard = new bool[Program.CanvasWidth, Program.CanvasHeight];
    private GameState _gameState = GameState.Inactive;
    private Pieces.Tetromino _currentTetrino;
    private Timer _gravityTimer;
    private Random _random;

    public Game(int? seed = null)
    {
        if (seed is not null)
        {
            _random = new Random((int) seed);
        }
        else
        {
            _random = new Random();
        }
    }
    
    public bool[,] GameBoard => _gameBoard;
    
    public int GameLogicUpdates { get; set; }

    public int GameLogicUpdatesPerSecond { get; set; }
    
    /// <summary>
    /// Starts the game if it isn't currently active.
    /// </summary>
    public void Start()
    {
        if (_gameState == GameState.Playing)
        {
            return;
        }
        
        _gameState = GameState.Playing;
        SpawnTetromino();
        new Thread(() =>
        {
            _gravityTimer = new Timer(
                _ => Gravity(), 
                null, 
                MillisBetweenGravityMovements, 
                MillisBetweenGravityMovements);
            
            while (_gameState == GameState.Playing)
            {
                ProcessUserInputs();
            }
        }).Start();
    }

    /// <summary>
    /// Processes user movement inputs, counts as a game logic update
    /// </summary>
    private void ProcessUserInputs()
    {
        GameLogicUpdates++;
        if (Console.KeyAvailable)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.RightArrow:
                {
                    RemoveCurrentTetrominoFromGameBoard(); // maybe don't remove the tetromino from the board if we never change it?
                    if (IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) => 
                            x + 1 < Program.CanvasWidth &&
                            !_gameBoard[x + 1, y]))
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
                            !_gameBoard[x - 1, y]))
                    {
                        _currentTetrino.X--;
                    }
                    
                    AddCurrentTetrominoToGameBoard();
                    break;
                }

                case ConsoleKey.DownArrow:
                {
                    RemoveCurrentTetrominoFromGameBoard();
                    if (IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) =>
                            y + 1 < Program.CanvasHeight && 
                            !_gameBoard[x, y + 1]))
                    {
                        _currentTetrino.Y++;
                    }
                    
                    AddCurrentTetrominoToGameBoard();
                    break;
                }
                case ConsoleKey.Spacebar:
                {
                    RemoveCurrentTetrominoFromGameBoard();
                    while (IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) =>
                               y + 1 < Program.CanvasHeight &&
                               !_gameBoard[x, y + 1]))
                    {
                        _currentTetrino.Y++;
                    }

                    AddCurrentTetrominoToGameBoard();
                    break;
                }
                case ConsoleKey.UpArrow:
                {
                    RemoveCurrentTetrominoFromGameBoard();
                    var rotationLayoutPreview = _currentTetrino.TetrominoLayout.PreviewLayoutAfterRotation();
                    if (IsValidPosition(rotationLayoutPreview.RelativeBlockLayout, (x, y) =>
                            x < Program.CanvasWidth  &&
                            x > -1                   &&
                            y < Program.CanvasHeight && 
                            y > -1                   && 
                            !_gameBoard[x, y]))
                    {
                        _currentTetrino.TetrominoLayout = rotationLayoutPreview;
                    }
                    
                    AddCurrentTetrominoToGameBoard();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Processes the current tetromino's gravity updates and checks if it is no longer able to move.
    /// </summary>
    private void Gravity()
    {
        GameLogicUpdates++;
        RemoveCurrentTetrominoFromGameBoard();
        if (!IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) =>
                y + 1 < Program.CanvasHeight
                && !_gameBoard[x, y + 1]))
        {
            AddCurrentTetrominoToGameBoard();
            ClearFilledLines();
            SpawnTetromino();
        }
        else
        {
            _currentTetrino.Y++;
            AddCurrentTetrominoToGameBoard();
        }
    }

    /// <summary>
    /// Clears all lines filled by the current tetrino
    /// </summary>
    private void ClearFilledLines()
    {
        var checkedYCoords = new HashSet<int>();
        foreach (var relativeBlockPosition in _currentTetrino.RelativeBlockLayout)
        {
            var y = _currentTetrino.Y + relativeBlockPosition[1];
            if (!checkedYCoords.Add(y))
            {
                continue;
            }

            var clearRow = true;
            for (var x = 0; x < Program.CanvasWidth; x++)
            {
                if (!_gameBoard[x, y])
                {
                    clearRow = false;
                    break;
                }
            }

            if (clearRow)
            {
                for (; y > -1; y--)
                {
                    for (var x = 0; x < Program.CanvasWidth; x++)
                    {
                        if (y != 0)
                        {
                            _gameBoard[x, y] = _gameBoard[x, y - 1];
                        }
                        else
                        {
                            _gameBoard[x, y] = false;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tries to spawn a tetromino and ends the game if it can't find a valid space
    /// </summary>
    private void SpawnTetromino()
    {
        _currentTetrino = new Pieces.Tetromino(TetrinoLayouts[_random.NextInt64(TetrinoLayouts.Length)]);
        _currentTetrino.X = _gameBoard.GetLength(0) / 2 - _currentTetrino.TetrominoLayout.LayoutWidth / 2;
        if (!IsValidPosition(_currentTetrino.RelativeBlockLayout, (x, y) => !_gameBoard[x, y]))
        {
            _gameState = GameState.GameOver;
            _gravityTimer.Dispose();
            return;
        }

        AddCurrentTetrominoToGameBoard();
    }

    /// <summary>
    /// Check all blocks within a layout against the current Tetromino's position to see if it satisfies the canMove func.
    /// </summary>
    /// <param name="relativeBlockLayout">The relative block layout to check</param>
    /// <param name="canMove">A function that returns whether the position is valid</param>
    /// <returns>True if the block layout satisfies the canMove func, otherwise false</returns>
    private bool IsValidPosition(List<int[]> relativeBlockLayout, Func<int, int, bool> canMove)
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

    private void RemoveCurrentTetrominoFromGameBoard()
    {
        _currentTetrino.RelativeBlockLayout
            .ForEach(relativeBlockPosition =>
            {
                _gameBoard[_currentTetrino.X + relativeBlockPosition[0], _currentTetrino.Y + relativeBlockPosition[1]] = false;
            });
    }
    
    private void AddCurrentTetrominoToGameBoard()
    {
        _currentTetrino.RelativeBlockLayout
            .ForEach(relativeBlockPosition =>
            {
                _gameBoard[_currentTetrino.X + relativeBlockPosition[0], _currentTetrino.Y + relativeBlockPosition[1]] = true;
            });
    }
}