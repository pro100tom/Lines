using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static Lines.Design;

namespace Lines
{
    enum CellState { Hover, Idle, Path, Pressed, Selected };

    class Cell : Canvas
    {
        public event DefaultEventHandler NotifyStateChanged;
        CellState state;
        //HashSet<string> neighbourCellNames;

        public Cell()
        {
            Background = new SolidColorBrush(CellColorDefault);
            Margin = new Thickness(0.01d);
            state = CellState.Idle;
            //neighbourCellNames = new HashSet<string>();
        }

        public bool HasBall
        {
            get
            {
                return Children.OfType<Ellipse>().Any();
            }
        }

        public void AddNeighbour(string name)
        {
            //neighbourCellNames.Add(name);
        }

        public void AddNeighbours(List<string> names)
        {
            foreach (string name in names)
            {
                AddNeighbour(name);
            }
        }

        internal CellState CellState
        {
            get
            {
                return state;
            }

            set
            {
                if (state == value)
                {
                    return;
                }

                state = value;
                NotifyStateChanged(this);
            }
        }
    }
}
