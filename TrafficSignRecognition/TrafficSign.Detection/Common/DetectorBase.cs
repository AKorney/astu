using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSign.Detection.Common
{
    public abstract class DetectorBase
    {
        public abstract Image<Gray, Byte> FilterColorChannel(Image<Rgb, Byte> source);
        protected Image<Gray, Byte> FilterColorChannel(Image<Rgb, Byte> source, int channelIndex)
        {
            Stopwatch watch = Stopwatch.StartNew();

            Image<Gray, Byte>[] channels = source.Split();
            Image<Gray, Byte> accumulator = new Image<Gray, byte>(source.Width, source.Height, new Gray(0));
            Image<Gray, Byte> result = new Image<Gray, byte>(source.Size);
            CvInvoke.AddWeighted(channels[0], 0.333, accumulator, 1, 1, accumulator);
            CvInvoke.AddWeighted(channels[1], 0.333, accumulator, 1, 1, accumulator);
            CvInvoke.AddWeighted(channels[2], 0.333, accumulator, 1, 1, accumulator);

            CvInvoke.Divide(channels[channelIndex], accumulator, result, 255 / 3.0);
            System.Diagnostics.Debug.WriteLine("Time color filtering: {0}ms", watch.ElapsedMilliseconds);
            return result;
        }

       public void Highlight(List<Rectangle> ROIs, Image<Rgb, byte> target)
        {
            foreach (var roi in ROIs)
            {
                CvInvoke.Rectangle(target, roi, new MCvScalar(255, 255, 0));
            }
        }
        public void ThresholdDetection(Image<Gray, byte> img, Image<Gray, byte> accumulator)
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



        public List<Rectangle> FilterContours(double cannyThreshold, double cannyThresholdLinking, Image<Gray, byte> accumulator)
        {
            List<Rectangle> ROIs = new List<Rectangle>();
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
                        ROIs.Add(rectangle);
                }
            }
            return ROIs;
        }


        public List<Rectangle> FindROIs(Image<Gray, Byte> img, Image<Gray, byte> accumulator)
        {

            double cannyThreshold = 180;
            double cannyThresholdLinking = 120;

            Stopwatch watch = Stopwatch.StartNew();
            UMat uimage = img.ToUMat();
            Image<Gray, Byte> filterBuffer = new Image<Gray, byte>(img.Size);

            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Init {0}ms", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();

            ThresholdDetection(img, accumulator);

            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Thresholding: {0}ms", watch.ElapsedMilliseconds);
            accumulator.Save("accum.jpg");
            watch = Stopwatch.StartNew();

            List<Rectangle> rois = FilterContours(cannyThreshold, cannyThresholdLinking, accumulator);
            watch.Stop();
            
            System.Diagnostics.Debug.WriteLine("Canny: {0}ms", watch.ElapsedMilliseconds);
            return rois;
        }

        public abstract Image<Gray, Byte> FindObjectOfInterest(Image<Gray, byte> img, Image<Gray, byte> accumulator, Size desiredSize);

    }
}
