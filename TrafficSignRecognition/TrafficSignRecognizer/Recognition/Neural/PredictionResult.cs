using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignRecognizer.Recognition.Neural
{
    public class PredictionResult
    {
        public int ClassId { get; private set; }
        public Rectangle ROI { get; private set; }
        public PredictionResult(double[] answer, Rectangle roi)
        {
            ROI = roi;
            double max = double.MinValue;
            int maxIdx = -1;
            for (int i = 0; i < answer.Length; i++)
            {
                if(answer[i]>max)
                {
                    max = answer[i];
                    maxIdx = i;
                } 
            }
            ClassId = maxIdx + 1;
        }
    }
}
