using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.CvEnum;

namespace TrafficSign.Detection.Red
{
    public class RedCirclesDetector : Common.DetectorBase
    {
        public override Image<Gray, byte> FilterColorChannel(Image<Rgb, byte> source)
        {
            return base.FilterColorChannel(source, 0);
        }

        public override Image<Gray, byte> FindObjectOfInterest(Image<Gray, byte> img, Image<Gray, byte> accumulator, Size desiredSize)
        {
            var binarized = new Image<Gray, byte>(img.Size);
            CvInvoke.Threshold(accumulator, binarized, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);
            //binarized.Save("bin.jpg");
            //binarized.Save(string.Format("{0}{1}.jpg", this.GetType().Name, "bin"));
            Image<Gray, Byte> filled = new Image<Gray, byte>(binarized.Size);
            var cpmsk = new Image<Gray, Byte>(binarized.Cols, binarized.Rows, new Gray(255));
            CvInvoke.cvCopy(binarized, filled, cpmsk);
            Image<Gray, byte> mask = new Image<Gray, byte>(filled.Width + 2, filled.Height + 2);
            MCvConnectedComp comp = new MCvConnectedComp();
            Point point1 = new Point(filled.Cols / 2, filled.Rows / 2);
            Rectangle rect;
            CvInvoke.FloodFill(filled, mask, point1, new MCvScalar(255),
                out rect,
            new MCvScalar(0),
            new MCvScalar(0));
            var masked = filled.Xor(binarized);
            //masked.Save("masked.jpg");
            //filled.Save("filled.jpg");
           // masked.Save(string.Format("{0}{1}.jpg", this.GetType().Name, "mas"));
           // filled.Save(string.Format("{0}{1}.jpg", this.GetType().Name, "fil"));
            Image<Gray, Byte> gsImg = new Image<Gray, byte>(img.Size);
            var resultRaw = img.And(masked);
            resultRaw.ROI = rect;
            var result = new Image<Gray, Byte>(resultRaw.Size);
            CvInvoke.cvCopy(resultRaw, result, IntPtr.Zero);
            result = result.Resize(desiredSize.Width, desiredSize.Height, Inter.Linear);
            //result.Save("found.jpg");
           // result.Save(string.Format("{0}{1}.jpg", this.GetType().Name, "res"));
            return result;
        }

        public override void ThresholdDetection(Image<Gray, byte> img, Image<Gray, byte> accumulator)
        {
            UMat uimage = img.ToUMat();
            Image<Gray, Byte> filterBuffer = new Image<Gray, byte>(img.Size);


            int threshmin = 80, threshmax = 180, step = 20;
            double alpha = (double)step / (threshmax - threshmin);
            for (int t = threshmin; t <= threshmax; t += step)
            {
                CvInvoke.Threshold(uimage, filterBuffer, t, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                CvInvoke.AddWeighted(filterBuffer, alpha, accumulator, 1, 0, accumulator);
            }
            CvInvoke.PyrDown(accumulator, filterBuffer);
            CvInvoke.PyrUp(filterBuffer, accumulator);
            CvInvoke.Threshold(accumulator, accumulator, 100, 255, ThresholdType.ToZero);
            //accumulator.Save(string.Format("{0}{1}.jpg", this.GetType().Name, "accum"));

        }
    }
}
