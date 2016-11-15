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

namespace TrafficSignRecognition.CV
{
    internal static class EmguHelper
    {
        public static Image<Gray, Byte> FilterRed(Image<Rgb, Byte> source)
        {            
            source.SmoothGaussian(5);
            Image<Hsv, Byte > hsvSource = source.Convert<Hsv, Byte>();

            
            Image<Gray, Byte>[] channels = hsvSource.Split();  //split into components
            Image<Gray, Byte> imghue = channels[0];            //hsv, so channels[0] is hue.
            Image<Gray, Byte> imgsat = channels[1];            //hsv, so channels[1] is saturation.

           
            Image<Gray, byte> hueFulterHigh = imghue.InRange(new Gray(160), new Gray(180));            
            Image<Gray, byte> hueFilterLow = imghue.InRange(new Gray(0), new Gray(20));
            Image<Gray, byte> hueFilter = hueFulterHigh.Or(hueFilterLow);

            Image<Gray, byte> colordetimg = imgsat.And(hueFilter);

            return colordetimg;
        }

        public static UMat DetectEdges(Image<Gray, Byte> img, Image<Rgb, Byte> toMark)
        {
            //double cannyThresholdLinking = 120.0;

            double cannyThreshold = 180;
            //double circleAccumulatorThreshold = 120;
            //CircleF[] circles = CvInvoke.HoughCircles(img, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

            double cannyThresholdLinking = 140.0;
            //UMat cannyEdges = new UMat();
            //cannyEdges = img.Canny(cannyThreshold, cannyThresholdLinking).ToUMat();
            Image<Gray, Byte> canny = img.Canny(cannyThreshold, cannyThresholdLinking);
            canny.Save("imcanny.jpg");
            Image<Rgb, Byte> fun = new Image<Rgb, byte>(toMark.Width, toMark.Height, new Rgb(Color.DarkCyan));
           
            /*
            List<Triangle2DF> triangleList = new List<Triangle2DF>();
            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle
            #region tri & rect
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                            {
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                   pts[0],
                                   pts[1],
                                   pts[2]
                                   ));
                            }
                            else if (approxContour.Size == 4) //The contour has 4 vertices.
                            {
                                #region determine if all the angles in the contour are within [80, 100] degree
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                #endregion

                                if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            }
                        }
                    }
                }
            }
            #endregion


            #region draw triangles and rectangles
            Mat triangleRectangleImage = new Mat(img.Size, DepthType.Cv8U, 3);
            triangleRectangleImage.SetTo(new MCvScalar(0));
            foreach (Triangle2DF triangle in triangleList)
            {
                CvInvoke.Polylines(toMark, Array.ConvertAll(triangle.GetVertices(), Point.Round), true, new Bgr(Color.DarkBlue).MCvScalar, 2);
            }
            foreach (RotatedRect box in boxList)
            {
                CvInvoke.Polylines(toMark, Array.ConvertAll(box.GetVertices(), Point.Round), true, new Bgr(Color.DarkOrange).MCvScalar, 2);
            }


            #endregion

            #region draw circles
            Mat circleImage = new Mat(img.Size, DepthType.Cv8U, 3);
            circleImage.SetTo(new MCvScalar(0));
            foreach (CircleF circle in circles)
                CvInvoke.Circle(toMark, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Brown).MCvScalar, 2);

            
            #endregion
            //*/
            //return cannyEdges;
            return canny.ToUMat();
            //CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);
        }
    }
}
