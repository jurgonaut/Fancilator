using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FanciLator
{
    class WaveDot
    {
        // to sploh ne rabim, je blo ustvarjeno samo za testiranje
        static int Count;
        public int Index { get; set; }
        public string Text { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        Ellipse e;

        public WaveDot( double x, double y, Canvas cv, string s)
        {
            Text = "string";
            Count++;
            Index = Count;
            X = x;
            Y = y;
            e = new Ellipse();
            e.Width = 30;
            e.Height = 30;
            SolidColorBrush Brush3 = new SolidColorBrush();
            switch (s)
            {
                case "g":
                    Brush3.Color = Color.FromArgb(255, 0, 102, 0);
                    break;
                case "b":
                    Brush3.Color = Color.FromArgb(255, 0, 51, 102);
                    break;
            }
            e.Fill = Brush3;
            Canvas.SetLeft(e, X - e.Width / 2);
            Canvas.SetTop(e, Y - e.Height / 2);
            Canvas.SetZIndex(e, 10);
            cv.Children.Add(e);
        }

        public int getIndex()
        {
            return Index;
        }
    }
}
