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
            ApplyDiscreteTransform(List<SignalPoint> source)
        {
            List<SignalPoint> 
                amplitude = new List<SignalPoint>(), phase = new List<SignalPoint>();

            double freqStep = 360.0 / source.Count;
            var complex_signal = source.Select(p => new Complex(p.Y, 0)).ToArray();
            var dft = DFT(complex_signal, false);

            for (int i = 0; i < source.Count; i++)
            {
                amplitude.Add(new SignalPoint { X = i * freqStep, Y = dft[i].Magnitude});
                phase.Add(new SignalPoint { X = i * freqStep, Y = dft[i].Phase });
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

        
    }
}
