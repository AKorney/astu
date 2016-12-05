using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nn.classes
{
    public class TrainingSet
    {
        private static Char [] separators = new Char[] { ' ' };

        private int _fullSamplesCount;
        private int _step;
        public int SampleWidth { get; private set; }
        public int SampleHeight { get; private set; }

        public int SampleSize
        {
            get; private set;
            
        }

        public int SamplesCount { get; private set; }

        public bool IsBinary { get; private set; }

        public double[][] Samples { get; private set; }

        public double[][] Answers { get; private set; }

        public int ClassesCount { get; private set; }

        public TrainingSet(string filePath, int step)
        {
            _step = step;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {


                    string trainingInfoString = reader.ReadLine();
                    ParseInfo(trainingInfoString);
                    Prepare();

                    for (int sampleIndex = 0; sampleIndex < _fullSamplesCount; sampleIndex++)
                    {
                        //parse one sample
                        StringBuilder sampleStringBuilder = new StringBuilder();
                        for (int i = 0; i < SampleHeight; i++)
                        {
                            sampleStringBuilder.Append(" ");
                            sampleStringBuilder.Append(reader.ReadLine());
                        }
                        if (sampleIndex % step == 0)
                        {
                            string sampleContent = sampleStringBuilder.ToString().Trim();
                            string[] sampleValuesBuffer = sampleContent.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < SampleSize; i++)
                            {
                                double currentValue;
                                double.TryParse(sampleValuesBuffer[i], out currentValue);
                                Samples[sampleIndex / _step][i] = currentValue;
                            }
                        }
                        int expectedClass;
                        int.TryParse(reader.ReadLine(), out expectedClass);
                        if (sampleIndex % step == 0)
                        {
                            Answers[sampleIndex / _step][expectedClass - 1] = 1.0;
                        }
                    }
                }
            }
        }

        private void Prepare()
        {
            Samples = new double[SamplesCount][];
            Answers = new double[SamplesCount][];
            for (int i = 0; i < SamplesCount; i++)
            {
                Samples[i] = new double[SampleSize];
                Answers[i] = new double[ClassesCount];
            }
        }

        private void ParseInfo(string trainingInfoString)
        {
            int binaryFlag, width, height, classesCount, samplesCount;
             
            string[] infoBuffer = trainingInfoString.Split(separators,
                StringSplitOptions.RemoveEmptyEntries);

            int.TryParse(infoBuffer[0], out binaryFlag);
            IsBinary = binaryFlag == 0;

            int.TryParse(infoBuffer[1], out width);
            int.TryParse(infoBuffer[2], out height);
            int.TryParse(infoBuffer[3], out classesCount);
            int.TryParse(infoBuffer[4], out samplesCount);

            SampleWidth = width;
            SampleHeight = height;
            ClassesCount = classesCount;
            _fullSamplesCount = samplesCount;
            SamplesCount = samplesCount / _step;
            if (_step > 1 && samplesCount % _step > 0)
            {
                SamplesCount += 1;
            }
            SampleSize = SampleWidth * SampleHeight;
        }
    }
}
