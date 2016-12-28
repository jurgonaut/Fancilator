using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using NAudio;
using NAudio.Wave;
using System.Threading;

namespace FanciLator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // sem razmišljal, močoge bi ustvaru ene ellipse elemente za
        // kontroliranje ostalih. Vsak premik bi šel preko tega elementa
        // in pole bi spremembo prenesu na wave element. Nevem mogoče
        // niti ni tako dobra ideja, ker bi blo več računjanja..

        bool addWave;
        bool removeWave;

        // se rabi za premikanje elementov na canvasu
        public bool isBeingDragged;
        public int currElem;
        SinWaveProvider32 usable;

        // vse frekvence "allSin", ki je ubisvu zbirka "sinWavePorviderju"
        AllSinWaves allSin = new AllSinWaves();
        // "waveOut" je izhodnja naprava ki producira zvok - slušake, zvočniki
        WaveOut waveOut;

        public MainWindow()
        {
            InitializeComponent();

            drawBackground();

            addWave = false;
            removeWave = false;

            waveOut = new WaveOut();
        }
       
        // kontrole za canvas
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // če je gumb za dodajanej kliknjen doda novo frekvenco
            if (addWave == true)
            {
                var position = e.GetPosition(canvas);
                SinWaveProvider32 newSin = new SinWaveProvider32(position.X, position.Y, canvas);
                allSin.waves.Add(newSin);
                waveOut.Init(newSin);
                waveOut.Play();
                addWave = false;
                btnAddWave.ClearValue(Button.BackgroundProperty);
            }
            // čene pogleda če je kateri od obsotoješih kliknjen in dela z njim
            else
            {
                // to deuje na vse elemente
                if (!(e.OriginalSource is Canvas))
                {
                    var element = (FrameworkElement)e.OriginalSource;

                    // Zindex reši problem prekrivnja elementov
                    Canvas.SetZIndex(element, 99);

                    foreach (var el in allSin.waves)
                    {
                        if ((int)(Canvas.GetLeft(element) + (element.Width / 2)) == el.x && (int)(Canvas.GetTop(element) + (element.Height / 2)) == el.y)
                        {
                            //Console.WriteLine("found sin " + el.index);
                            //currElem = el.index;
                            usable = el;
                        }
                    }
                    isBeingDragged = true;
                }
            }

            // če je gumb oznanečn odstrani frekvenco
            if ( removeWave == true )
            {
                canvas.Children.Remove(usable.e);
                usable.x = 0;
                usable.y = 0;
                allSin.waves.Remove(usable);
                isBeingDragged = false;
            }

            // nočem da se premika linija
            if (e.OriginalSource is Line)
            {
                isBeingDragged = false;
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        { 
            // nočem da se premika linija
            if (isBeingDragged && !(e.OriginalSource is Line))
            {
                var elementBeingDragged = (FrameworkElement)e.OriginalSource;
                var position = e.GetPosition(canvas);

                // kontrola de ne gre izven območja
                if (position.X + elementBeingDragged.ActualWidth / 2 > canvas.Width - 8)
                {
                    position.X = canvas.Width - elementBeingDragged.ActualWidth / 2 - 9;
                }

                if (position.X - elementBeingDragged.ActualWidth / 2 < 0)
                {
                    position.X = elementBeingDragged.ActualWidth / 2 + 1;
                }

                if (position.Y - elementBeingDragged.ActualHeight / 2 > canvas.Height - 1)
                {
                    position.Y = canvas.Height + elementBeingDragged.ActualHeight / 2 - 2;
                }

                if (position.Y - elementBeingDragged.ActualHeight / 2 < 0)
                {
                    position.Y = elementBeingDragged.ActualHeight / 2 + 1;
                }

                Canvas.SetLeft(elementBeingDragged, position.X - elementBeingDragged.ActualWidth / 2);
                Canvas.SetTop(elementBeingDragged, position.Y - elementBeingDragged.ActualHeight / 2);

                // to bi lahko naredu tako, da bi v "mouseClicked" identificiru in skopiru 
                // element, tle bi samo tistemu elementu prirejal vrednosti
                // to bi uničilo potrebo po "loopanju" skozi vse elemente...

                // neki takega mogoče,... ubistvu dela doro
                usable.x = position.X;
                usable.y = position.Y;

                // prejšnji način
                //foreach (var el in allSin.waves)
                //{
                //    if (el.index == currElem)
                //    {
                //        el.x = position.X;
                //        el.y = position.Y;
                //        //el.GetAmpli();
                //        //el.GetFreq();
                //        //el.collectSamples = true;
                //        //el.fillSamples();
                //        //Console.WriteLine("index sin " + el.index + " x " + el.x + " y " + el.y);
                //    }
                //}
            }
        }
        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // to povrne stanje z indexa na prvotno
            if (!(e.OriginalSource is Canvas))
            {
                var element = (FrameworkElement)e.OriginalSource;
                Canvas.SetZIndex(element, 10);
            }

            isBeingDragged = false;
        }
        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            isBeingDragged = false;
        }
        // kontrole za celotno okno
        // ne pokaže prau natančne frekvence, ma je dovolj blizu
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if ( usable != null )
            {
                txtWaveFreq.Text = usable.displayFreq();
                txtWaveAmpli.Text = usable.displayAmpli();
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (usable != null)
            {
                txtWaveFreq.Text = usable.displayFreq();
                txtWaveAmpli.Text = usable.displayAmpli();
            }
        }
        // kontrole na panelu
        private void btnAddWave_Click(object sender, RoutedEventArgs e)
        {
            if (addWave == true)
            {
                addWave = false;
                btnAddWave.ClearValue(Button.BackgroundProperty);
            }
            else
            {
                addWave = true;
                btnAddWave.Background = Brushes.Aquamarine;
            }
        }
        private void btnRemoveWave_Click(object sender, RoutedEventArgs e)
        {
            if (removeWave == true)
            {
                removeWave = false;
                btnRemoveWave.ClearValue(Button.BackgroundProperty);
            }
            else
            {
                removeWave = true;
                btnRemoveWave.Background = Brushes.Aquamarine;
            }
        }

        // razne pomožne metode
        public void findElements()
        {
            // tako dobiš informacije o elementih na canvasu
            IEnumerable<Ellipse> circles = canvas.Children.OfType<Ellipse>();

            foreach (var cir in circles)
            {
                Console.WriteLine("circ X" + (int)(Canvas.GetLeft(cir) + (cir.Width / 2)) + " Y " + (int)(Canvas.GetTop(cir) + (cir.Height / 2)));
            }
        }
        public void drawElement(string s, double xPos, double yPos, [Optional] double xPos2, [Optional] double yPos2 )
        {
            // nariše element, če bi hotu večjo raznolikost bi še 
            // določu kak element

            switch (s)
            {
                case "line":
                    Line el = new Line();
                    SolidColorBrush Brush = new SolidColorBrush();
                    Brush.Color = Color.FromArgb(255, 0, 0, 0);
                    el.Stroke = Brush;
                    el.X1 = xPos;
                    el.Y1 = yPos;
                    el.X2 = xPos2;
                    el.Y2 = yPos2;
                    el.StrokeThickness = 2;
                    canvas.Children.Add(el);
                    break;

                case "rect":
                    Rectangle el2 = new Rectangle();
                    el2.Width = 50;
                    el2.Height = 50;
                    SolidColorBrush Brush2 = new SolidColorBrush();
                    Brush2.Color = Color.FromArgb(255, 0, 102, 0);
                    el2.Fill = Brush2;
                    Canvas.SetLeft(el2, xPos - el2.Width / 2);
                    Canvas.SetTop(el2, yPos - el2.Height / 2);
                    canvas.Children.Add(el2);
                    break;
                case "circle":
                    Ellipse el3 = new Ellipse();
                    el3.Width = 30;
                    el3.Height = 30;
                    SolidColorBrush Brush3 = new SolidColorBrush();
                    Brush3.Color = Color.FromArgb(255, 0, 102, 0);
                    el3.Fill = Brush3;
                    Canvas.SetLeft(el3, xPos - el3.Width / 2);
                    Canvas.SetTop(el3, yPos - el3.Height / 2);
                    canvas.Children.Add(el3);
                    break;
            }
        }
        public double[] getXYpos(MouseButtonEventArgs ms)
        {
            // to dobi x in y pozicijo na canvasu
            double[] XY = new double[2];
            var p = ms.GetPosition(canvas);
            XY[0] = p.X;
            XY[1] = p.Y;
            Console.WriteLine("x " + XY[0] + " y " + XY[1]);
            return XY;
        }
        public void drawBackground()
        {
            // tametoda mora bit obvezno, ker če ni nobenga elementa na canvasu
            // ne zazna klika in ne vrne pozicije
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(0, 255, 255, 255);

            canvas.Background = mySolidColorBrush;
        }
    }
}
