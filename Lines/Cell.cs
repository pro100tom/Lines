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
        bool accessible, ghost;

        public Cell()
        {
            Background = new SolidColorBrush(CellColorIdle);
            Margin = new Thickness(0.01d);
            state = CellState.Idle;
            accessible = true;
        }

        public bool HasBall
        {
            get
            {
                return Children.OfType<Ellipse>().Any();
            }
        }

        public Ellipse GetBall()
        {
            return HasBall ? Children.OfType<Ellipse>().First() : null;
        }

        public void RemoveBall()
        {
            if (HasBall)
            {
                var ball = GetBall();
                Children.Remove(ball);
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
                if (state == value) { return; }

                state = value;
                NotifyStateChanged(this);
            }
        }

        public bool Accessible
        {
            get
            {
                return accessible;
            }

            set
            {
                accessible = value;
            }
        }

        public bool Ghost
        {
            get
            {
                return ghost;
            }

            set
            {
                ghost = value;
            }
        }
    }
}
