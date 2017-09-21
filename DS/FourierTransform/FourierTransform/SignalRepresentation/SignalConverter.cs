using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourierTransform.SignalRepresentation
{
    public class SignalProfile
    {
        public SignalProfile(double zero, double step, double frequency, string xLabel, string yLabel)
        {
            Zero = zero;
            Step = step;
            Frequency = frequency;
            XLabel = xLabel;
            YLabel = yLabel;
        }

        public double Zero { get; private set; }
        public double Step { get; private set; }
        public double Frequency { get; private set; }
        public string XLabel { get; private set; }
        public string YLabel { get; private set; }
    }
    public struct SignalPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public static class SignalConverter
    {
        public static List<SignalPoint> ConvertWithProfile(List<SignalPoint> rawData,
            SignalProfile profile)
        {
            var result = rawData.Select(p => new SignalPoint { X = p.X / profile.Frequency,
                Y = (p.Y - profile.Zero)/profile.Step });
            return result.ToList();
        }
    }
}
