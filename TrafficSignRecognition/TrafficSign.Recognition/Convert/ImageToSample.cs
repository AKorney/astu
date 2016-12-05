using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSign.Recognition.Convertion
{
    public class ImageToSample
    {
        public static double[] ConvertImageToSample(Image<Gray,Byte> source)
        {
            double[] sample = new double[source.Height * source.Width];
            int idx = 0;
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    sample[idx++] = Convert.ToDouble(source.Data[i, j, 0]);
                }
            }
            return sample;
        }
    }
}
