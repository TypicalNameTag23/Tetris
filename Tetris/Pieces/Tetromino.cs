namespace Tetris.Pieces;

public class Tetromino(bool[,] layoutMatrix)
{
    private int[] _coords = [0, 0];
    
    public TetrominoLayout TetrominoLayout { get; set; } = new(layoutMatrix);
    
    /// <summary>
    /// This <see cref="Tetromino"/>'s layout matrix's upper right X coord
    /// </summary>
    public int X
    {
        get => _coords[0];
        set => _coords[0] = value;
    }

    /// <summary>
    /// This <see cref="Tetromino"/>'s layout matrix's upper right Y coord
    /// </summary>
    public int Y
    {
        get => _coords[1];
        set => _coords[1] = value;
    }

    public List<int[]> RelativeBlockLayout => TetrominoLayout.RelativeBlockLayout;
}