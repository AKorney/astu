using FourierTransform.Fourier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            //y[k] = b0x[k] + b1x[k - 1] + b2x[k - 2] + 
            //    a1y[k - 1] + a2y[k - 2]

            for (int i = N; i < source.Length; i++)
            {
                result[i] = hc.alpha[0]*source[i];
                //int FN = i > N ? N : i;
                for (int k = 1; k <= N; k++)
                {
                    result[i] += hc.alpha[k] * source[i - k] + hc.betta[k] * result[i - k];
                }
            }
            return result;
        }


        public static (double []alpha, double[] betta) CreateChebyshevFilter(int n, double fc, FilterType filterType, int pr = 10)
        {
            // NP количество звеньев (полюсов) 2 – 20 (четное)
            Debug.Assert(n%2==0);
            int NP = n;
            int N = n + 2;//extra space for shift
            double[] a, b, ta, tb;
            double a0;
            double a1;
            double a2;
            double b1;
            double b2;
            a = new double[N+1];
            b = new double[N+1];
            ta = new double[N+1];
            tb = new double[N+1];

            a[2] = b[2] = 1;

            for (int j = 1; j <= NP / 2; j++)
            {
                //CalcPole(j)
                (a0,a1,a2,b1,b2) = CalcPole(NP, fc, filterType, j, pr);
                
                //
                a.CopyTo(ta, 0);
                b.CopyTo(tb, 0);
                for (int i = 2; i <= N; i++)
                {
                    a[i] = a0 * ta[i] + a1 * ta[i - 1] + a2 * ta[i - 2];
                    b[i] = tb[i] - b1 * tb[i - 1] - b2 * tb[i - 2];
                }
            }

            b[2] = 0;
            for (int i = 0; i <= n; i++)
            {
                a[i] = a[i + 2];
                b[i] = -b[i + 2];
            }
            // нормировка коэффициента усиления
            ///*
            double SA = 0;
            double SB = 0;
            int k = 0;
            for (int i = 0; i <= n; i++)
            {
                if (filterType == FilterType.Low)
                {
                    SA += a[i];
                    SB += b[i];
                }
                else
                {
                    k = ((i % 2) == 0)?1:-1;
                    SA += a[i] * k;
                    SB += b[i] * k;
                }
            }
            double GAIN = SA / (1 - SB);
            for (int i = 0; i <= n; i++)
            {
                a[i] = a[i] / GAIN;
            }
            //*/
            return (a, b);
        }

        private static (double a0,double a1,double a2,double b1,double b2) 
            CalcPole(int NP, double fc, FilterType filterType,int p,int pr)
        {
            //double arsh(double x) => Log(x + Sqrt(x * x + 1));
            double sqr(double x) => x * x;

            
            ///*
            double phase = PI/2/NP + (p-1)*PI/NP;//PI / (2 * n) + (j - 1) * PI / n;
            double rp = -Cos(phase);
            double ip = Sin(phase);

            if (pr != 0)
            {
                double es = Sqrt(sqr(100.0 / (100.0 - pr)) - 1);
                double vx = 1.0 / NP * Log(1.0 / es + Sqrt(1 / sqr(es) + 1));
                double kx = 1.0 / NP * Log(1.0 / es + Sqrt(1 / sqr(es) - 1));
                kx = (Exp(kx) + Exp(-kx)) / 2;
                //p = p * Sinh(vx) / Cosh(vx);
                rp = rp * (Exp(vx) - Exp(-vx)) / 2 / kx;
                ip = ip * (Exp(vx) + Exp(-vx)) / 2 / kx;
            }
            // преобразование аналоговой области в цифровую
            //Debug.Assert(fc == 0.5);
            double t = 2 * Tan(0.5);
            double w = 2 * PI * fc;
            //m = rp * rp + ip * ip;
            double m = sqr(rp) + sqr(ip);
            double d = 4 - 4 * rp * t + m * sqr(t);
            double x0 = sqr(t) / d;
            double x1 = 2 * sqr(t) / d;
            double x2 = sqr(t) / d;
            double y1 = (8 - 2 * m * sqr(t)) / d;
            double y2 = (-4 - 4 * rp * t - m * sqr(t)) / d;
            double k;
            if (filterType == FilterType.High)
            {
                k = -Cos(w / 2.0 + 0.5) / Cos(w / 2.0 - 0.5);
            }
            else
            {
                k = Sin(0.5 - w / 2.0) / Sin(0.5 + w / 2.0);
            }
            d = 1 + y1 * k - y2 * sqr(k);
            double a0 = (x0 - x1 * k + x2 * sqr(k)) / d;
            double a1 = (-2 * x0 * k + x1 + x1 * sqr(k) - 2 * x2 * k) / d;
            double a2 = (x0 * sqr(k) - x1 * k + x2) / d;
            double b1 = (2 * k + y1 + y1 * sqr(k) - 2 * y2 * k) / d;
            double b2 = (-sqr(k) - y1 * k + y2) / d;
            if (filterType == FilterType.High)
            {
                a1 = -a1;
                b1 = -b1;
            }
            return (a0, a1, a2, b1, b2);
            //}
            //return (0, 0, 0, 0, 0);
            //*/


        }
    }
}
