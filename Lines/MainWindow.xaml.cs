using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Lines.BallBounceEventManager;
using static Lines.BallSpawnController;
using static Lines.BalSizeEventManager;
using static Lines.CellHelper;
using static Lines.Design;
using static Lines.GameHelper;
using static Lines.MovementEventManager;
using static Lines.MovementHelper;
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
            InitializeEventListeners();
            InitializeScoreboard();
        }

        private void InitializeEventListeners()
        {
            NotifyBounceFinished += MainWindow_NotifyBounceFinished;
            NotifyGrowFinished += MainWindow_NotifyGrowFinished;
            NotifyShrinkFinished += MainWindow_NotifyShrinkFinished;

            NotifyStepMade += MainWindow_NotifyStepMade;
            NotifyMoveCompleted += MainWindow_NotifyMoveCompleted;
        }

        private void InitializeScoreboard()
        {
            AdjustFont();
            AlignScoreboard();
        }

        private void DisplayGameOverBlock()
        {
            Overlay.Opacity = 0.5;
            GameOver.Opacity = 0.7;
            Panel.SetZIndex(Overlay, 3);
            Panel.SetZIndex(GameOverBorder, 4);

            Overlay.Visibility = Visibility.Visible;
            GameOverBorder.Visibility = Visibility.Visible;
        }

        private void AdjustFont()
        {
            Scoreboard.Text = "Score\n0";
            var coefficient = MainField.ActualHeight / 1040;
            if (coefficient == 0)
            {
                return;
            }

            Scoreboard.FontSize *= coefficient;
            GameOver.FontSize *= coefficient;
        }

        private void AlignScoreboard()
        {
            var mainFieldWidth = MainField.Width; 
            var windowWidth = MainField.ActualWidth;
            var difference = windowWidth - mainFieldWidth;
            var oneSideGap = difference / 2;

            Scoreboard.Width = oneSideGap / 2;
            var scoreboardWidth = Scoreboard.Width;

            var margin = Scoreboard.Margin;
            var horizontalOffset = (oneSideGap - scoreboardWidth) / 2;

            margin.Right = horizontalOffset;

            Scoreboard.Margin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            /*SpawnBall("cell_12", Colors.AliceBlue);
            SpawnBall("cell_16", Colors.AliceBlue);
            SpawnBall("cell_20", Colors.AliceBlue);

            SpawnBall("cell_24", Colors.AliceBlue);
            SpawnBall("cell_27", Colors.AliceBlue);
            SpawnBall("cell_30", Colors.AliceBlue);

            SpawnBall("cell_36", Colors.AliceBlue);
            SpawnBall("cell_38", Colors.AliceBlue);
            SpawnBall("cell_40", Colors.AliceBlue);

            SpawnBall("cell_36", Colors.AliceBlue);
            SpawnBall("cell_38", Colors.AliceBlue);
            SpawnBall("cell_40", Colors.AliceBlue);

            SpawnBall("cell_48", Colors.AliceBlue);
            SpawnBall("cell_49", Colors.AliceBlue);
            SpawnBall("cell_50", Colors.AliceBlue);

            SpawnBall("cell_56", Colors.AliceBlue);
            SpawnBall("cell_57", Colors.AliceBlue);
            SpawnBall("cell_58", Colors.AliceBlue);
            SpawnBall("cell_59", Colors.AliceBlue);
            SpawnBall("cell_61", Colors.AliceBlue);
            SpawnBall("cell_62", Colors.AliceBlue);
            SpawnBall("cell_63", Colors.AliceBlue);
            SpawnBall("cell_64", Colors.AliceBlue);

            SpawnBall("cell_70", Colors.AliceBlue);
            SpawnBall("cell_71", Colors.AliceBlue);
            SpawnBall("cell_72", Colors.AliceBlue);

            SpawnBall("cell_80", Colors.AliceBlue);
            SpawnBall("cell_82", Colors.AliceBlue);
            SpawnBall("cell_84", Colors.AliceBlue);

            SpawnBall("cell_90", Colors.AliceBlue);
            SpawnBall("cell_93", Colors.AliceBlue);
            SpawnBall("cell_96", Colors.AliceBlue);

            SpawnBall("cell_100", Colors.AliceBlue);
            SpawnBall("cell_104", Colors.AliceBlue);
            SpawnBall("cell_108", Colors.AliceBlue);

            SpawnBall("cell_119", Colors.AliceBlue);*/

            /*SpawnBall("cell_4");
            SpawnBall("cell_6");
            SpawnBall("cell_9");
            SpawnBall("cell_10");
            SpawnBall("cell_14");
            SpawnBall("cell_15");
            SpawnBall("cell_18");
            SpawnBall("cell_19");
            SpawnBall("cell_20");
            SpawnBall("cell_24");*/

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
            var emptyCellNames = (from cell in AllCells
                             where !cell.HasBall
                             select cell.Name).ToList();

            var result = GetBallSpawnQty(min, max);

            bool shouldDisplayGaveOver = false;

            if (emptyCellNames.Count() < result)
            {
                result = emptyCellNames.Count;

                shouldDisplayGaveOver = true;
            }

            for (int i = 0; i < result; i++)
            {
                var name = emptyCellNames.FetchRandomItem();
                var color = GetRandomColor();
                SpawnBall(name, color);
            }

            if (shouldDisplayGaveOver)
            {
                DisplayGameOverBlock();
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

        private void SpawnBall(string cellName, Color color)
        {
            var cell = (from c in AllCells
                        where c.Name == cellName
                        select c).SingleOrDefault();

            if (cell == null || cell.HasBall) { return; }

            var ball = CreateBall();
            ball.Fill = new SolidColorBrush(color);

            double ballWidth = cell.ActualWidth * BallSizePercentage;
            double ballHeight = ballWidth;

            double margin = GetBallDefaultMargin(cell.ActualWidth);
            ball.Margin = new Thickness(margin);

            cell.Children.Add(ball);

            ball.Grow(ballWidth, cell.ActualWidth);
        }

        private double GetBallDefaultMargin(double cellWidth)
        {
            var ballWidth = cellWidth * BallSizePercentage;
            var margin = GetBallMargin(cellWidth, ballWidth);

            return margin;
        }

        private double GetBallMargin(double cellWidth, double ballWidth)
        {
            return (cellWidth - ballWidth) / 2;
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            try
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
                var pathIndices = Pathfinder.FindPath(selectedIndex, currentIndex, indices);
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
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
        }

        private void Cell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Registry.DoesContainKey("lock") && (bool)Registry.GetItem("lock"))
                {
                    return;
                }

                var cell = sender as Cell;
                cell.Background = new SolidColorBrush(cell.Accessible ? CellColorPressed : CellColorInaccessiblePressed);
            }
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
        }

        private void Cell_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
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
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            try
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
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
        }

        private void MainWindow_NotifyBounceFinished(object sender)
        {
            try
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
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
        }

        private void MainWindow_NotifyMoveCompleted(object sender)
        {
            try
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
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
        }

        private void Move(Cell currentCell)
        {
            try
            {
                Registry.PutItem("lock", true);
                
                var movementInfoList = Registry.GetItem("movement_info_list") as List<MovementInfo>;
                if (!movementInfoList.Any())
                {
                    int removedCount = RemoveBalls();
                    int currentMultiplier = 1;

                    if (removedCount == 0)
                    {
                        SpawnFewRandomBalls(SpawnBallQtyMin, SpawnBallQtyMax);

                        Registry.PutItem("bonus_multiplier", 1);
                    }
                    else
                    {
                        currentMultiplier = Registry.DoesContainKey("bonus_multiplier") ? Convert.ToInt32(Registry.GetItem("bonus_multiplier")) : 1;
                        Registry.PutItem("bonus_multiplier", currentMultiplier + 1);
                    }

                    UpdateScoreboard(removedCount * 10 * currentMultiplier);

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
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
        }

        private async void MainWindow_NotifyGrowFinished(object sender)
        {
            await Task.Delay(300);

            RemoveBalls();
        }

        private void MainWindow_NotifyShrinkFinished(object sender)
        {
            var ball = sender as Ellipse;
            var cell = ball.Parent as Cell;

            if (cell != null)
            {
                cell.Children.Clear();
            }
        }

        private void PassBallToAnotherCell(Cell cellWithBall, Cell targetCell)
        {
            try
            {
                var ball = cellWithBall.GetBall();
                RestoreDefaultMargin(ball);

                var cell = ball.Parent as Cell;
                cell.Children.Clear();

                targetCell.Children.Add(ball);
            }
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);
            }
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

        private int RemoveBalls()
        {
            try
            {
                var cellsToClear = new HashSet<Cell>();

                for (int i = 0; i < FieldSideCellCount; i++)
                {
                    // Horizontal scan.
                    var cellsFromRow = GetCellsFromRow(i);
                    var list = GetSuccessiveBallList(cellsFromRow);
                    foreach (var tuple in list)
                    {
                        if (tuple.Item2 < MinNumberOfBallsRequired)
                        {
                            continue;
                        }

                        int last = tuple.Item1 + tuple.Item2;
                        for (int j = tuple.Item1; j < last; j++)
                        {
                            var cellName = GetCellName(j);
                            var cell = GetCell(cellName);

                            cellsToClear.Add(cell);
                        }
                    }

                    // Vertical scan.
                    var cellsFromColumn = GetCellsFromColumn(i);
                    list = GetSuccessiveBallList(cellsFromColumn);
                    foreach (var tuple in list)
                    {
                        if (tuple.Item2 < MinNumberOfBallsRequired)
                        {
                            continue;
                        }

                        var currentIndex2D = tuple.Item1.ToIndex2D();
                        int last = currentIndex2D.Item1 + tuple.Item2;

                        for (int j = currentIndex2D.Item1; j < last; j++)
                        {
                            var index2D = Tuple.Create(j, currentIndex2D.Item2);
                            var index = index2D.ToIndex1D();

                            var cellName = GetCellName(index);
                            var cell = GetCell(cellName);

                            cellsToClear.Add(cell);
                        }
                    }

                    // Diagonal down right horizontal/vertical scan.
                    for (int k = 0; k < 2; k++)
                    {
                        bool horizontal = k == 0;
                        var cellsFromDiagonal = GetCellsFromDownRightDiagonal(i, horizontal);

                        list = GetSuccessiveBallList(cellsFromDiagonal);
                        foreach (var tuple in list)
                        {
                            if (tuple.Item2 < MinNumberOfBallsRequired)
                            {
                                continue;
                            }

                            var currentIndex2D = tuple.Item1.ToIndex2D();
                            int last = tuple.Item2;

                            for (int j = 0; j < last; j++)
                            {
                                var index2D = Tuple.Create(currentIndex2D.Item1 + j, currentIndex2D.Item2 + j);
                                var index = index2D.ToIndex1D();

                                var cellName = GetCellName(index);
                                var cell = GetCell(cellName);

                                cellsToClear.Add(cell);
                            }
                        }
                    }

                    // Diagonal down left horizontal/vertical scan.
                    for (int k = 0; k < 2; k++)
                    {
                        bool horizontal = k == 0;

                        var cellsFromDiagonal = GetCellsFromDownLeftDiagonal(i, horizontal);

                        list = GetSuccessiveBallList(cellsFromDiagonal);
                        foreach (var tuple in list)
                        {
                            if (tuple.Item2 < MinNumberOfBallsRequired)
                            {
                                continue;
                            }

                            var currentIndex2D = tuple.Item1.ToIndex2D();
                            int last = tuple.Item2;

                            for (int j = 0; j < last; j++)
                            {
                                var index2D = Tuple.Create(currentIndex2D.Item1 + j, currentIndex2D.Item2 - j);
                                var index = index2D.ToIndex1D();

                                var cellName = GetCellName(index);
                                var cell = GetCell(cellName);

                                cellsToClear.Add(cell);
                            }
                        }
                    }
                }

                int cellsFound = cellsToClear.Count();

                foreach (var cell in cellsToClear)
                {
                    var ball = cell.GetBall();
                    ball.Shrink(1, cell.ActualWidth);
                }

                return cellsFound;
            }
            catch (Exception exception)
            {
                int gg = 0;

                MessageBox.Show(exception.Message);

                return 0;
            }
        }

        private List<Cell> PrepareRemoval(List<Cell> cells)
        {
            var cellsToClear = new List<Cell>();
            var list = GetSuccessiveBallList(cells);
            foreach (var tuple in list)
            {
                if (tuple.Item2 < MinNumberOfBallsRequired)
                {
                    continue;
                }

                int last = tuple.Item1 + tuple.Item2;
                for (int j = tuple.Item1; j < last; j++)
                {
                    var cellName = GetCellName(j);
                    var cell = GetCell(cellName);

                    cellsToClear.Add(cell);
                }
            }

            return cellsToClear;
        }

        private List<Tuple<int, int>> GetSuccessiveBallList(List<Cell> cells)
        {
            // index, count and color
            var list = new List<Tuple<int, int, Color>>();
            bool shallAddNewItem = true;

            foreach (var cell in cells)
            {
                if (!cell.HasBall)
                {
                    shallAddNewItem = true;
                    continue;
                }

                var cellName = cell.Name;
                var cellIndex = GetCellIndex(cellName);

                var ball = cell.GetBall();
                var ballColor = ball.GetColor();

                if (!list.Any())
                {
                    shallAddNewItem = true;
                }

                if (list.Any())
                {
                    var last = list.Last();

                    if (ballColor != last.Item3)
                    {
                        shallAddNewItem = true;
                    }
                }

                if (shallAddNewItem)
                {
                    var tuple = Tuple.Create(cellIndex, 1, ballColor);
                    list.Add(tuple);
                }
                else
                {
                    var last = list.Last();
                    list[list.Count - 1] = new Tuple<int, int, Color>(last.Item1, last.Item2 + 1, ballColor);
                }

                shallAddNewItem = false;
            }

            var strippedList = (from tuple in list
                                select Tuple.Create(tuple.Item1, tuple.Item2)).ToList();

            return strippedList;
        }

        private List<Cell> GetCellsFromRow(int rowNumber)
        {
            var list = new List<Cell>();

            foreach (var cell in AllCells)
            {
                var cellName = cell.Name;
                var cellIndex = GetCellIndex(cellName);
                var cellIndex2D = cellIndex.ToIndex2D();

                if (cellIndex2D.Item1 == rowNumber)
                {
                    list.Add(cell);
                }
            }

            return list;
        }

        private List<Cell> GetCellsFromColumn(int columnNumber)
        {
            var list = new List<Cell>();

            foreach (var cell in AllCells)
            {
                var cellName = cell.Name;
                var cellIndex = GetCellIndex(cellName);
                var cellIndex2D = cellIndex.ToIndex2D();

                if (cellIndex2D.Item2 == columnNumber)
                {
                    list.Add(cell);
                }
            }

            return list;
        }

        private List<Cell> GetCellsFromDownRightDiagonal(int topLeftIndex, bool iterateHorizontally = true)
        {
            var list = new List<Cell>();

            var topLeftIndex2D = topLeftIndex.ToIndex2D();
            if (!iterateHorizontally)
            {
                topLeftIndex2D = Tuple.Create(topLeftIndex2D.Item2, topLeftIndex2D.Item1);
            }

            for (int i = 0; i < FieldSideCellCount; i++)
            {
                var nextCellIndex2D = Tuple.Create(topLeftIndex2D.Item1 + i, topLeftIndex2D.Item2 + i);
                if (nextCellIndex2D.Item1 >= FieldSideCellCount || nextCellIndex2D.Item2 >= FieldSideCellCount)
                {
                    continue;
                }

                var nextCellIndex = nextCellIndex2D.ToIndex1D();
                var nextCellName = GetCellName(nextCellIndex);
                var nextCell = GetCell(nextCellName);

                if (nextCell == null)
                {
                    continue;
                }

                list.Add(nextCell);
            }

            return list;
        }

        private List<Cell> GetCellsFromDownLeftDiagonal(int topRightIndex, bool iterateHorizontally = true)
        {
            var list = new List<Cell>();

            var topRightIndex2D = topRightIndex.ToIndex2D();
            if (!iterateHorizontally)
            {
                topRightIndex2D = Tuple.Create(topRightIndex2D.Item2, FieldSideCellCount - topRightIndex2D.Item1 - 1);
            }

            for (int i = 0; i < FieldSideCellCount; i++)
            {
                var nextCellIndex2D = Tuple.Create(topRightIndex2D.Item1 + i, topRightIndex2D.Item2 - i);
                if (nextCellIndex2D.Item1 >= FieldSideCellCount || nextCellIndex2D.Item2 >= FieldSideCellCount
                    || nextCellIndex2D.Item1 < 0 || nextCellIndex2D.Item2 < 0)
                {
                    continue;
                }

                var nextCellIndex = nextCellIndex2D.ToIndex1D();
                var nextCellName = GetCellName(nextCellIndex);
                var nextCell = GetCell(nextCellName);

                if (nextCell == null)
                {
                    continue;
                }

                list.Add(nextCell);
            }

            return list;
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

        private void UpdateScoreboard(int points)
        {
            int currentPoints = GetScoreboardPoints();
            int result = currentPoints + points;
            Scoreboard.Text = "Score\n" + result.ToString();
        }

        private int GetScoreboardPoints()
        {
            var points = Scoreboard.Text.Split('\n').Last();

            return Convert.ToInt32(points);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Close(); }
        }
    }
}
