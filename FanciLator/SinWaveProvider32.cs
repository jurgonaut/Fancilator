using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Windows;

namespace FanciLator
{
    class SinWaveProvider32 : WaveProvider32
    {
        int sample;
        // "count" in "index" se rabijo za hranit zaporedje oz. index 
        // vsakega elementa. To sm rabu za testiranje, bi delalo tudi brez
        static int count;
        public int index;
        public double x;
        // "nX" je začasna "x" spremenljivka, ki se uporabi pri
        // preverjanje ali se je x spremenil od zadnjega izračuna
        public double nX;
        public double y;
        public double nY;
        public Ellipse e;
        Canvas can;
        // maksimalna vrednost herzov - laho bi dal uporabniku možnost spreminjat
        float max = 600;
        // buffer za shranjevat podatke o samplu
        float[] samples = new float[44100];
        // se rabi v kombinacij z zgornjem, za napoliit zbirko 
        // z vrednostmi samplu
        public int samplesPlayed = 0;
        public SinWaveProvider32(double xPos, double yPos, Canvas cv)
        {
            Frequency = 100;
            Amplitude = 0.25f; // let's not hurt our ears    

            // to shrani pozicijo klika na canvas 
            x = xPos;
            y = yPos;

            can = cv;

            // ustvari novo elipso in jo nariče 
            createElement(xPos, yPos, cv);

            // poveča index in ga pripiše temu elementu
            count++;
            index = count;
        }
        public int Frequency { get; set; }
        public float Amplitude { get; set; }

        // ta metoda predvaja ponavljajoči se zvok ozirom sinusoidno frekvenco
        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            // sample count je odvisen od števila bufferjev, skupaj dajo vrednost 13230, baje
            int sampleRate = WaveFormat.SampleRate;

            // ta loop napolni buffer z vrednostmi, ki so tipa float,
            // te vrednosti se pošljejo slušalkam da ga prevajajo

            // preveri če se je pozicija spremenila, če se je
            // ponovno nastavi frekvenco glede na x pozicijo
            if (nX != x)
            {
                Frequency = GetFreq();
                sample = 0;
            }

            // ponovno nastavi amplitudi glede na y pozicijo če je treba
            if ( nY != y )
            {
                Amplitude = GetAmpli();
                sample = 0;
            }

            // s tem while loopom poveš koliko časa se bo zvok predvaju 44100 = 1 sekunda
            // brez tega se predvaja v neskončnost
            //while (samplesPlayed < 88200)
            //{

            for (int n = 0; n < sampleCount; n++)
            {
                // ta formula nardi celotno čarovnijo, ustvari vsak košček in mu doda neko amplitudo
                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
                sample++;

                // to je za shranihevat podatke o samplih
                //if (samplesPlayed < 44100 && collectSamples == true)
                //{
                //    samples[n + offset] = buffer[n + offset];
                //    samplesPlayed++;
                //}
                //if ( samplesPlayed >= 44100 )
                //{
                //    collectSamples = false;
                //}

                //samplesPlayed++;
                if (sample >= sampleRate)
                {
                    sample = 0;
                }
            }
            nX = x;
            nY = y;
            return sampleCount;
            //}
            ////// neki moraš vrnt..
            //return 0;
        }

        public void createElement(double x, double y, Canvas c)
        {
            // ustvari nov element
            e = new Ellipse();
            e.Width = 30;
            e.Height = 30;
            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Color.FromArgb(255, 0, 51, 102);
            e.Fill = Brush;
            Canvas.SetLeft(e, x - e.Width / 2);
            Canvas.SetTop(e, y - e.Height / 2);
            Canvas.SetZIndex(e, 10);
            can.Children.Add(e);
        }

        // metoda, ki pridobi frekvenco na podalgi x poziciije
        public int GetFreq()
        {
            //mogoče bi lakho dal določene spremenljivke ven, da bi blo bolj optimizirano
            float frq = (float)x;

            // razmerje med canvasom in frekvenco
            float frqToCanRatio = frq * 100.0F / (float)can.Width;
            //frq = frq * 100.0F / (float)can.Width;

            // razmerej med canvasom in maksimalno vrednostjo
            float frqToMaxRatio = max / 100 * frqToCanRatio;
            //frq = max / 100 * frq;

            // "(int)" zaokroži in prepreči "klikanje" zvoka
            return (int)frqToMaxRatio;
        }

        // metoda, ki pridobi amplitudo na podlagi y pozicije
        public float GetAmpli()
        {
            float amp = (float)y;

            // dobi razmerje med višino in trenutno y pozicijo elementa
            float ampToCanRatio = amp * 100.0F / (float)can.Width;

            // vrnjeno vrednost še deli z 100, ker mora bit med
            // 1 in -1
            return ampToCanRatio / 100;
        }

        // vrne "string" frekvence za prikazovat
        // uporabniku na oknu
        public string displayFreq()
        {
            return Frequency.ToString();
        }
        // vrne "string" amplitude za prikazovat
        // uporabniku na oknu
        public string displayAmpli()
        {
            double toDisplay = Math.Round(Amplitude, 2);
            toDisplay = toDisplay * 100;
            int toInt = (int)toDisplay;
            return toInt.ToString();
        }

        // nariše krivulju trenutne frekvence
        // s tem v kombinaciji z "samplePlayed" dobiš podatke o 
        // samplih v vrednostih od +1 do -1. Mogoče bi blo 
        // za razvit v ki koristnega
        public void drawSamples()
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            mySolidColorBrush.Color = Color.FromArgb(0, 255, 255, 255);
            can.Background = mySolidColorBrush;

            mySolidColorBrush.Color = Color.FromArgb(100, 100, 100, 0);

            for (var i = 0; i < 602; i++)
            {
                Ellipse p = new Ellipse();
                //Console.WriteLine("float sample " + samplesToDraw[i]);
                float realValue = samples[i + 100] * 1000;

                p.Fill = mySolidColorBrush;
                p.StrokeThickness = 2;
                p.Stroke = Brushes.Black;
                p.Width = 2;
                p.Height = 2;
                Canvas.SetTop(p, can.Height / 2 - realValue);
                Canvas.SetLeft(p, i);
                can.Children.Add(p);
                //Console.WriteLine("samples ratio " + (int)realValue);
                //samplesToDraw[i] * 100.0F / (float)can.Height / 2
            }
        }
    }
}
