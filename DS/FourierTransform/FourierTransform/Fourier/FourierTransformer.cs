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
    public enum FourierAlgType
    {
        DFT,
        FFT
    }

    public static class FourierTransformer
    {


        public static (List<SignalPoint> amplitudeSpec, List<SignalPoint> phaseSpec)
            ApplyDiscreteTransform(List<SignalPoint> source, FourierAlgType alg)
        {
            List<SignalPoint> 
                amplitude = new List<SignalPoint>(), phase = new List<SignalPoint>();

            double freqStep = 360.0 / source.Count;
            var complex_signal = source.Select(p => new Complex(p.Y, 0)).ToArray();
            Complex[] result = new Complex[0];
            switch (alg)
            {
                case FourierAlgType.DFT:
                    result = DFT(complex_signal, false);
                    break;
                case FourierAlgType.FFT:
                    result = FFT(complex_signal, false);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < source.Count; i++)
            {
                amplitude.Add(new SignalPoint { X = i * freqStep, Y = result[i].Magnitude});
                phase.Add(new SignalPoint { X = i * freqStep, Y = result[i].Phase });
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
        public static Complex[] FFT(Complex[] x, bool inverse)
        {
            var fft = RunFFT(x);
            var result = fft.Select(c => c / (inverse ? 1 : fft.Length)).ToArray();
            return result;
        }
        internal static Complex[] RunFFT(Complex[] x)
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

            C1 = RunFFT(F1);
            C0 = RunFFT(F0);

            for (k = 0; k < N / 2; k++)
            {
                Complex W = Complex.FromPolarCoordinates(1, -2 * Math.PI * k / N);
                C[k] = C0[k] + C1[k]*W;
                C[k + N / 2] = C0[k] - C1[k]*W;
            }
            return C;
        }

    }
}
