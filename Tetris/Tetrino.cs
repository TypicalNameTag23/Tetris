namespace Tetris;

public class Tetrino(TetrinoLayout tetrinoLayout)
{
    private Coordinate _coords = new(0, 0);
    
    public TetrinoLayout TetrinoLayout { get; } = tetrinoLayout;

    public int X
    {
        get => _coords.RelativeX;
        set => _coords.RelativeX = value;
    }

    public int Y
    {
        get => _coords.RelativeY;
        set => _coords.RelativeY = value;
    }

    public int LeftPosition => X;
    
    public int RightPosition => X + TetrinoLayout.Width;
    
    public int BottomPosition => Y + TetrinoLayout.Height;
}