using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;

using System.Drawing;
using System.IO;

namespace ANNTest
{
    internal struct TrainSample
    {
        public float[] HoG { get; set; }
        public int ClassId { get; set; }
    }
    internal class TrainHog
    {
        static Random randomizer = new Random();
       


        public static Image<Bgr, Byte> Resize(Image<Bgr, Byte> src)
        {
            return src.Resize(32, 32, Emgu.CV.CvEnum.Inter.Linear);
        }


        public static float[] GetVector(Image<Bgr, Byte> im)
        {
            HOGDescriptor hog = new HOGDescriptor(new Size(32,32), new Size(16,16), new Size(8,8), new Size(8,8));
            
            Image<Gray, Byte> imageOfInterest = new Image<Gray, byte>(im.Size);
            CvInvoke.CvtColor(Resize(im), imageOfInterest, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            System.Drawing.Point[] p = new System.Drawing.Point[0];

            float[] result = hog.Compute(imageOfInterest);//, new System.Drawing.Size(8, 8), default(Size), p);
            return result;
        }

        public static List<TrainSample> ComputeForClass(string path, int classId)
        {
            List<TrainSample> samples = new List<TrainSample>();
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                var img = new Image<Bgr, byte>(files[i]);
                var vec = GetVector(img);
                samples.Add(new TrainSample() { HoG = vec, ClassId = classId });
            }
            return samples;
        }


        public static void PrepareNegative(int count, int width, int height, string[] paths)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), new Size(width, height));
            for (int i = 0; i < paths.Length; i++)
            {
                var bmp = new Bitmap(paths[i]);
                Image<Bgr, Byte> current = new Image<Bgr, Byte>(bmp);
                for (int j = 0; j < count / paths.Length; j++)
                {


                    rect.X = randomizer.Next(current.Width - width - 1);
                    rect.Y = randomizer.Next(current.Height - height - 1);

                    current.ROI = rect;

                    var img = current.Clone();
                    img.Save(string.Format("{0}_{1}.jpg", i, j));
                    current.ROI = System.Drawing.Rectangle.Empty;
                }

            }
        }
    }
}
