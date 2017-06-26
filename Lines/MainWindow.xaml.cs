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
using static Lines.BounceHelper;
using static Lines.MoveHelper;
using static Lines.BallHelper;

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

            NotifyBounceStopped += MainWindow_NotifyBounceStopped;
            NotifyMoveStopped += MainWindow_NotifyMoveStopped;
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

            cell.Name = String.Join(CellNameSeparator.ToString(), CellPrefix, AllCells.Count().ToString());

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
            var cellNames = (from cell in AllCells
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

        private IEnumerable<Cell> AllCells
        {
            get
            {
                return from border in MainField.Children.OfType<Border>()
                       select border.Child as Cell;
            }
        }

        private IEnumerable<Border> GetAllBorders()
        {
            var borders = from border in MainField.Children.OfType<Border>()
                        select border as Border;

            return borders;
        }

        private void SpawnBall(string cellName)
        {
            var cell = (from c in AllCells
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

            var names = (from c in AllCells
                         where !c.HasBall || c.Name == selectedCell.Name || c.Name == currentCell.Name
                         select c.Name).ToList();
            var indices = GetCellNameIndices(names);
            var pathIndices = FindPath(selectedIndex, currentIndex, indices);
            var pathNames = ConvertIndicesToNames(pathIndices);
            var pathCells = from c in AllCells
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
            cell.Background = new SolidColorBrush(cell.Accessible ? CellColorPressed : CellColorInaccessiblePressed);
        }

        private void Cell_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var currentCell = sender as Cell;
            var selectedCell = GetSelectedCell();

            if (currentCell.Accessible)
            {
                ResetPathCells();
                ResetGhostCells();
                ResetSelectedCells();
            }

            if (!currentCell.HasBall)
            {
                if (selectedCell == null)
                {
                    currentCell.CellState = CellState.Hover;
                }
                else
                {
                    if (currentCell.Accessible)
                    {
                        var selectedCellBall = selectedCell.GetBall();
                        selectedCellBall.BounceStop();

                        LastAccessedCell = currentCell;
                        AllowMove = true;
                    }
                    else
                    {
                        currentCell.CellState = CellState.Hover;
                    }
                }

                UpdateCellsDisplay();

                return;
            }

            // Now we assume current cell does have a ball.
            var currentBall = currentCell.GetBall();

            if (selectedCell == null)
            {
                currentCell.CellState = CellState.Selected;
                currentBall.BounceStart();
            }
            else
            {
                if (currentCell == selectedCell)
                {
                    // We get here when we click on the selected cell again.
                    currentCell.CellState = CellState.Hover;
                    var ball = currentCell.GetBall();
                    ball.BounceStop();
                }
                else
                {
                    // We get here when we have a selected cell but click on another one with the ball.
                    var selectedBall = selectedCell.GetBall();
                    selectedBall.BounceStop();
                    
                    currentCell.CellState = CellState.Selected;
                    currentBall.BounceStart();
                }
            }

            UpdateCellsDisplay();
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

        private void BringBallOnTop(Cell cell)
        {
            foreach (var b in GetAllBorders())
            {
                Panel.SetZIndex(b, ZIndexDefault);
            }

            Panel.SetZIndex(cell.Parent as Border, ZIndexTop);
        }

        private void MainWindow_NotifyBounceStopped(object sender)
        {
            if (!AllowMove) { return; }

            AllowMove = false;

            var ellipse = sender as Ellipse;
            var selectedCell = ellipse.Parent as Cell;
            BringBallOnTop(selectedCell);

            ellipse.Move(Indices, selectedCell.ActualWidth);
        }

        private void MainWindow_NotifyMoveStopped(object sender)
        {
            var ellipse = sender as Ellipse;
            var selectedCell = ellipse.Parent as Cell;
            var currentCell = GetCurrentCell();
            var defaultMargin = GetBallDefaultMargin(currentCell.ActualWidth);

            var ball = selectedCell.GetBall();
            ball.Margin = new Thickness(defaultMargin);

            selectedCell.RemoveBall();
            currentCell.Children.Add(ball);
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

        private Cell GetCurrentCell()
        {
            return LastAccessedCell;
        }

        private Cell GetSelectedCell()
        {
            var selected = (from c in AllCells
                            where c.CellState == CellState.Selected
                            select c).SingleOrDefault();

            return selected;
        }

        private void ResetHoverCells()
        {
            foreach (var c in AllCells)
            {
                if (c.CellState == CellState.Hover)
                {
                    c.CellState = CellState.Idle;
                }
            }
        }

        private void ResetPathCells()
        {
            foreach (var c in AllCells)
            {
                if (c.CellState == CellState.Path)
                {
                    c.CellState = CellState.Idle;
                }
            }
        }

        private void ResetSelectedCells()
        {
            foreach (var c in AllCells)
            {
                if (c.CellState == CellState.Selected)
                {
                    c.CellState = CellState.Idle;
                }
            }
        }

        private void ResetInaccessibleCells()
        {
            foreach (var c in AllCells)
            {
                c.Accessible = true;
            }
        }

        private void ResetGhostCells()
        {
            foreach (var c in AllCells)
            {
                c.Ghost = false;
            }
        }

        private void ResetAllCells()
        {
            foreach (var c in AllCells)
            {
                c.CellState = CellState.Idle;
                c.Accessible = true;
                c.Ghost = false;
            }
        }

        private void UpdateCellsDisplay()
        {
            foreach (var c in AllCells)
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
