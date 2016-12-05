using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.Drawing;
using Emgu.CV.Features2D;
using System.Diagnostics;

namespace TrafficSignRecognition.CV
{
    internal static class SignDetectionHelper
    {
        public static Image<Gray, Byte> GrayScaleByChannel(Image<Rgb, Byte> source, int channelIndex)
        {
            Stopwatch watch = Stopwatch.StartNew();
            
            Image<Gray, Byte>[] channels = source.Split();
            Image<Gray, Byte> accumulator = new Image<Gray, byte>(source.Width, source.Height, new Gray(0));
            Image<Gray, Byte> result = new Image<Gray, byte>(source.Size);
            CvInvoke.AddWeighted(channels[0], 0.333, accumulator, 1, 1, accumulator);
            CvInvoke.AddWeighted(channels[1], 0.333, accumulator, 1, 1, accumulator);
            CvInvoke.AddWeighted(channels[2], 0.333, accumulator, 1, 1, accumulator);

            CvInvoke.Divide(channels[channelIndex], accumulator, result, 255/3.0);
            System.Diagnostics.Debug.WriteLine("Time color filtering: {0}ms", watch.ElapsedMilliseconds);
            return result;
        }

      
        public static List<Rectangle> DetectEdges(Image<Gray, Byte> img,  Image<Gray, byte> accumulator)
        {

            double cannyThreshold = 180;
            double cannyThresholdLinking = 120;

            Stopwatch watch = Stopwatch.StartNew();
            //Image<Gray, Byte> accumulator = new Image<Gray, byte>(img.Width, img.Height, new Gray(0));

            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Init {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();

            ThresholdDetection(img, accumulator);

            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Thresholding: {0}ms", watch.ElapsedMilliseconds);
            accumulator.Save("accum.jpg");
            watch = Stopwatch.StartNew();

            List<Rectangle> rois = new List<Rectangle>();

            UMat cannyEdges = FindROIs(cannyThreshold, cannyThresholdLinking, accumulator, rois);
            watch.Stop();
            cannyEdges.Save("canny.jpg");
            System.Diagnostics.Debug.WriteLine("Canny: {0}ms", watch.ElapsedMilliseconds);
            return rois;
        }

        internal static Image<Gray, Byte> FindObjectOfInterest(Image<Gray, byte> img, Image<Gray, byte> accumulator)
        {
            var binarized = accumulator.ThresholdBinary(new Gray(100), new Gray(255));
            binarized.Save("bin.jpg");
            Image<Gray, Byte> filled = new Image<Gray, byte>(binarized.Size);
            var cpmsk = new Image<Gray, Byte>(binarized.Cols, binarized.Rows, new Gray(255));
            CvInvoke.cvCopy(binarized, filled, cpmsk);
            Image<Gray, byte> mask = new Image<Gray, byte>(filled.Width + 2, filled.Height + 2);
            MCvConnectedComp comp = new MCvConnectedComp();
            Point point1 = new Point(filled.Cols/2, filled.Rows/2);
            //CvInvoke.cvFloodFill(
            Rectangle rect;
            CvInvoke.FloodFill(filled, mask, point1, new MCvScalar(255),
                out rect,
            new MCvScalar(0),
            new MCvScalar(0));//, out comp,
            //Emgu.CV.CvEnum.Connectivity.EightConnected,
            //Emgu.CV.CvEnum.FloodFillType.Default, mask.Ptr);
            var masked = filled.Xor(binarized);
            masked.Save("masked.jpg");
            filled.Save("filled.jpg");

            Image<Gray, Byte> gsImg = new Image<Gray, byte>(img.Size);
            //gsImg = img.Convert<Gray, Byte>();
            var resultRaw = img.And(masked);
            resultRaw.ROI = rect;
            var result = new Image<Gray, Byte>(resultRaw.Size);
            CvInvoke.cvCopy(resultRaw, result, IntPtr.Zero);
            result = result.Resize(40, 40, Inter.Linear);
            result.Save("found.jpg");
            return result;
        }

        private static UMat FindROIs( double cannyThreshold, double cannyThresholdLinking, Image<Gray, byte> accumulator, List<Rectangle> rois)
        {
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(accumulator, cannyEdges, cannyThreshold, cannyThresholdLinking);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                Mat hierarchy = new Mat();
                CvInvoke.FindContours(cannyEdges, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    var rectangle = CvInvoke.BoundingRectangle(contours[i]);
                    double ratio = rectangle.Size.Width / rectangle.Size.Height;
                    if (rectangle.Size.Width > 25 && rectangle.Size.Height > 25
                        && rectangle.Size.Height < 100 && rectangle.Size.Width < 100
                        && ratio > 0.8 && ratio < 1.25)
                        //CvInvoke.Rectangle(toMark, rectangle, new MCvScalar(255, 255, 0));
                        rois.Add(rectangle);
                }
            }

            return cannyEdges;
        }

        public static void ThresholdDetection(Image<Gray, byte> img, Image<Gray, byte> accumulator)
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
        }
    }
}
