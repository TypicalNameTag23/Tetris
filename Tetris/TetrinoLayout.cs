namespace Tetris;

public class TetrinoLayout(Coordinate[] relativeLayout, int width, int height)
{
    public Coordinate[] RelativeLayout { get; } = relativeLayout;

    public int Width { get; } = width;

    public int Height { get; } = height;
}