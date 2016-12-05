using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficSign.Detection.Common;

namespace TrafficSign.Recognition.TrainingSetExtraction
{
    public class TextExtractor
    {
        DetectorBase _detector;
        public TextExtractor(DetectorBase detector)
        {
            _detector = detector;
        }
        public void PrepareData(string rootPath, int classesCount, Size sampleSize)
        {
            Mat img = new Mat();
            Image<Gray, float> inputMat = new Image<Gray, float>(sampleSize.Width, sampleSize.Height);

            int fullSampleSize = sampleSize.Width * sampleSize.Height;

            //prepare full samples list
            int samplesCount = 0;
            string[][] fileNames = new string[classesCount][];
            for (int i = 0; i < classesCount; i++)
            {
                string[] files = Directory.GetFiles(string.Format("{0}//{1}", rootPath, i));
                fileNames[i] = files;
                samplesCount += files.Length;
            }

            ///*
            using (StreamWriter writer = new StreamWriter(string.Format("tstask{0}.txt", _detector.GetType().Name)))
            {
                writer.WriteLine(string.Format("{0} {1} {2} {3} {4}", 1, sampleSize.Height, sampleSize.Width, classesCount, samplesCount));
                for (int i = 0; i < classesCount; i++)
                {
                    for (int j = 0; j < fileNames[i].Length; j++)
                    {
                        Image<Rgb, Byte> source = new Image<Rgb, byte>(fileNames[i][j]);
                        Write(writer, GetImgOfInterest(source, sampleSize), i + 1);

                    }
                }
            }            
        }

        private Image<Gray, Byte> GetImgOfInterest(Image<Rgb, Byte> img, Size size)
        {
            Image<Gray, Byte> filtered = _detector.FilterColorChannel(img);
            filtered.Bitmap.Save("fltr.jpg");

            Image<Gray, Byte> accumulator = new Image<Gray, byte>(img.Width, img.Height, new Gray(0));
            _detector.ThresholdDetection(filtered, accumulator);
            return _detector.FindObjectOfInterest(img.Convert<Gray, Byte>(), accumulator, size);
        }

        private void Write(StreamWriter writer, Image<Gray, Byte> img, int v)
        {

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    writer.Write(string.Format("{0} ", img.Data[i, j, 0]));
                }
                writer.Write(writer.NewLine);
            }
            writer.WriteLine(string.Format("{0}", v));
        }

        public void ExtractFromFile(string destPath, string fileName, Size size)
        {
            
            Mat inputMat = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.LoadImageType.Grayscale);
            Image<Rgb, Byte> source = new Image<Rgb, byte>(fileName);
            using (StreamWriter writer = new StreamWriter(destPath, true))
            {
                Write(writer, GetImgOfInterest(source, size), -1);
            }
            
        }
        
              
        
    }
}
