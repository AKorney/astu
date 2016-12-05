using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Accord.Neuro;
using Accord.Neuro.Learning;
using TrafficSign.Imaging;
using System.IO;
using Imaging;

namespace TrafficSignTrain
{

    public class ANNTrainer
    {
        private ActivationNetwork _network;
        private double _learningRate = 0.1;
        private double _sigmoidAlphaValue = 2.0;
        private int _neuronsInHiddenLayer = 800;
        private int _inputsCount = 1600;
        private int _iterations = 500;
        private bool _useRegularization;
        private int _classesCount = 5;
        private double _termError = 0.00001;

        public ANNTrainer()
        {
            _network = new ActivationNetwork(
               // new  BipolarSigmoidFunction(_sigmoidAlphaValue),
               new SigmoidFunction(),
                _inputsCount,
                _inputsCount * 2,
                _classesCount
                );
        }


        public void Train(List<Imaging.ImgSample> rawData)
        //public void Train(List<TrainSample> rawData)
        {
            double[][] data_in;
            double[][] response;

            PrepareData(rawData, out data_in, out response);
            //var teacher = new ParallelResilientBackpropagationLearning(_network);
            var teacher = new BackPropagationLearning(_network);
            // set learning rate and momentum
            teacher.LearningRate = _learningRate;
            
            teacher.Momentum = 0;
            //_network.Randomize();
            // iterations
            int iteration = 1;

            int samplesCount = rawData.Count;
            // solution array
            double[,] solution = new double[samplesCount, _classesCount];


            // loop
            while (true)
            {
                // run epoch of learning procedure
                double error = teacher.RunEpoch(data_in, response) / samplesCount;
                                              

                // calculate error
                double learningError = 0.0;
                for (int j = 0; j < samplesCount; j++)
                {
                    double[] x = data_in[j];
                    double[] expected = response[j];
                    double[] actual = _network.Compute(x);

                    for (int i = 0; i < _classesCount; i++)
                    {
                        learningError += (actual[i] - expected[i])* (actual[i] - expected[i]) / 2;
                    }
                }
                System.Diagnostics.Debug.WriteLine(string.Format("Error:{0}\t Iteretion: {1}", learningError, iteration));

                // increase current iteration
                iteration++;


                if (learningError < _termError)
                    break;
                // check if we need to stop
                if ((_iterations != 0) && (iteration > _iterations))
                    break;
            }




        }
        private void PrepareData(List<ImgSample> rawData, out double[][] data_in, out double[][] response)
        //private void PrepareData(List<TrainSample> rawData, out double[][] data_in, out double[][] response)
        {
            
            data_in = new double[rawData.Count][];
            response = new double[rawData.Count][];
            //using (StreamWriter writer = new StreamWriter("task.txt"))
            //{
            //    writer.WriteLine("1 18 18 3 90");
            for (int sampleIndex = 0; sampleIndex < rawData.Count; sampleIndex++)
            {
                data_in[sampleIndex] = new double[_inputsCount];
                for (int i = 0; i < _inputsCount; i++)
                {
                    data_in[sampleIndex][i] = rawData[sampleIndex].Data[i];// / 255;
                    //data_in[sampleIndex][i] = rawData[sampleIndex].HoG[i];// / 255;
                }
                //int idx = 0;
                //for (int i = 0; i < 18; i++)
                //{
                //    for (int j = 0; j < 18; j++)
                //    {
                //        writer.Write(string.Format("{0} ", (int)(data_in[sampleIndex][idx++] * 255)));
                //    }
                //    writer.Write(writer.NewLine);
                //}
                //writer.WriteLine(rawData[sampleIndex].ClassId + 1);
                response[sampleIndex] = new double[_classesCount];
                response[sampleIndex][rawData[sampleIndex].ClassId] = 1.0;
                //    }
            }
        }
    }
}
