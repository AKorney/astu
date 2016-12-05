using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nn.classes
{

    internal class SamplesCollection
    {
        public Double[][] Samples { get; private set; }
        public Double[][] ExpectedPotentials { get; private set; }
        public bool IsBinary { get; private set; }
        public SampleSize Size { get; private set; }
        public int ClassesCount { get; private set; }
        public int Count { get; private set; }
        public SamplesCollection(string filePath)
        {
            int width, height, outputClassesCount;
            bool isBinary;
            int samplesCount;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {

                    ///*
                    String content = streamReader.ReadLine();

                    String[] managementBuffer = content.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    int binaryFlag = 0;
                    Int32.TryParse(managementBuffer[0], out binaryFlag);
                    IsBinary = binaryFlag == 0;

                    Int32.TryParse(managementBuffer[1], out width);
                    Int32.TryParse(managementBuffer[2], out height);
                    Int32.TryParse(managementBuffer[3], out outputClassesCount);
                    ClassesCount = outputClassesCount;
                    Int32.TryParse(managementBuffer[4], out samplesCount);
                    Count = samplesCount;
                    //    ReadToEnd();
                    //content = content.Replace('\n', ' ');
                    //content = content.Replace('\r', ' ');

                    //String[] buffer = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Size = new SampleSize(width, height);
                    Samples = new double[Count][];
                    ExpectedPotentials = new double[Count][];
                    for (int i = 0; i < Count; i++)
                    {
                        Samples[i] = new double[Size.PointsCount];
                        ExpectedPotentials[i] = new double[outputClassesCount];
                    }

                    for (int sampleIndex = 0; sampleIndex < samplesCount; sampleIndex++)
                    {
                        string sampleText = "";
                        for (int line = 0; line < height; line++)
                        {
                            sampleText = sampleText + ' ' + streamReader.ReadLine();
                        }
                        string[] buffer = sampleText.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < Size.PointsCount; i++)
                        {
                            Double.TryParse(buffer[i], out Samples[sampleIndex][i]); 
                        }
                        string answer = streamReader.ReadLine();
                        int sampleClass;
                        Int32.TryParse(answer, out sampleClass);
                        ExpectedPotentials[sampleIndex][sampleClass - 1] = 1.0;
                    }
                }
            }
        }
    }


    internal struct SampleSize
    {
        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public SampleSize(int width, int height)
        {
            _width = width;
            _height = height;
            _pointsCount = _width * _height;
        }

        private int _pointsCount;
        public int PointsCount
        {
            get
            {
                return _pointsCount;
            }
        }
    }

}
