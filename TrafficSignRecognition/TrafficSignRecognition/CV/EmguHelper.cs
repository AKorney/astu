﻿using System;
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
            

            watch = Stopwatch.StartNew();
          
            int threshmin = 80, threshmax = 180, step = 15;
            double alpha =  (double)step / (threshmax - threshmin);
            for(int t = threshmin; t <= threshmax; t+=step)
            {
                CvInvoke.Threshold(uimage, filterBuffer, t, 255 , Emgu.CV.CvEnum.ThresholdType.Binary);
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
