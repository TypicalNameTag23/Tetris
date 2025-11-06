namespace Tetris;

public class Tetromino(bool[,] layoutMatrix)
{
    private int[] _coords = [0, 0];
    
    public bool[,] LayoutMatrix { get; } = layoutMatrix;
    
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

    public int Width => LayoutMatrix.GetLength(0);
    
    public int Height => LayoutMatrix.GetLength(1);

    // public int LeftPosition => X;
    //
    // public int RightPosition => X + TetrinoLayout.Width;
    //
    // public int BottomPosition => Y + TetrinoLayout.Height;

    /// <summary>
    /// Rotates this <see cref="Tetromino"/>'s layout matrix by 90 degrees to the right
    /// </summary>
    public void Rotate()
    {
        var layers = LayoutMatrix.GetLength(0) / 2;
        for (var layer = 0; layer < layers; layer++)
        {
            var startIndex = layer;
            var endIndex = LayoutMatrix.GetLength(0) - layer - 1;

            var upperLeft = LayoutMatrix[startIndex, startIndex];
            var upperRight = LayoutMatrix[startIndex, endIndex];
            var bottomLeft = LayoutMatrix[endIndex, startIndex];
            var bottomRight = LayoutMatrix[endIndex, endIndex];
            
            LayoutMatrix[startIndex, startIndex] = bottomLeft;
            LayoutMatrix[startIndex, endIndex] = upperLeft;
            LayoutMatrix[endIndex, endIndex] = upperRight;
            LayoutMatrix[endIndex, startIndex] = bottomRight;
            for (var inner = 1; inner < endIndex - startIndex; inner++)
            {
                var top = LayoutMatrix[startIndex, startIndex + inner];
                var right = LayoutMatrix[startIndex + inner, endIndex];
                var bottom = LayoutMatrix[endIndex, endIndex - inner];
                var left = LayoutMatrix[endIndex - inner, startIndex];

                LayoutMatrix[startIndex, startIndex + inner] = left;
                LayoutMatrix[startIndex + inner, endIndex] = top;
                LayoutMatrix[endIndex, endIndex - inner] = right;
                LayoutMatrix[endIndex - inner, startIndex] = bottom;
            }
        }
    }
}