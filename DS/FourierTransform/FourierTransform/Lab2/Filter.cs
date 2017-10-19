using FourierTransform.Fourier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace FourierTransform.Lab2
{
    enum WindowType
    {
        Rectangular,
        Hamming,        
        Hanning,
        Bartlet,
        Blackman
    }
    enum FilterType
    {
        Low,
        High
    }
    static class Filter
    {

         static double[] GenerateWeights(WindowType windowType, int N)
        {
            int M = N - 1;
            switch(windowType)
            {
                case WindowType.Rectangular:
                    return Enumerable.Range(0, N).Select(n => 1.0).ToArray();
                case WindowType.Hamming:
                    return Enumerable.Range(0, N).Select(n => 0.54 - 0.46 * Cos(2 * PI * n / M)).ToArray();
                case WindowType.Hanning:
                    return Enumerable.Range(0, N).Select(n => 0.5 - 0.5 * Cos(2 * PI * n / M)).ToArray();
                case WindowType.Bartlet:
                    return Enumerable.Range(0, N).Select(n => 1 - 2 * Abs(n - M / 2.0) / M).ToArray();
                case WindowType.Blackman:
                    return Enumerable.Range(0, N).Select(n => 0.42 - 0.5 * Cos(2 * PI * n / M)
                            - 0.8 * Cos(4 * PI * n / M)).ToArray();
                default:
                    return null;
            }
        }

        static double[] GenerateH(double fc, int N)
        {
            int M = N - 1;
            return Enumerable.Range(0, N).Select
                ((i) => (i == M/2? 2*fc : Sin(2*PI*fc*(i-M/2))/(PI*(i-M/2)))
                ).ToArray();

        }

        public static (double[] result, double[] simpleAFC, double[] dbAFC) CalculateAFC(double fc, int N, WindowType window)
        {
            double[] testSignal = new double[1<<13];
            testSignal[0] = 1;
            double[] resultSignal = ApplyLowFilter(testSignal, fc, N, window);

            var fft = FourierTransformer.FFT(resultSignal.Select(d => new Complex(d, 0)).ToArray(), false);

            var afc = FourierTransformer.CalculateSpecs(fft);

            var db = afc.amplitudeSpec.Select(d => 20 * Log10(d.Y)).ToArray();

            return (resultSignal, afc.amplitudeSpec.Select(p => p.Y).ToArray(), db);
        }
           

        public static double[] ApplyLowFilter(double[] source, double fc, int N, WindowType window)
        {
            var result = new double[source.Length];
            var w = GenerateWeights(window, N);
            var h = GenerateH(fc, N);
            for (int k = 0; k < source.Length; k++)
            {
                int windowN = N;
                if (k + 1 < N)
                {
                    windowN = k + 1;
                }
                for (int m = 0; m < windowN; m++)
                {
                    result[k] += w[m] * source[k - m] * h[m];
                }
            }
            return result;
        }


        public static double[] ApplyCheb(double[] source, double fc, int N, FilterType type = FilterType.Low)
        {
            var result = new double[source.Length];
            /*
             * y[k]=b0x[k]+ b1x[k-1]+...+ bmx[k-m]
+ a1y[k-1]+ a2y[k-2]+...+ any[k-n] 
             */

            var hc = CreateChebyshevFilter(N, fc, type);
            //for(int i=0; i<N; i++)
            //{
            //    result[i] = source[i];
            //}
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = hc.alpha[0]*source[i];
                int FN = i > N ? N : i;
                for (int k = 1; k < FN; k++)
                {
                    result[i] += hc.alpha[k] * source[i - k] - hc.betta[k] * result[i - k];
                }
            }
            return result;
        }


        public static (double []alpha, double[] betta) CreateChebyshevFilter(int n, double fc, FilterType filterType, int pr = 10)
        {
            
            int N = n + 3;
            double[] a, b, temp_a, temp_b;
            double a0, a1, a2,
                   b1, b2;
            a0 = a1 = a2 = b1 = b2 = 0;
            a = new double[N];
            b = new double[N];
            temp_a = new double[N];
            temp_b = new double[N];

            a[2] = b[2] = 1;

            for (int j = 1; j < n / 2; j++)
            {
                //CalcPole(j)
                CalcPole(n, fc, filterType, pr, ref a0, ref a1, ref a2, ref b1, ref b2, j);

                //
                a.CopyTo(temp_a, 0);
                b.CopyTo(temp_b, 0);
                for (int i = 2; i < N; i++)
                {
                    a[i] = a0 * temp_a[i] + a1 * temp_a[i - 1] + a2 * temp_a[i - 2];
                    b[i] = temp_b[i] - b1 * temp_b[i - 1] - b2 * temp_b[i - 2];
                }
            }

            b[2] = 0;
            for (int i = 0; i < n; i++)
            {
                a[i] = a[i + 2];
                b[i] = b[i + 2];
            }
            // нормировка коэффициента усиления
            ///*
            double SA = 0;
            double SB = 0;
            int k = 0;
            for (int i = 0; i < n; i++)
            {
                if (filterType == FilterType.Low)
                {
                    SA += a[i];
                    SB += b[i];
                }
                else
                {
                    if ((i % 2) == 0)
                        k = 1;
                    else
                        k = -1;
                    SA += a[i] * k;
                    SB += b[i] * k;
                }
            }
            double GAIN = SA / (1 - SB);
            for (int i = 0; i < n; i++)
            {
                a[i] = a[i] / GAIN;
            }
            //*/
            return (a, b);
        }

        private static void CalcPole(int n, double fc, FilterType filterType, double pr, ref double a0, ref double a1, ref double a2, ref double b1, ref double b2, int j)
        {
            double arsh(double x) => Log(x + Sqrt(x * x + 1));


            #region bullshit
            ///*
            double phase = (2*j-1)*PI/(2*n);//PI / (2 * n) + (j - 1) * PI / n;
            Complex p = new Complex(Sin(phase), -Cos(phase));
            
            if (pr > 0)
            {
                double es = Sqrt(100 / (Pow(100.0 - pr, 2) - 1));
                double argh = 1.0 / n * Log((1.0 / es) + Sqrt(1 / (es * es) + 1));

                p = p * Sinh(argh) / Cosh(argh);

                // преобразование аналоговой области в цифровую
                double t = 2 * Tan(fc);
                double w = 2 * PI * fc;
                //m = rp * rp + ip * ip;
                double m = p.Real * p.Real + p.Imaginary * p.Imaginary ;
                double d = 4 - 4 * p.Real * t + m * t * t;
                double x0 = t * t / d;
                double x1 = 2 * t * t / d;
                double x2 = t * t / d;
                double y1 = (8 - 2 * m * t * t) / d;
                double y2 = (-4 - 4 * p.Real * t - m * t * t) / d;
                double k;
                if (filterType == FilterType.High)
                {
                    k = -Cos(w / 2.0 + fc) / Cos(w / 2.0 - fc);
                }
                else
                {
                    k = Sin(fc - w / 2.0) / Sin(w / 2.0 + fc);
                }
                d = 1 + y1 * k - y2 * k * k;
                a0 = (x0 - x1 * k - x2 * k * k) / d;
                a1 = (-2 * x0 * k + x1 + x1 * k * k - 2 * x2 * k) / d;
                a2 = (x0 * k * k - x1 * k + x2) / d;
                b1 = (2 * k + y1 + y1 * k * k - 2 * y2 * k) / d;
                b2 = (-(k * k) - y1 * k + y2) / d;
                if (filterType == FilterType.High)
                {
                    a1 = -a1;
                    b1 = -b1;
                }
            }
            //*/
#endregion

        }
    }
}
