using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Lines.BallBounceController;
using static Lines.BallSpawnController;
using static Lines.CellHelper;
using static Lines.Design;
using static Lines.MovementCalculator;
using static Lines.Settings;
using static Lines.MovementHelper;

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

            NotifyBounceFinished += MainWindow_NotifyBounceFinished;

            NotifyStepMade += MainWindow_NotifyStepMade;
            NotifyMoveCompleted += MainWindow_NotifyMoveCompleted;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SpawnFewRandomBalls(SpawnBallQtyMin, SpawnBallQtyMax);

            /*SpawnBall("cell_11");
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
            for (int i = 0; i < 80; i++)
            {
                var name = cellNames.FetchRandomItem();
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
            if (Registry.DoesContainKey("lock") && (bool)Registry.GetItem("lock"))
            {
                return;
            }

            var currentCell = sender as Cell;

            // If we entered the same selected cell again...
            if (currentCell.State == CellState.Selected)
            {
                return;
            }

            // If we entered an empty cell and no other cell is selected...
            var doesSelectedCellExist = DoesSelectedCellExist();
            if (!doesSelectedCellExist)
            {
                currentCell.State = CellState.Hover;
                return;
            }

            ResetCells(CellState.Path);
            ResetGhostCells();
            ResetInaccessibleCells();

            var selectedCell = GetSelectedCell();
            var selectedIndex = GetCellIndex(selectedCell.Name);
            var currentIndex = GetCellIndex(currentCell.Name);

            var names = (from c in AllCells
                         where !c.HasBall || c.Name == selectedCell.Name || c.Name == currentCell.Name
                         select c.Name).ToList();
            var indices = GetCellNameIndices(names);
            var pathIndices = Pathfinder.Instance.FindPath(selectedIndex, currentIndex, indices);
            var pathNames = ConvertIndicesToNames(pathIndices);
            var pathCells = from c in AllCells
                            where pathNames.Contains(c.Name)
                        select c;

            Registry.PutItem("indices", pathIndices);

            // Now we assume that we have a selected cell somewhere
            // and we are entering a non empty cell.
            if (currentCell.HasBall)
            {
                foreach (var c in pathCells)
                {
                    c.State = CellState.Path;
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
                        c.State = CellState.Path;
                    }
                }
                else
                {
                    // Same as above but we are entering an empty cell which is not accessible.
                    foreach (var c in pathCells)
                    {
                        c.State = CellState.Path;
                    }

                    currentCell.Accessible = false;
                }
            }

            selectedCell.State = CellState.Selected;
            currentCell.State = CellState.Hover;
        }

        private void Cell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Registry.DoesContainKey("lock") && (bool)Registry.GetItem("lock"))
            {
                return;
            }

            var cell = sender as Cell;
            cell.Background = new SolidColorBrush(cell.Accessible ? CellColorPressed : CellColorInaccessiblePressed);
        }

        private void Cell_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Registry.DoesContainKey("lock") && (bool)Registry.GetItem("lock"))
            {
                return;
            }

            var currentCell = sender as Cell;
            var selectedCell = GetSelectedCell();

            Registry.PutItem("allow_move_calls", false);

            if (currentCell.Accessible)
            {
                ResetCells(CellState.Path);
                ResetGhostCells();
                ResetCells(CellState.Selected);
            }

            if (!currentCell.HasBall)
            {
                if (selectedCell == null)
                {
                    currentCell.State = CellState.Hover;
                }
                else
                {
                    if (currentCell.Accessible)
                    {
                        var selectedCellBall = selectedCell.GetBall();
                       
                        Registry.PutItem("active_ball", selectedCellBall);
                        Registry.PutItem("current_cell", currentCell);
                        Registry.PutItem("allow_move_calls", true);

                        selectedCellBall.BounceStop(true);
                    }
                    else
                    {
                        currentCell.State = CellState.Hover;
                    }
                }

                UpdateCellsDisplay();

                return;
            }

            // Now we assume current cell does have a ball.
            var currentBall = currentCell.GetBall();
            
            if (selectedCell == null)
            {
                currentCell.State = CellState.Selected;
                currentBall.BounceStart();
            }
            else
            {
                if (currentCell == selectedCell)
                {
                    // We get here when we click on the selected cell again.
                    currentCell.State = CellState.Hover;
                    var ball = currentCell.GetBall();
                    ball.BounceStop(true);
                }
                else
                {
                    // We get here when we have a selected cell but click on another one with the ball.
                    var selectedBall = selectedCell.GetBall();
                    selectedBall.BounceStop(true);

                    currentCell.State = CellState.Selected;
                    currentBall.BounceStart();
                }
            }

            UpdateCellsDisplay();
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            var cell = sender as Cell;

            switch (cell.State)
            {
                case CellState.Idle:
                    break;
                case CellState.Hover:
                    cell.State = CellState.Idle;
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

        private void MainWindow_NotifyBounceFinished(object sender)
        {
            var activeBall = sender as Ellipse;
            RestoreDefaultMargin(activeBall);

            if (Registry.DoesContainKey("allow_move_calls") && !(bool)Registry.GetItem("allow_move_calls"))
            {
                return;
            }

            Registry.PutItem("allow_move_calls", false);

            var activeBallCell = activeBall.Parent as Cell;

            var indices = Registry.GetItem("indices") as List<int>;
            if (!indices.Any())
            {
                return;
            }

            var movementInfoList = GetMovementInfoList(indices);
            if (!movementInfoList.Any())
            {
                return;
            }

            Registry.PutItem("movement_info_list", movementInfoList);

            Move(activeBallCell);
        }

        private void MainWindow_NotifyMoveCompleted(object sender)
        {
            var activeBall = Registry.GetItem("active_ball") as Ellipse;
            var activeBallCell = activeBall.Parent as Cell;

            var currentStagingCell = Registry.GetItem("staging_cell") as Cell;
            if (currentStagingCell == null)
            {
                currentStagingCell = Registry.GetItem("current_cell") as Cell;
                if (currentStagingCell == null)
                {
                    return;
                }
            }

            PassBallToAnotherCell(activeBallCell, currentStagingCell);

            Move(currentStagingCell);
        }

        private void Move(Cell currentCell)
        {
            Registry.PutItem("lock", true);

            var movementInfoList = Registry.GetItem("movement_info_list") as List<MovementInfo>;
            if (!movementInfoList.Any())
            {
                Registry.PutItem("lock", false);
                return;
            }

            var movementInfoItem = movementInfoList.Dequeue();
            Registry.PutItem("movement_info_list", movementInfoList);

            var currentIndex = GetCellIndex(currentCell.Name);
            var nextStagingCell = GetNextStagingCell(currentIndex, movementInfoItem);
            if (nextStagingCell == null)
            {
                return;
            }

            Registry.PutItem("staging_cell", nextStagingCell);

            var activeBall = Registry.GetItem("active_ball") as Ellipse;
            double coordinate = activeBall.Margin.Left;
            double step = 30.0d;
            double threshold = movementInfoItem.StepNumber * currentCell.ActualWidth - step;

            BringBallOnTop(currentCell);
            StartMovementTimer(coordinate, step, threshold, movementInfoItem.Direction);
        }

        private void PassBallToAnotherCell(Cell cellWithBall, Cell targetCell)
        {
            var ball = cellWithBall.GetBall();
            RestoreDefaultMargin(ball);

            var cell = ball.Parent as Cell;
            cell.Children.Clear();

            targetCell.Children.Add(ball);
        }

        private void RestoreDefaultMargin(Ellipse ball)
        {
            Canvas.SetLeft(ball, 0);
            Canvas.SetRight(ball, 0);
            Canvas.SetTop(ball, 0);
            Canvas.SetBottom(ball, 0);

            var cell = ball.Parent as Cell;
            var defaultBallMargin = GetBallDefaultMargin(cell.ActualWidth);
            ball.Margin = new Thickness(defaultBallMargin);
        }

        private Cell GetNextStagingCell(int currentIndex, MovementInfo movementInfo)
        {
            var stagingCellIndex = GetIndexByOffset(currentIndex, movementInfo.Direction, movementInfo.StepNumber, FieldSideCellCount);
            var stagingCelName = GetCellName(stagingCellIndex);
            var stagingCell = GetCell(stagingCelName);

            return stagingCell;
        }

        private void MainWindow_NotifyStepMade(double coordinate, Direction direction)
        {
            var activeBall = Registry.GetItem("active_ball") as Ellipse;
            var margin = activeBall.Margin;

            if (direction == Direction.Left || direction == Direction.Right)
            {
                Canvas.SetLeft(activeBall, coordinate);
                Canvas.SetRight(activeBall, coordinate);
            }
            else
            {
                Canvas.SetTop(activeBall, coordinate);
                Canvas.SetBottom(activeBall, coordinate);
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

        private void Cell_NotifyStateChanged(object sender)
        {
            UpdateCellsDisplay();
        }

        private void UpdateCellsDisplay()
        {
            foreach (var c in AllCells)
            {
                switch (c.State)
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

        private bool DoesSelectedCellExist()
        {
            var selected = GetSelectedCell();
            var exists = selected != null;

            return exists;
        }

        private Cell GetSelectedCell()
        {
            var selected = (from c in AllCells
                            where c.State == CellState.Selected
                            select c).SingleOrDefault();

            return selected;
        }

        private Cell GetCell(string name)
        {
            var cell = (from c in AllCells
                      where c.Name == name
                      select c).SingleOrDefault();

            return cell;
        }

        private void ResetCells(CellState cellState)
        {
            foreach (var c in AllCells)
            {
                if (c.State == cellState)
                {
                    c.State = CellState.Idle;
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
                c.State = CellState.Idle;
                c.Accessible = true;
                c.Ghost = false;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Close(); }
        }
    }
}
