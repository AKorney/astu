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
            var binarized = accumulator.ThresholdBinary(new Gray(100), new Gray(255));
            binarized.Save("bin.jpg");
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
            masked.Save("masked.jpg");
            filled.Save("filled.jpg");

            Image<Gray, Byte> gsImg = new Image<Gray, byte>(img.Size);
            var resultRaw = img.And(masked);
            resultRaw.ROI = rect;
            var result = new Image<Gray, Byte>(resultRaw.Size);
            CvInvoke.cvCopy(resultRaw, result, IntPtr.Zero);
            result = result.Resize(desiredSize.Width, desiredSize.Height, Inter.Linear);
            result.Save("found.jpg");
            return result;
        }
    }
}
