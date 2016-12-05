using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.Structure;

namespace ANNTest
{
    internal class ANNHelper
    {
        public void Train(List<TrainSample> data, Matrix<int> layerSizes)
        {
            Matrix<float> data_in;
            Matrix<float> response;

            int sampleSize = layerSizes[0, 0];
            int classesCount = 6;

            data_in = new Matrix<float>(data.Count, sampleSize);
            response = new Matrix<float>(data.Count, classesCount);

            for (int i = 0; i < data.Count; i++)
            {
                var sample = data[i];
                for (int j = 0; j < sampleSize; j++)
                {
                    data_in.Data[i, j] = data[i].HoG[j];
                }
                response[i, data[i].ClassId] = 1.0f;
                //data_in.Data[i,j]=
            }
            //Matrix< float > response
            using (ANN_MLP network = new ANN_MLP())
            {
                network.SetActivationFunction(ANN_MLP.AnnMlpActivationFunction.SigmoidSym, 1,1);
                network.SetLayerSizes(layerSizes);
                network.TermCriteria = new MCvTermCriteria(2000, 1.0e-6); // Number of Iteration for training change 1000 and 1.0e-6 values for your own requirements 
                network.SetTrainMethod(ANN_MLP.AnnMlpTrainMethod.Backprop);
                network.BackpropWeightScale = 0.0;// 1;
                network.BackpropMomentumScale = 0.0;// 4;
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
        public Matrix<float> Predict(float[] inp)
        {
            Matrix<float> result = new Matrix<float>(1, 6);
            Matrix<float> input = new Matrix<float>(1, 324);
            for (int j = 0; j < 324; j++)
            {
                input.Data[0, j] = inp[j];
            }
            using (ANN_MLP network = new ANN_MLP())
            {

                network.Read(new FileStorage("trained.xml", FileStorage.Mode.Read).GetFirstTopLevelNode()); // Read Fixed values for training
                var res = network.Predict(input, result);
                return result;
            }

        }
    }
}
