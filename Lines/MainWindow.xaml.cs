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
using static Lines.PathfindingHelper;

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
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SpawnFewRandomBalls(SpawnBallQtyMin, SpawnBallQtyMax);
            /*SpawnBall("cell_0");
            SpawnBall("cell_4");
            SpawnBall("cell_6");
            SpawnBall("cell_9");
            SpawnBall("cell_10");
            SpawnBall("cell_14");
            SpawnBall("cell_15");
            SpawnBall("cell_18");
            SpawnBall("cell_19");
            SpawnBall("cell_20");
            SpawnBall("cell_24");*/

            //var pathIndices = FindPath(0, 8, new List<int>() {0, 1, 2, 3, 5, 7, 8, 11, 12, 13, 16, 17, 21, 22, 23 });
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

            cell.Name = String.Join(CellNameSeparator.ToString(), CellPrefix, GetAllCells().Count().ToString());

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

        private void SpawnFewRandomBalls(int min, int max)
        {
            var cellNames = (from cell in GetAllCells()
                             where !cell.HasBall
                             select cell.Name).ToList();

            if (!cellNames.Any()) { return; }

            var result = GetBallSpawnQty(min, max);
            for (int i = 0; i < 50; i++)
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
            var currentCell = sender as Cell;

            // If we entered the same selected cell again...
            if (currentCell.CellState == CellState.Selected)
            {
                return;
            }

            // If we entered an empty cell and no other cell is selected...
            var doesSelectedCellExist = DoesSelectedCellExist();
            if (!doesSelectedCellExist)
            {
                currentCell.CellState = CellState.Hover;
                return;
            }

            ResetPathCells();
            ResetGhostCells();
            ResetInaccessibleCells();

            var selectedCell = GetSelectedCell();
            var selectedIndex = GetCellNameIndex(selectedCell.Name);
            var currentIndex = GetCellNameIndex(currentCell.Name);

            var names = (from c in GetAllCells()
                         where !c.HasBall || c.Name == selectedCell.Name || c.Name == currentCell.Name
                         select c.Name).ToList();
            var indices = GetCellNameIndices(names);
            var pathIndices = FindPath(selectedIndex, currentIndex, indices);
            var pathNames = ConvertIndicesToNames(pathIndices);
            var pathCells = from c in GetAllCells()
                        where pathNames.Contains(c.Name)
                        select c;

            // Now we assume that we have a selected cell somewhere
            // and we are entering a non empty cell.
            if (currentCell.HasBall)
            {
                foreach (var c in pathCells)
                {
                    c.CellState = CellState.Path;
                    c.Ghost = true;
                }

                selectedCell.Ghost = true;
            }
            else
            {
                bool pathFound = pathCells.Contains(currentCell);
                if (pathFound)
                {
                    // We still assume we have a selected cell somewhere
                    // and we are entering an empty cell which is accessible.
                    foreach (var c in pathCells)
                    {
                        c.CellState = CellState.Path;
                    }
                }
                else
                {
                    // Same as above but we are entering an empty cell which is not accessible.
                    foreach (var c in pathCells)
                    {
                        c.CellState = CellState.Path;
                    }

                    currentCell.Accessible = false;
                }
            }

            selectedCell.CellState = CellState.Selected;
            currentCell.CellState = CellState.Hover;
        }

        private void Cell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as Cell;

            cell.CellState = CellState.Pressed;
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

        private void Cell_NotifyStateChanged(object sender)
        {
            UpdateCellsDisplay();
        }

        private bool DoesSelectedCellExist()
        {
            var selected = GetSelectedCell();
            var exists = selected != null;

            return exists;
        }

        private Cell GetSelectedCell()
        {
            var selected = (from c in GetAllCells()
                            where c.CellState == CellState.Selected
                            select c).SingleOrDefault();

            return selected;
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

        private void ResetPathCells()
        {
            foreach (var c in GetAllCells())
            {
                if (c.CellState == CellState.Path)
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

        private void ResetInaccessibleCells()
        {
            foreach (var c in GetAllCells())
            {
                c.Accessible = true;
            }
        }

        private void ResetGhostCells()
        {
            foreach (var c in GetAllCells())
            {
                c.Ghost = false;
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
                        c.Background = new SolidColorBrush(CellColorIdle);
                        break;
                    case CellState.Hover:
                        c.Background = new SolidColorBrush(c.Accessible ? CellColorHover : CellColorInaccessible);
                        break;
                    case CellState.Path:
                        c.Background = new SolidColorBrush(c.Ghost ? CellColorGhost : CellColorPath);
                        break;
                    case CellState.Pressed:
                        c.Background = new SolidColorBrush(c.Accessible ? CellColorPressed : CellColorInaccessiblePressed);
                        break;
                    case CellState.Selected:
                        c.Background = new SolidColorBrush(c.Ghost ? CellColorSelectedGhost : CellColorSelected);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
