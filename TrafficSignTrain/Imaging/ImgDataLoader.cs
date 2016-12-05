using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaging
{
    public struct ImgSample
    {
        public double[] Data { get; set;}
        public int ClassId { get; set; }
        public ImgSample(int size)
        {
            Data = new double[size];
            ClassId = -1;
        }
    }
    public class ImgDataLoader
    {
        private List<ImgSample> _dataSet;
        private int _classesCount;
        private int _sampleSize;
        private Size _imgSize;

        public List<ImgSample> DataSet
        {
            get { return _dataSet; }
        }
        public ImgDataLoader(int classesCount,  Size imgSize)
        {
            _classesCount = classesCount;
            _sampleSize = imgSize.Width * imgSize.Height;
            _imgSize = imgSize;
            _dataSet = new List<ImgSample>();
        }
        public void LoadAll(string rootPath)
        {
            Mat img = new Mat();
            Image<Gray, float> inputMat = new Image<Gray, float>(_imgSize.Width, _imgSize.Height);


            //prepare full samples list
            int samplesCount = 0;
            string[][] fileNames = new string[_classesCount][];
            for (int i = 0; i < _classesCount; i++)
            {
                string[] files = Directory.GetFiles(string.Format("{0}//{1}", rootPath, i));
                fileNames[i] = files;
                samplesCount += files.Length;
            }


            

            for (int i = 0; i < _classesCount; i++)
            {
                for (int j = 0; j < fileNames[i].Length; j++)
                {
                    ImgSample sample = new ImgSample(_sampleSize);
                    img = CvInvoke.Imread(fileNames[i][j], Emgu.CV.CvEnum.LoadImageType.Grayscale);
                    sample.ClassId = i;
                    ConvertMatToDoubleArr(sample.Data, img);
                    _dataSet.Add(sample);
                }
            }
            //*/
        }
    

        private void ConvertMatToDoubleArr(double[] data, Mat img)
        {
            int lineIndex = 0;
            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    byte[] value = img.GetData(i, j);
                    double normalized = Convert.ToDouble(value[0]);// / 255.0f;
                    data[lineIndex++] = normalized;
                }
            }
        }
    }
}
