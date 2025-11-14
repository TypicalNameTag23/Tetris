using System.Diagnostics;
using System.Drawing;
using Painter;
using Painter.Drawing;
using Painter.Elements;
using Painter.Layouts;
using Tetris.Game;

namespace Tetris.UI;

public class StatisticsView : Element
{
    private TextBlock _gameInfoTextBlock = new()
    {
        TextColor = Color.IndianRed,
        TextAlignment = TextAlignment.Left,
        WrapText = false,
        Width = null,
        Height = 1,
        Y = 0
    };
    
    private TextBlock _windowInfoTextBlock = new()
    {
        TextColor = Color.IndianRed,
        TextAlignment = TextAlignment.Left,
        WrapText = false,
        Width = null,
        Height = 1,
        Y = 1
    };

    private Panel _infoPanel = new();
    private int _frames;
    private int _gameLogicUpdates;
    private long _renderTime;
    private Stopwatch _renderStopwatch;

    public StatisticsView(TetrisGame tetrisGame)
    {
        _infoPanel.Parent = this;
        _infoPanel.Add(_windowInfoTextBlock);
        _infoPanel.Add(_gameInfoTextBlock);
        Application.BeforeFrameRender += () =>
        {
            _renderStopwatch = Stopwatch.StartNew();
        };
        
        Application.AfterFrameRender += () =>
        {
            _frames += 1;
            _renderStopwatch!.Stop();
            _renderTime = _renderStopwatch.ElapsedMilliseconds;
        };

        tetrisGame.OnGameUpdateEvent += () => _gameLogicUpdates++;
        new Thread(_ =>
        {
            while (true)
            {
                Redraw();
                _gameInfoTextBlock.Text = $"FPS: {_frames} | RT: {_renderTime}ms | GLUPS: {_gameLogicUpdates}";
                _frames = 0;
                _gameLogicUpdates = 0;
                Thread.Sleep(1000);
            }
        }).Start();
        
        new Thread(_ =>
        {
            while (true)
            {
                Redraw();
                _windowInfoTextBlock.Text = $"W: {Console.BufferWidth} | H: {Console.BufferHeight}";
            }
        }).Start();
    }
    
    protected override void DrawElement(Canvas canvas, bool force)
    {
        _infoPanel.Draw(canvas, force);
    }
}