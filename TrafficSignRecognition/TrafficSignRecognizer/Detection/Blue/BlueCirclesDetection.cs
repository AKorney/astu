using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace TrafficSign.Detection.Blue
{
    public class BlueCirclesDetection : Common.DetectorBase
    {
        public override Image<Gray, byte> FilterColorChannel(Image<Rgb, byte> source)
        {
            return base.FilterColorChannel(source, 2);
        }
        public override void ThresholdDetection(Image<Gray, byte> img, Image<Gray, byte> accumulator)
        {
            UMat uimage = img.ToUMat();
            Image<Gray, Byte> filterBuffer = new Image<Gray, byte>(img.Size);


            int threshmin = 60, threshmax = 180, step = 10;
            double alpha = (double)step / (threshmax - threshmin);
            for (int t = threshmin; t <= threshmax; t += step)
            {
                CvInvoke.Threshold(uimage, filterBuffer, t, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                CvInvoke.AddWeighted(filterBuffer, alpha, accumulator, 1, 0, accumulator);
            }
            CvInvoke.PyrDown(accumulator, filterBuffer);
            CvInvoke.PyrUp(filterBuffer, accumulator);
            CvInvoke.Threshold(accumulator, accumulator, 100, 255, ThresholdType.ToZero);


        }
        public override Image<Gray, byte> FindObjectOfInterest(Image<Gray, byte> img, Image<Gray, byte> accumulator, Size desiredSize)
        {
            var binarized = new Image<Gray, byte>(img.Size);
            CvInvoke.Threshold(accumulator, binarized, 0, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);
                //accumulator.ThresholdBinaryInv(new Gray(100), new Gray(255));
            //binarized.Save("bin.jpg");
            Image<Gray, Byte> filled = new Image<Gray, byte>(binarized.Size);
            var cpmsk = new Image<Gray, Byte>(binarized.Cols, binarized.Rows, new Gray(255));
            CvInvoke.cvCopy(binarized, filled, cpmsk);
            Image<Gray, byte> mask = new Image<Gray, byte>(filled.Width + 2, filled.Height + 2);

            Point point1 = new Point(filled.Cols / 2, filled.Rows / 2);
            while(binarized.Data[point1.Y, point1.X, 0] != 0 && point1.Y > 2 && point1.X < filled.Width-2)
            {
                point1.X += 2;
                point1.Y -= 2;
            }
            if(binarized.Data[point1.Y, point1.X, 0] !=0)
            {
                return img;
            }
            Rectangle rect;
            CvInvoke.FloodFill(filled, mask, point1, new MCvScalar(255),
                out rect,
            new MCvScalar(0),
            new MCvScalar(0));
            
            img.ROI = rect;
            var result = new Image<Gray, Byte>(img.Size);
            CvInvoke.cvCopy(img, result, IntPtr.Zero);
            result = result.Resize(desiredSize.Width, desiredSize.Height, Inter.Linear);
            //result.Save("found.jpg");
            return result;
        }
    }
}
