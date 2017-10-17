using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using FourierTransform.SignalRepresentation;
using System.Numerics;

namespace FourierTransform.Fourier
{


    public static class FourierTransformer
    {


        public static (List<SignalPoint> amplitudeSpec, List<SignalPoint> phaseSpec)
            CalculateSpecs(Complex[] fourier)
        {
            List<SignalPoint> 
                amplitude = new List<SignalPoint>(), phase = new List<SignalPoint>();

            double freqStep = 360.0 / fourier.Length;
            

            for (int i = 0; i < fourier.Length/2; i++)
            {
                amplitude.Add(new SignalPoint { X = i * freqStep, Y = fourier[i].Magnitude});
                phase.Add(new SignalPoint { X = i * freqStep, Y = fourier[i].Phase });
            }
            
            return (amplitude, phase);
        }


        public static Complex[] DFT(Complex[] x, bool invert)
        {
            int N = x.Length;

            Complex[] X = new Complex[N];
            for (int k = 0; k < N; k++)
            {
                X[k] = new Complex(0, 0);
                for (int n = 0; n < N; n++)
                {
                    Complex temp = Complex.FromPolarCoordinates(1, 2 * Math.PI * n * k * (invert?1:-1) / N);
                    temp *= x[n];
                    temp /= invert ? 1 : N;
                    X[k] += temp;
                }
            }
            return X;
        }

        public static Complex[] FFT(Complex[] x, bool inverse, bool useRecursion = false)
        {
            var fft = useRecursion? RFFT(x,inverse) : IFFT(x,inverse);
            var result = fft.Select(c => c / (inverse ? 1 : fft.Length)).ToArray();
            return result;
        }

        internal static int Reverse(int x, int n)
        {
            int b = 0;
            int i = 0;
            while (x != 0)
            {
                b <<= 1;
                b |= (x & 1);
                x >>= 1;
                i++;
            }
            if (i < n)
                b <<= n - i;
            return b;
        }

        public static Complex[] IFFT(Complex[] x, bool inverse)
        {
            int N = x.Length;
            Complex[] X = new Complex[N];
            int M = (int)Log(N, 2);
            for (int i = 0; i < N; i++)
            {
                int k = Reverse(i, M);
                X[k] = x[i];
            }

            for (int m = 2; m <= N; m<<=1)
            {
                Complex Wm = Complex.FromPolarCoordinates(1, -2 * PI / m * (inverse?1:-1));
                for (int k = 0; k < N; k+=m)
                {
                    Complex W = 1;
                    for (int j = 0; j <= m/2-1; j++)
                    {
                        Complex t, u;
                        t = X[k + j + m / 2] * W;
                        u = X[k + j];
                        X[k + j] = t + u;
                        X[k + j + m / 2] = u - t;
                        W *= Wm;
                    }
                }
            }
            return X;
        }


        public static Complex[] FFTn(Complex[] x, int M, bool inverse)
        {
            int N = x.Length;
            //М - шаг разбиения
            Complex[] fft = new Complex[N];
            Complex[] X = new Complex[N];
            int L =  N / M;
            int p = (int)Log(L, 2);
            //подготовили бпф по наборам
            for (int z = 0; z < M; z++)
            {
                Complex[] range = x.Where((c, i) => (i - z) % M == 0).ToArray();
                var fourier = IFFT(range, inverse);
                for (int l = 0; l < L; l++)
                {
                    fft[z + M * l] = fourier[l];
                }
            }

            for (int s = 0; s < M; s++)
            {
                for (int r = 0; r < L; r++)
                {
                    X[r + s * L] = 0;
                    for (int m = 0; m < M; m++)
                    {
                        X[r + s * L] += fft[m + r * M] * 
                            Complex.FromPolarCoordinates(1, -2 * PI * m * (r + s * L) / N);
                    }
                }
            }
            return X.Select(c=>c/(inverse?1:N)).ToArray();
        }

        public static Complex[] RFFT(Complex[] x, bool inverse)
        {
            int N = x.Length;
            Complex[] C = new Complex[N];
            Complex[] F1, C1, F0, C0;
            if (N == 1)
            {
                C[0] = x[0];
                return C;
            }
            int k;
            F0 = x.Where((c, i) => i % 2 == 0).ToArray();
            F1 = x.Where((c, i) => i % 2 != 0).ToArray();

            C1 = RFFT(F1, inverse);
            C0 = RFFT(F0, inverse);

            for (k = 0; k < N / 2; k++)
            {
                Complex W = Complex.FromPolarCoordinates(1, -2 * Math.PI * k / N * (inverse?1:-1));
                C[k] = C0[k] + C1[k]*W;
                C[k + N / 2] = C0[k] - C1[k]*W;
            }
            return C;
        }


        public static Complex[] HFF(Complex[] source, double freq, double threshold)
        {
            Complex[] temp = new Complex[source.Length];
            source.CopyTo(temp, 0);

            double hc = freq / source.Length;
            int targetIndex = (int)(threshold / hc);

            for (int i = 0; i < targetIndex; i++)
            {
                temp[i] = temp[temp.Length - i - 1] = 0;
            }

            return FFT(temp, true);
        }

        public static Complex[] LFF(Complex[] source, double freq, double threshold)
        {
            Complex[] temp = new Complex[source.Length];
            source.CopyTo(temp, 0);

            double hc = freq / source.Length;
            int targetIndex = (int)(threshold / hc);

            for (int i = targetIndex; i < source.Length-targetIndex; i++)
            {
                temp[i] = temp[temp.Length - i - 1] = 0;
            }

            return FFT(temp, true);
        }
    }
}
