namespace Tetris.Pieces;

public class TetrominoLayout
{
    private List<int[]> _relativeBlockLayout = [];
    private bool[,] _layoutMatrix;
    private int? initialOffet;

    public int LayoutWidth => _layoutMatrix.GetLength(0);
    
    public int LayoutHeight => _layoutMatrix.GetLength(1);

    public TetrominoLayout(bool[,] layoutMatrix)
    {
        _layoutMatrix = layoutMatrix;
        UpdateRelativeLayout();
    }

    public List<int[]> RelativeBlockLayout => _relativeBlockLayout;

    /// <summary>
    /// Rotates this <see cref="TetrominoLayout"/>'s layout matrix by 90 degrees to the right
    /// </summary>
    public void Rotate()
    {
        var layers = _layoutMatrix.GetLength(0) / 2;
        for (var layer = 0; layer < layers; layer++)
        {
            var startIndex = layer;
            var endIndex = _layoutMatrix.GetLength(0) - layer - 1;

            var upperLeft = _layoutMatrix[startIndex, startIndex];
            var upperRight = _layoutMatrix[startIndex, endIndex];
            var bottomLeft = _layoutMatrix[endIndex, startIndex];
            var bottomRight = _layoutMatrix[endIndex, endIndex];
            
            _layoutMatrix[startIndex, startIndex] = bottomLeft;
            _layoutMatrix[startIndex, endIndex] = upperLeft;
            _layoutMatrix[endIndex, endIndex] = upperRight;
            _layoutMatrix[endIndex, startIndex] = bottomRight;
            for (var inner = 1; inner < endIndex - startIndex; inner++)
            {
                var top = _layoutMatrix[startIndex, startIndex + inner];
                var right = _layoutMatrix[startIndex + inner, endIndex];
                var bottom = _layoutMatrix[endIndex, endIndex - inner];
                var left = _layoutMatrix[endIndex - inner, startIndex];

                _layoutMatrix[startIndex, startIndex + inner] = left;
                _layoutMatrix[startIndex + inner, endIndex] = top;
                _layoutMatrix[endIndex, endIndex - inner] = right;
                _layoutMatrix[endIndex - inner, startIndex] = bottom;
            }
        }

        UpdateRelativeLayout();
    }
    
    public TetrominoLayout PreviewLayoutAfterRotation()
    {
        var clone = Clone();
        clone.Rotate();

        return clone;
    }

    public TetrominoLayout Clone()
    {
        return new TetrominoLayout(
            (bool[,]) _layoutMatrix.Clone()
        );
    }

    private void UpdateRelativeLayout()
    {
        _relativeBlockLayout.Clear();

        var offset = true;
        for (var y = 0; y < LayoutHeight; y++)
        {
            for (var x = 0; x < LayoutWidth; x++)
            {
                if (_layoutMatrix[x, y])
                {
                    if (initialOffet is null) // hack
                    {
                        initialOffet = y;
                    }

                    if (y == 0)
                    {
                        offset = false;
                    }
                    
                    _relativeBlockLayout.Add([x, y - (offset ? (int) initialOffet : 0)]);
                }
            }
        }
    }
}