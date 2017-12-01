using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace FourierTransform.Wavelets
{
    public  class WaveletTransformer
    {
        public  double[] ForwardHaar(double[] a, int scaleFactor)
        {
            int n = a.Length;
            int stop = 1 << scaleFactor;
            while (n > stop)
            {
                int half = n >> 1;
                double[] sum = new double[half];
                double[] diff = new double[half];
                for (int i = 0; i < half; i++)
                {
                    sum[i] = (a[i * 2] + a[i * 2 + 1]) / 2.0;
                    diff[i] = (a[i * 2] - a[i * 2 + 1]) / 2.0;
                }
                for (int i = 0; i < half; i++)
                {
                    a[i] = sum[i];
                    a[i + half] = diff[i];
                }
                n = n >> 1;
            }
            return a;
        }
        public  double[] InverseHaar(double[] a, int scaleFactor)
        {
            int n = 2 * (1 << scaleFactor);
            while (n <= a.Length)
            {
                int half = n >> 1;
                double[] sum = new double[half];
                double[] diff = new double[half];
                for (int i = 0; i < half; i++)
                {
                    sum[i] = a[i];
                    diff[i] = a[i + half];
                }
                for (int i = 0; i < half; i++)
                {
                    a[2 * i] = (sum[i] + diff[i]);
                    a[2 * i + 1] = (sum[i] - diff[i]);
                }
                n <<= 1;
            }
            return a;
        }
        

        private double h1 => (3 + Sqrt(3)) / (4 * Sqrt(2));
        private double h2 => (3 - Sqrt(3)) / (4 * Sqrt(2));
        private double h3 => (1 - Sqrt(3)) / (4 * Sqrt(2));
        private double h0 => (1 + Sqrt(3)) / (4 * Sqrt(2));

        private double g0 => h3;
        private double g1 => -h2;
        private double g2 => h1;
        private double g3 => -h0;

        private double invH0 => h2;
        private double invH1 => g2;  // h1
        private double invH2 => h0;
        private double invH3 => g0;  // h3

        private double invG0 => h3;
        private double invG1 => g3;  // -h0
        private double invG2 => h1;
        private double invG3 => g1;  // -h2

        public double[] ForwardDaubechies(double[] a, int scaleFactor)
        {
            int N = a.Length;
            int n;
            for (n = N; n >= 4 * (1 << scaleFactor); n >>= 1)
            {

                if (n >= 4)
                {
                    int i, j;
                    int half = n >> 1;

                    double[] tmp = new double[n];

                    i = 0;
                    for (j = 0; j < n - 3; j = j + 2)
                    {
                        tmp[i] = a[j] * h0 + a[j + 1] * h1 + a[j + 2] * h2 + a[j + 3] * h3;
                        tmp[i + half] = a[j] * g0 + a[j + 1] * g1 + a[j + 2] * g2 + a[j + 3] * g3;
                        i++;
                    }

                    tmp[i] = a[n - 2] * h0 + a[n - 1] * h1 + a[0] * h2 + a[1] * h3;
                    tmp[i + half] = a[n - 2] * g0 + a[n - 1] * g1 + a[0] * g2 + a[1] * g3;

                    for (i = 0; i < n; i++)
                    {
                        a[i] = tmp[i];
                    }
                }
            }
            return a;
        }

        public double[] HighPassFilterDaubechies(double[] a, int segmentGranulation)
        {
            int position = 1 << segmentGranulation;
            var direct = ForwardDaubechies(a, 0);
            for (int i = 0; i < position; i++) direct[i] = 0;
            return InverseDaubechies(direct, 0);
        }
        public double[] LowPassFilterDaubechies(double[] a, int segmentGranulation)
        {
            int position = 1 << segmentGranulation;
            var direct = ForwardDaubechies(a, 0);
            for (int i = position; i < direct.Length; i++) direct[i] = 0;
            return InverseDaubechies(direct, 0);
        }
        public double[] HighPassFilterHaar(double[] a, int segmentGranulation)
        {
            int position = 1 << segmentGranulation;
            var direct = ForwardHaar(a, 0);
            for (int i = 0; i < position; i++) direct[i] = 0;
            return InverseHaar(direct, 0);
        }
        public double[] LowPassFilterHaar(double[] a, int segmentGranulation)
        {
            int position = 1 << segmentGranulation;
            var direct = ForwardHaar(a, 0);
            for (int i = position; i < direct.Length; i++) direct[i] = 0;
            return InverseHaar(direct, 0);
        }

        public double[] InverseDaubechies(double[] a, int scaleFactor)
        {
            int N = a.Length;
            int n;
            for (n = 4 * (1 << scaleFactor); n <= N; n <<= 1)
            {
                int i, j;
                int half = n >> 1;
                int halfPls1 = half + 1;

                double[] tmp = new double[n];

                tmp[0] = a[half - 1] * invH0 + a[n - 1] * invH1 + a[0] * invH2 + a[half] * invH3;
                tmp[1] = a[half - 1] * invG0 + a[n - 1] * invG1 + a[0] * invG2 + a[half] * invG3;
                j = 2;
                for (i = 0; i < half - 1; i++)
                {
                    tmp[j++] = a[i] * invH0 + a[i + half] * invH1 + a[i + 1] * invH2 + a[i + halfPls1] * invH3;
                    tmp[j++] = a[i] * invG0 + a[i + half] * invG1 + a[i + 1] * invG2 + a[i + halfPls1] * invG3;
                }
                for (i = 0; i < n; i++)
                {
                    a[i] = tmp[i];
                }
            }
            return a;
        }
    }
}
