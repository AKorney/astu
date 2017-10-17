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

    }
}
