using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Lines.Design;
using static Lines.Helper;
using static Lines.CellHelper;
using static Lines.Settings;

namespace Lines
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SquarizeField();
            AddCells(FieldSideCellCount);
            //SetupNeighbours();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SpawnFewRandomBalls(SpawnBallQtyMin, SpawnBallQtyMax);
        }

        public void SquarizeField()
        {
            double width = MainField.ActualWidth;
            double height = MainField.ActualHeight;
            double min = Math.Min(width, height);

            MainField.Width = min;
            MainField.Height = min;
        }

        public void AddCells(int sideCellCount)
        {
            for (int i = 0; i < sideCellCount; i++)
            {
                for (int j = 0; j < sideCellCount; j++)
                {
                    var cell = CreateCell();
                    var border = CreateBorder();
                    border.Child = cell;

                    MainField.Children.Add(border);
                }
            }
        }

        private Cell CreateCell()
        {
            var cell = new Cell();

            cell.MouseEnter += Cell_MouseEnter;
            cell.PreviewMouseLeftButtonDown += Cell_PreviewMouseLeftButtonDown;
            cell.PreviewMouseLeftButtonUp += Cell_PreviewMouseLeftButtonUp;
            cell.MouseLeave += Cell_MouseLeave;
            cell.NotifyStateChanged += Cell_NotifyStateChanged;

            cell.Name = CellPrefix + (GetAllCells().Count() + 1).ToString();

            return cell;
        }

        private Border CreateBorder()
        {
            var border = new Border()
            {
                BorderThickness = new Thickness(CellBorderThinknessLength),
                BorderBrush = new SolidColorBrush(CellBorderColor),
                SnapsToDevicePixels = true,
            };

            return border;
        }

        private void SetupNeighbours()
        {
            foreach (var cell in GetAllCells())
            {
                int index = GetCellNameIndex(cell.Name);
                if (index == 0) { continue; }

                var neighbourIndices = GetCellNeighbourIndices(index);
                var neighbourNames = ConvertIndicesToNames(neighbourIndices);
                cell.AddNeighbours(neighbourNames);
            }
        }

        private void SpawnFewRandomBalls(int min, int max)
        {
            var cellNames = (from cell in GetAllCells()
                             where !cell.HasBall
                             select cell.Name).ToList();

            if (!cellNames.Any()) { return; }

            var result = GetBallSpawnQty(min, max);
            for (int i = 0; i < result; i++)
            {
                var name = FetchRandomItem(cellNames);
                SpawnBall(name);
            }
        }

        private IEnumerable<Cell> GetAllCells()
        {
            var cells = from border in MainField.Children.OfType<Border>()
                        select border.Child as Cell;

            return cells;
        }

        private IEnumerable<Border> GetAllBorders()
        {
            var borders = from border in MainField.Children.OfType<Border>()
                        select border as Border;

            return borders;
        }

        private void SpawnBall(string cellName)
        {
            var cell = (from c in GetAllCells()
                        where c.Name == cellName
                        select c).SingleOrDefault();

            if (cell == null || cell.HasBall) { return; }

            var ball = CreateBall();

            double ballWidth = cell.ActualWidth * BallSizePercentage;
            double ballHeight = ballWidth;

            double margin = GetBallDefaultMargin(cell.ActualWidth);
            ball.Margin = new Thickness(margin);
            ball.Width = ballWidth;
            ball.Height = ballHeight;

            cell.Children.Add(ball);
        }

        private double GetBallDefaultMargin(double cellWidth)
        {
            return (cellWidth - cellWidth * BallSizePercentage) / 2;
        }

        private Ellipse CreateBall()
        {
            var ball = new Ellipse()
            {
                Fill = new SolidColorBrush(Colors.AliceBlue),
                Stroke = new SolidColorBrush(Colors.Black),
                IsHitTestVisible = false,
            };

            return ball;
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            var cell = sender as Cell;
            if (cell.CellState == CellState.Selected)
            {
                return;
            }

            ResetHoverCells();

            cell.CellState = CellState.Hover;






            /*cell.Background = new SolidColorBrush(CellColorHover);

            foreach (var c in GetAllCells())
            {
                if (c.CellState == CellState.Hover)
                {
                    c.CellState = CellState.Idle;
                }
            }

            cell.CellState = CellState.Hover;*/
        }

        private void Cell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as Cell;
            cell.Background = new SolidColorBrush(CellColorClick);
        }

        private void Cell_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as Cell;
            cell.Background = new SolidColorBrush(CellColorHover);

            if (!cell.HasBall)
            {
                cell.CellState = CellState.Hover;

                return;
            }

            var ball = cell.Children[0] as Ellipse;

            if (cell.CellState == CellState.Selected)
            {
                cell.CellState = CellState.Hover;
                ball.BounceStop();

                return;
            }

            ResetAllCells();

            ball.BounceStart();
            cell.CellState = CellState.Selected;







            //ball.Margin = new Thickness(40);

            /*foreach (var b in GetAllBorders())
            {
                Panel.SetZIndex(b, zIndexDefault);
            }*/

            //Panel.SetZIndex(cell.Parent as Border, zIndexTop);
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            var cell = sender as Cell;

            switch (cell.CellState)
            {
                case CellState.Idle:
                    break;
                case CellState.Hover:
                    cell.CellState = CellState.Idle;
                    break;
                case CellState.Path:
                    break;
                case CellState.Pressed:
                    break;
                case CellState.Selected:
                    break;
                default:
                    break;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Close(); }
        }

        private void ResetCellsStatus()
        {
            foreach (var c in GetAllCells())
            {
                c.CellState = CellState.Idle;
            }
        }

        private void Cell_NotifyStateChanged(object sender)
        {
            UpdateCellsDisplay();
        }

        private bool DoesSelectedCellExist()
        {
            foreach (var c in GetAllCells())
            {
                if (c.CellState == CellState.Selected)
                {
                    return true;
                }
            }

            return false;
        }

        private void ResetHoverCells()
        {
            foreach (var c in GetAllCells())
            {
                if (c.CellState == CellState.Hover)
                {
                    c.CellState = CellState.Idle;
                }
            }
        }

        private void ResetSelectedCells()
        {
            foreach (var c in GetAllCells())
            {
                if (c.CellState == CellState.Selected)
                {
                    c.CellState = CellState.Idle;
                }
            }
        }

        private void ResetAllCells()
        {
            foreach (var c in GetAllCells())
            {
                c.CellState = CellState.Idle;
            }
        }

        private void UpdateCellsDisplay()
        {
            foreach (var c in GetAllCells())
            {
                switch (c.CellState)
                {
                    case CellState.Idle:
                        c.Background = new SolidColorBrush(CellColorDefault);
                        break;
                    case CellState.Hover:
                        c.Background = new SolidColorBrush(CellColorHover);

                        // check if we have a selected cell. if so, calculate path cells
                        // shit, recursion...
                        break;
                    case CellState.Path:
                        c.Background = new SolidColorBrush(Colors.Orange);
                        break;
                    case CellState.Pressed:
                        c.Background = new SolidColorBrush(CellColorClick);
                        break;
                    case CellState.Selected:
                        c.Background = new SolidColorBrush(CellColorHover);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
