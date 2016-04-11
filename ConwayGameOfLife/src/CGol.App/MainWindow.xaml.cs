using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Cgol.Core;
using Timer = System.Timers.Timer;

namespace CGol.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly GameMatrix _gameMatrix;
        private Rectangle[,] _rectangles;
        private readonly SolidColorBrush _colorOff;
        private readonly SolidColorBrush _colorOn;
        private Timer _timer;
        private ProcessRules _rules;

        public MainWindow()
        {
            InitializeComponent();
            _gameMatrix = new GameMatrix(40, 40);
            _rules = ProcessRules.DefaultRules();
            _timer = new Timer
                         {
                             Interval = TimeSpan.FromSeconds(.5).TotalMilliseconds,
                         };
            _timer.Elapsed += UpdateBoard;
            _gameMatrix.Activate(_gameMatrix.RandomPoints().Take((_gameMatrix.Height * _gameMatrix.Width)/2).ToArray());
            
            Loaded += OnLoaded;

            const byte i = 241;
            _colorOff = new SolidColorBrush(new Color { A = 255, R = i, B = i, G = i });
            _colorOn = new SolidColorBrush(Colors.Gray);
        }

        private void UpdateBoard(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var processAllRules = _rules.ProcessAllRules(_gameMatrix);
            stopwatch.Stop();
            Console.Out.WriteLine("Time " + stopwatch.ElapsedMilliseconds);
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<IEnumerable<Cell>>(OnAction), processAllRules);
        }

        private void OnAction(IEnumerable<Cell> enumerable)
        {
            Populate(enumerable);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            BuildGameBoard();
            Populate(_gameMatrix.Cells);
            _timer.Start();
        }

        private void Populate(IEnumerable<Cell> enumerable)
        {  
            foreach (var cell in enumerable)
            {
                _rectangles[cell.Point.X , cell.Point.Y].Fill = cell.IsOn ? _colorOn : _colorOff;
            }
        }

        private void BuildGameBoard()
        {
            _rectangles = new Rectangle[_gameMatrix.Width, _gameMatrix.Height];
            var width = GameBoard.ActualWidth / _gameMatrix.Width;
            var height = GameBoard.ActualHeight / _gameMatrix.Height;

            for (int x = 0; x < _gameMatrix.Width; x++)
            {
                
                GameBoard.ColumnDefinitions.Add(new ColumnDefinition() );
                for (int y = 0; y < _gameMatrix.Height; y++)
                {
                    if (x == 0)
                    {
                        GameBoard.RowDefinitions.Add(new RowDefinition());
                    }

                    var rectangle = new Rectangle { Margin = new Thickness(1) ,Fill = _colorOff };
                    Grid.SetColumn(rectangle, x);
                    Grid.SetRow(rectangle, y);
                    GameBoard.Children.Add(rectangle);
                    _rectangles[x, y] = rectangle;
                }
            }
        }
    }

}
