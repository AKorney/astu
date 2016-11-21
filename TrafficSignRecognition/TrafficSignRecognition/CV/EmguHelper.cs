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
    internal static class EmguHelper
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
            System.Diagnostics.Debug.WriteLine("Time for red conv {0}", watch.ElapsedMilliseconds);
            return result;
        }

        public static Image<Gray, Byte> FilterRed(Image<Rgb, Byte> source)
        {
            // source.SmoothGaussian(5);
            Stopwatch watch = Stopwatch.StartNew();

            Image<Hsv, Byte > hsvSource = source.Convert<Hsv, Byte>();

            
            Image<Gray, Byte>[] channels = hsvSource.Split();  //split into components
            Image<Gray, Byte> imghue = channels[0];            //hsv, so channels[0] is hue.
            Image<Gray, Byte> imgsat = channels[1];            //hsv, so channels[1] is saturation.

                     
            Image<Gray, byte> hueFilterHigh = imghue.InRange(new Gray(160), new Gray(180));
            Image<Gray, byte> hueFilterLow = imghue.InRange(new Gray(0), new Gray(20));
            Image<Gray, byte> hueFilter = hueFilterHigh.Or(hueFilterLow);

           
            Image<Gray, byte> colordetimg = imgsat.And(hueFilter);
            CvInvoke.Normalize(colordetimg, colordetimg, 0, 255, NormType.MinMax);
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Time for red conv {0}", watch.ElapsedMilliseconds);
            return colordetimg[0];
            
        }
        
        public static UMat DetectEdges(Image<Gray, Byte> img, Image<Rgb, Byte> toMark)
        {

            double cannyThreshold =200;
            double cannyThresholdLinking = 120;
            Stopwatch watch = Stopwatch.StartNew();
            UMat uimage = img.ToUMat();
            Image<Gray, Byte> filterBuffer = new Image<Gray, byte>(img.Size);
            Image<Gray, Byte> accumulator = new Image<Gray, byte>(img.Width, img.Height, new Gray(0));

            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Init {0}", watch.ElapsedMilliseconds);
            
            //uimage.Save("noise.jpg");

            //Image<Gray, byte> imgThresh = img.CopyBlank();
            //CvInvoke.Threshold(uimage, imgThresh, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu | Emgu.CV.CvEnum.ThresholdType.ToZero);
            //CvInvoke.Threshold(imgThresh, uimage, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu | Emgu.CV.CvEnum.ThresholdType.ToZero);
            //CvInvoke.PyrDown(uimage, filterBuffer);
            //CvInvoke.PyrUp(filterBuffer, uimage);
            //imgThresh.Save("otsu.jpg");
            //watch = Stopwatch.StartNew();

            //Emgu.CV.Features2D.MSERDetector det = new Emgu.CV.Features2D.MSERDetector(
            //    minArea: (int)(0.0005 * toMark.Rows * toMark.Cols)
            //    , maxArea: (int)(0.003 * toMark.Rows * toMark.Cols)
            //    , delta:12
            //    , maxVariation:0.22
            //    , minDiversity: 0.5);
            //MKeyPoint[] pts = det.Detect(img);

            //watch.Stop();
            //System.Diagnostics.Debug.WriteLine("MSER {0}", watch.ElapsedMilliseconds);
            //foreach (MKeyPoint pt in pts)
            //    CvInvoke.Circle(toMark, Point.Round(pt.Point), (int)pt.Size, new Bgr(Color.Brown).MCvScalar, 2);

            watch = Stopwatch.StartNew();
          
            int threshmin = 80, threshmax = 180, step = 15;
            double alpha =  (double)step / (threshmax - threshmin);
            for(int t = threshmin; t <= threshmax; t+=step)
            {
                CvInvoke.Threshold(uimage, filterBuffer, t, 255 , Emgu.CV.CvEnum.ThresholdType.Binary);
                //filterBuffer.Save(String.Format("{0}.jpg", t));
                CvInvoke.AddWeighted(filterBuffer, alpha, accumulator, 1, 0, accumulator);
            }
            CvInvoke.PyrDown(accumulator, filterBuffer);
            CvInvoke.PyrUp(filterBuffer, accumulator);
            CvInvoke.Threshold(accumulator, accumulator, 85, 255, ThresholdType.ToZero );

            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Th {0}", watch.ElapsedMilliseconds);
            accumulator.Save("accum.jpg");
            watch = Stopwatch.StartNew();
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(accumulator, cannyEdges, cannyThreshold, cannyThresholdLinking);
                    
            
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {

                    var rectangle = CvInvoke.BoundingRectangle(contours[i]);
                    if (rectangle.Size.Width > 25 && rectangle.Size.Height > 25 && rectangle.Size.Height < 50 && rectangle.Size.Width < 50)                        
                            CvInvoke.Rectangle(toMark, rectangle, new MCvScalar(255, 255, 0));                    
                }
            }
            watch.Stop();
            cannyEdges.Save("canny.jpg");
            System.Diagnostics.Debug.WriteLine("Th {0}", watch.ElapsedMilliseconds);
            return img.ToUMat();
            
        }
        
    }
}
