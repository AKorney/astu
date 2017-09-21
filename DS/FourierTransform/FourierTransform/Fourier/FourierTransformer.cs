using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using FourierTransform.SignalRepresentation;

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
            var ab = CalculateAB(source);
            for (int i = 0; i < source.Count; i++)
            {
                amplitude.Add(new SignalPoint { X = i * freqStep, Y = Sqrt(ab.a[i] * ab.a[i] + ab.b[i] * ab.b[i]) });
                phase.Add(new SignalPoint { X = i * freqStep, Y = Atan2(ab.b[i], ab.a[i]) });
            }
            
            return (amplitude, phase);
        }

        public static List<SignalPoint> InverseTransform((double[] a, double[] b) transformData)
        {
            List<SignalPoint> result = new List<SignalPoint>();
            int N = transformData.a.Length;


            for (int k = 0; k < N; k++)
            {
                double fk = 0;
                for (int i = 0; i < N; i++)
                {
                    fk += transformData.a[i] * Cos((2 * PI * i * k) / N)
                        + transformData.b[i] * Sin((2 * PI * i * k) / N);
                }
                result.Add(new SignalPoint { X = 1.0 * k / 360, Y = fk });
            }

            return result;
        }

        public static (double[] a, double[] b) CalculateAB(List<SignalPoint> source)
        {
            int N = source.Count;
            double[] a = new double[N];
            double[] b = new double[N];
            for (int k = 0; k < N; k++)
            {
                for (int i = 0; i < N; i++)
                {
                    a[k] += source[i].Y * Cos((2 * PI * k * i) / N);
                    b[k] += source[i].Y * Sin((2 * PI * i * k) / N);
                }
                a[k] /= N;
                b[k] /= N;
            }
            return (a, b);


        }
    }
}
