using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;

using System.Drawing;
using System.IO;
using System.Threading;

namespace TrafficSign.Imaging
{
    public struct TrainSample
    {
        public float[] HoG { get; set; }
        public int ClassId { get; set; }
    }
    public class TrainHog
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
            for (int i = 0; i < files.Length; i++)// 1; i++)
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

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
