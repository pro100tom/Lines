using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Lines.Helper;
using static Lines.Design;
using static Lines.Settings;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Windows.Documents;

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
            SpawnFewRandomBalls();
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
                    var ball = new Ellipse()
                    {
                        Margin = new Thickness(10.0d),
                        Fill = new SolidColorBrush(Colors.AliceBlue),
                        Stroke = new SolidColorBrush(Colors.Black),
                        IsHitTestVisible = false,
                        Width = 70,
                        Height = 70,
                        ClipToBounds = false,
                    };

                    var cell = CreateCell();
                    //cell.Children.Add(ball);

                    cell.Name = "cell" + GetAllCells().Count().ToString();
                    UIElement child = cell;

                    if (UseBorder)
                    {
                        var border = new Border()
                        {
                            BorderThickness = new Thickness(CellBorderThinknessLength),
                            BorderBrush = new SolidColorBrush(CellBorderColor),
                            SnapsToDevicePixels = CellBorderSnapToDevicePixelsFlag,
                        };

                        border.Child = cell;
                        child = border;
                    }

                    MainField.Children.Add(child);
                    if (i == 0 && j == 0)
                    {
                        //var border = MainField.Children[0] as Border;
                        //var cell2 = border.Child as Cell;
                        //cell2.Children.Add(ball);
                    }
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

            return cell;
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            var cell = sender as Cell;
            cell.Background = new SolidColorBrush(CellColorHover);
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

            //Panel.SetZIndex(cell, 1);
        }

        private void SpawnFewRandomBalls()
        {
            var result = GetBallSpawnQty(SpawnBallQtyMin, SpawnBallQtyMax);
            var cells = GetAllCells();
            var cellsCopy = cells.ToList();

            for (int i = 0; i < result; i++)
            {
                var cell = FetchRandomItem(cellsCopy);
                SpawnBall(cell.Name);
            }
        }

        private IEnumerable<Cell> GetAllCells()
        {
            var result = from cell in MainField.Children.OfType<Cell>()
                         select cell as Cell;

            if (!result.Any())
            {
                result = from border in MainField.Children.OfType<Border>()
                         select border.Child as Cell;
            }

            return result;
        }

        private void SpawnBall(string cellName)
        {
            var cell = (from c in GetAllCells()
                        where c.Name == cellName
                        select c).SingleOrDefault();

            

            var ball = CreateBall();

            

            

            foreach (var border2 in MainField.Children.OfType<Border>())
            {
                var cell3 = border2.Child as Cell;
                if (cell3.Name == cellName)
                {
                    ball.Width = 0.65d * cell3.ActualWidth;
                    ball.Height = ball.Width;

                    double margin = (cell3.ActualWidth - ball.Width) / 2.0d;
                    ball.Margin = new Thickness(margin);

                    ball.Width = 50;
                    ball.Height = 50;
                    ball.Margin = new Thickness(2.0d);

                    cell3.Children.Add(ball);
                }
            }

            //cell.Children.Add(ball);
            //var border = MainField.Children[0] as Border;
            //var cell2 = border.Child as Cell;
            //cell2.Children.Add(ball);
        }

        private Ellipse CreateBall()
        {
            var ball = new Ellipse()
            {
                Margin = new Thickness(10.0d),
                Fill = new SolidColorBrush(Colors.AliceBlue),
                Stroke = new SolidColorBrush(Colors.Black),
                IsHitTestVisible = false,
                ClipToBounds = false,
            };

            return ball;
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            var cell = sender as Cell;
            cell.Background = new SolidColorBrush(CellColorDefault);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
