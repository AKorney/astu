using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using System.IO;
using System.Drawing;
using TrafficSignRecognition.CV;

namespace ANNTest
{
    internal class ANNHelper
    {
        public void PrepareData(string rootPath, int classesCount, Size sampleSize)//)
           // , out Matrix<float> data_in, out Matrix<float> response)
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
            using (StreamWriter writer = new StreamWriter("tstask.txt"))
            {
                writer.WriteLine("1 40 40 3 54");
                for (int i = 0; i < classesCount; i++)
                {
                    for (int j = 0; j < fileNames[i].Length; j++)
                    {
                        //img = CvInvoke.Imread(fileNames[i][j], Emgu.CV.CvEnum.LoadImageType.Grayscale);
                        //CvInvoke.Normalize(img, img, 255, 0, Emgu.CV.CvEnum.NormType.MinMax);
                        //CvInvoke.Threshold(img, img, 0, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv | Emgu.CV.CvEnum.ThresholdType.Otsu);
                        //CvInvoke.cvConvertScale(img, inputMat, 1.0 / 255, 0);
                        Image<Rgb, Byte> source = new Image<Rgb, byte>(fileNames[i][j]);
                        Write(writer, GetImgOfInterest(source), i + 1);
                        //WriteSample(data_in, currentIndex, img);
                        //response[currentIndex, i] = 1.0f;
                        //currentIndex++;
                    }
                }
            }
            //*/

            /*
            data_in = new Matrix<float>(samplesCount, fullSampleSize);
            response = new Matrix<float>(samplesCount, classesCount);
            int currentIndex = 0;
            for (int i = 0; i < classesCount; i++)
            {
                for (int j = 0; j < fileNames[i].Length; j++)
                {
                    img = CvInvoke.Imread(fileNames[i][j], Emgu.CV.CvEnum.LoadImageType.Grayscale);
                    //CvInvoke.cvConvertScale(img, inputMat, 1.0 / 255, 0);
                    WriteSample(data_in, currentIndex, img);
                    response[currentIndex, i] = 1.0f;
                    currentIndex++;
                }
            }
            //*/
        }

        private Image<Gray, Byte> GetImgOfInterest(Image<Rgb, Byte> img)
        {
            Image<Gray, Byte> filtered = SignDetectionHelper.GrayScaleByChannel(img, 0);
            filtered.Bitmap.Save("fltr.jpg");

            Image<Gray, Byte> accumulator = new Image<Gray, byte>(img.Width, img.Height, new Gray(0));
            SignDetectionHelper.ThresholdDetection(filtered, accumulator);
            return SignDetectionHelper.FindObjectOfInterest(img.Convert<Gray, Byte>(), accumulator);
        }

        private void Write(StreamWriter writer, Image<Gray, Byte> img, int v)
        {
            
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    writer.Write(string.Format("{0} ", img.Data[i,j,0]));
                }
                writer.Write(writer.NewLine);
            }
            writer.WriteLine(string.Format("{0}", v));
        }

        public Matrix<float> ReadSample(string fileName)
        {
            Matrix<float> data = new Matrix<float>(1, 1600);
            Mat inputMat = CvInvoke.Imread(fileName, Emgu.CV.CvEnum.LoadImageType.Grayscale);
            ///*
            Image<Rgb, Byte> source = new Image<Rgb, byte>(fileName);
            using (StreamWriter writer = new StreamWriter("sample.txt", true))
            {
                Write(writer, GetImgOfInterest(source), -1);
            }
            //*/

            /*
            int lineIndex = 0;
            for (int i = 0; i < inputMat.Rows; i++)
            {
                for (int j = 0; j < inputMat.Cols; j++)
                {
                    byte[] value = inputMat.GetData(i, j);
                    float normalized = Convert.ToSingle(value[0]) / 255.0f;
                    data[0, lineIndex++] = normalized;
                }
            }
            */
            return data;
        }

        private void WriteSample(Matrix<float> data_in, int v, Mat inputMat)
        {
            int lineIndex = 0;
            for (int i = 0; i < inputMat.Rows; i++)
            {
                for (int j = 0; j < inputMat.Cols; j++)
                {
                    byte[] value = inputMat.GetData(i, j);
                    float normalized = Convert.ToSingle(value[0]);// / 255.0f;
                    data_in[v, lineIndex++] = normalized;
                }
            }
        }

        public void Train(Matrix<float> data_in, Matrix<int> layerSizes, Matrix<float> response)
        {
            //Matrix< float > response
            using (ANN_MLP network = new ANN_MLP())
            {
                network.SetActivationFunction(ANN_MLP.AnnMlpActivationFunction.SigmoidSym,1,1);
                network.SetLayerSizes(layerSizes);
                network.TermCriteria = new MCvTermCriteria(2000, 1.0e-6); // Number of Iteration for training change 1000 and 1.0e-6 values for your own requirements 
                network.SetTrainMethod(ANN_MLP.AnnMlpTrainMethod.Backprop);
                network.BackpropWeightScale = 0;// .1;
                network.BackpropMomentumScale = 0;// .4;
                /* 
                network.RpropDWMin = 0.2; // some parameters to improve ANN training 
                network.RpropDWMax = 2;
                network.RpropDWMinus = -1.1;
                network.RpropDWPlus = 1.1;
                //*/

                network.Save("tmp.xml"); // Save temp weights to file for correction before training
                network.Read(new FileStorage("tmp.xml", FileStorage.Mode.Read).GetFirstTopLevelNode()); // Read Fixed values for training
                TrainData training = new TrainData(data_in, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, response); // Creating training data
                network.Train(training);
                // Start Training 
                network.Save("trained.xml");

            }
        }
        public Matrix<float> Predict(Matrix<float> input)
        {
            Matrix<float> result = new Matrix<float>(1, 3);

            using (ANN_MLP network = new ANN_MLP())
            {

                network.Read(new FileStorage("trained.xml", FileStorage.Mode.Read).GetFirstTopLevelNode()); // Read Fixed values for training
                var res = network.Predict(input, result);
                return result;
            }

        }
    }
}
