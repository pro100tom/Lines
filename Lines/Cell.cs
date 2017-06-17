using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static Lines.Design;

namespace Lines
{
    class Cell : Canvas
    {
        public Cell()
        {
            Background = new SolidColorBrush(CellColorDefault);
            Margin = new Thickness(0.01d);
        }
    }
}
