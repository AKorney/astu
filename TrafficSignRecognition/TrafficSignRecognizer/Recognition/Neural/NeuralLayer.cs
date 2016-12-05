using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nn.classes
{
    /// <summary>
    /// Класс, описывающий слой полносвязной нейросети.
    /// Слой характеризуется числом входов,
    /// нейронов и матрицей весов.
    /// Параметры задаются.
    /// </summary>
    class NeuralLayer
    {
        #region fields
        /// <summary>
        /// генератор синапсов для начальной инициализации сети
        /// создаю статическим, чтоб для всех слоев работал один генератор        
        /// </summary>
        private static Random randomGenerator = new Random();
        /// <summary>
        /// число входов и выходов слоя нейросети
        /// </summary>
        private int outputsCount, inputsCount;        
        /// <summary>
        /// матрица весов, [число входов * число нейронов]
        /// </summary>
        private double[,] weights;
        /// <summary>
        /// состояние выходов на текущий момент
        /// </summary>
        private double[] lastOut;
        /// <summary>
        /// Накопленное на входах в данный момент
        /// </summary>
        private double[] lastIn;
        /// <summary>
        /// Храним производные по выходам, входам и весам
        /// </summary>
        private double[] dEdY;
        private double[] dEdX;
        private double[,] dEdW;
        private const int Bias =1;
        #endregion
        #region methods
        /// <summary>
        /// Инициализирует новый слой нейросети (веса не генерятся)
        /// </summary>
        /// <param name="inputsCount">число входов</param>
        /// <param name="outputsCount">число выходов</param>
        public NeuralLayer( int inputsCount,int outputsCount)
        {
            this.outputsCount = outputsCount;
            this.inputsCount = inputsCount;
            dEdY = new double[outputsCount];
            dEdX = new double[outputsCount];
            dEdW = new double[inputsCount + Bias, outputsCount];
        }
        /// <summary>
        /// Генерирует случайные входные веса
        /// </summary>
        public void GenerateWeights()
        {
            //выделим память под веса
            weights = new double[inputsCount+Bias,outputsCount];
            //всю матрицу
            for (int i = 0; i < inputsCount+Bias; i++)
            {                
                for (int j = 0; j < outputsCount; j++)
                    //заполним случайными числами
                    weights[i,j] = randomGenerator.NextDouble()-0.5;
            }
        }
        /// <summary>
        /// Для последнего слоя
        /// </summary>
        /// <param name="idealOut">Идеальный выходной вектор</param>
        public void CountOutDetivativesAsLast(double[] idealOut)
        {
            for (int i = 0; i < outputsCount; i++)
            {
                //простейшая формула производной от ошибки
                dEdY[i] = lastOut[i] - idealOut[i];//=dE/dYk3
            }
        }
        /// <summary>
        /// Считает производные по выходам промежуточного слоя
        /// </summary>
        /// <param name="dEdXPrev">Производные по входам слоя, следующего за данным
        /// (того, в который входят связи, исходящие из данного слоя)</param>
        /// <param name="wPrev">Веса связей, исходящих из данного слоя
        /// (берем набор связей, входящих в следующий слой)</param>
        public void CountOutDerivatives(double[] dEdXPrev,double[,] wPrev)
        {
            //по каждому выходу
            for (int n = 0; n < outputsCount; n++)
            {
                //производная есть сумма, т.к. E=E(x1(y1,y2,...,yn),...,xk(y1,y2,...,yn))
                //k - число входов след. слоя
                //n - число выходов текущего слоя
                //т.о. на каждой веточке вносится вклад в ошибку ~ весу
                double dEdYn = 0.0;
                for (int i = 0; i < dEdXPrev.Length; i++)
                {
                    dEdYn += dEdXPrev[i] * wPrev[n, i];
                }
                dEdY[n] = dEdYn;
            }
        }
        /// <summary>
        /// Считает производные по входам слоя
        /// </summary>
        public void CountInDerivatives()
        {
            //проходя через слой, получаем X->Y, Y=Sg(X)
            //производная сигмоида выражается через его значение
            for (int i = 0; i < outputsCount; i++)
            {
                double dYdX = Sigmoid.betta * lastOut[i] * (1 - lastOut[i]);
                dEdX[i] = dEdY[i] * dYdX;
            }
        }
        /// <summary>
        /// Считает производные по весам
        /// </summary>
        /// <param name="yprev">Выход предыдущего слоя</param>
        public void CountWeightDerivatives(double[] yprev)
        {
            //yprev.Lenght = inputsCount
            //dEdX.Lenght = outputsCount
            //Вход для каждого нейрона - линейная комбинация
            //выходов предыдущего (к-т равен весу). Т.о. производная по ij весу
            //(для i входа j нейрона) равна i компоненте входного в-ра данного слоя
            //т.е. выходного в-ра предыдущего
            for (int i = 0; i < inputsCount+Bias; i++)
            {                
                for (int j = 0; j < outputsCount; j++)
                {
                    if (i == inputsCount)
                    {
                        dEdW[i, j] = dEdX[j] * 1;
                    }
                    else
                    {
                        dEdW[i, j] = dEdX[j] * yprev[i];
                    }
                }                 
            }
        }
        /// <summary>
        /// Обновляет веса на основании расчитанных производных
        /// </summary>
        /// <param name="delta"></param>
        public void Update(double delta)
        {
            for(int i=0;i<inputsCount + Bias;i++)
                for (int j = 0; j < outputsCount; j++)
                {
                    weights[i, j] -= delta * dEdW[i, j];
                }
        }
        private double mc = 0.5;
       
        private double[,] _dxPrev;
        public void UpdateWithMomentum(double delta)
        {
            //dX = lr * dperf / dX 
            //dX = delta * dEdW

            //normal

            //dX = mc * dXprev + delta * (1 - mc) * dperf / dX
            //dX = mc * delta * _savedDeDw + delta * (1-mc)* DEDW
            //momentum
            //
            if (_dxPrev == null)
            {
                _dxPrev = new double[dEdW.GetLength(0), dEdW.GetLength(1)];

                for (int i = 0; i < inputsCount + Bias; i++)
                {
                    for (int j = 0; j < outputsCount; j++)
                    {
                        _dxPrev[i, j] = delta * dEdW[i, j];
                        weights[i, j] -= _dxPrev[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < inputsCount + Bias; i++)
                {
                    for (int j = 0; j < outputsCount; j++)
                    {
                        _dxPrev[i, j] = mc * _dxPrev[i, j] + delta * (1 - mc) * dEdW[i, j];
                        weights[i, j] -= _dxPrev[i, j];

                    }
                }

            }
        }
        #endregion
        #region properties
        /// <summary>
        /// Возвращает или устанавливает число входов
        /// </summary>
        public int InputsCount { get { return inputsCount; } set { inputsCount = value; } }
        /// <summary>
        /// Возвращает или устанивливает число выходов
        /// </summary>
        public int OutputsCount { get { return outputsCount; } set { outputsCount = value; } }
        /// <summary>
        /// Возвращает или устанавливает вес
        /// </summary>
        /// <param name="inp">Номер нейрона в слое</param>
        /// <param name="outp">Номер входа</param>
        /// <returns></returns>
        public double this[int inp, int outp] 
        { 
            get { return weights[inp, outp]; } 
            set { weights[inp, outp] = value; } 
        }
        /// <summary>
        /// состояние всех нейронов слоя
        /// </summary>
        public double[] LastOut { get { return lastOut; } set { lastOut = value; } }
        /// <summary>
        /// входныe сигналы
        /// </summary>
        public double[] LastIn { get { return lastIn; } set { lastIn = value; } }
        public double[] DEDX { get { return dEdX; } }
        public double[,] W { get { return weights; } }

        private double[] _savedLastOut;
        private double[] _savedLastIn;
        private double[] _savedDEDX;
        private double[] _savedDEDY;
        private double[,] _savedW;
        private double[,] _savedDEDW;

        public void RestoreState()
        {            
            for (int i = 0; i < _savedLastOut.Length; i++)
            {
                lastOut[i] = _savedLastOut[i];
            }

            for (int i = 0; i < _savedLastIn.Length; i++)
            {
                lastIn[i] = _savedLastIn[i];
            }


            for (int i = 0; i < _savedDEDX.Length; i++)
            {
                dEdX[i] = _savedDEDX[i];
            }

            for (int i = 0; i < _savedDEDY.Length; i++)
            {
                dEdY[i] = _savedDEDY[i];
            }

            for (int i = 0; i < _savedW.GetLength(0); i++)
            {
                for (int j = 0; j < _savedW.GetLength(1); j++)
                {
                   weights[i, j] = _savedW[i, j];
                }
            }

            for (int i = 0; i < _savedDEDW.GetLength(0); i++)
            {
                for (int j = 0; j < _savedDEDW.GetLength(1); j++)
                {
                    dEdW[i, j] = _savedDEDW[i, j];
                }
            }
        }

        public void SaveState()
        {
            if (_savedLastOut == null)
            {
                _savedLastOut = new double[lastOut.Length];
            }
            for (int i = 0; i < _savedLastOut.Length; i++)
            {
                _savedLastOut[i] = lastOut[i];
            }

            if (_savedLastIn == null)
            {
                _savedLastIn = new double[lastIn.Length];
            }
            for (int i = 0; i < _savedLastIn.Length; i++)
            {
                _savedLastIn[i] = lastIn[i];
            }


            if (_savedDEDX == null)
            {
                _savedDEDX = new double[dEdX.Length];
            }
            for (int i = 0; i < _savedDEDX.Length; i++)
            {
                _savedDEDX[i] = dEdX[i];
            }

            if (_savedDEDY == null)
            {
                _savedDEDY = new double[dEdY.Length];
            }
            for (int i = 0; i < _savedDEDY.Length; i++)
            {
                _savedDEDY[i] = dEdY[i];
            }

            if (_savedW == null)
            {
                _savedW = new double[weights.GetLength(0),weights.GetLength(1)];
            }
            for (int i = 0; i < _savedW.GetLength(0); i++)
            {
                for (int j = 0; j < _savedW.GetLength(1); j++)
                {
                    _savedW[i, j] = weights[i, j];
                }
            }

            if (_savedDEDW == null)
            {
                _savedDEDW = new double[dEdW.GetLength(0),dEdW.GetLength(1)];
            }
            for (int i = 0; i < _savedDEDW.GetLength(0); i++)
            {
                for (int j = 0; j < _savedDEDW.GetLength(1); j++)
                {
                    _savedDEDW[i, j] = dEdW[i, j];
                }
            }            
        }
        #endregion
        
        public StringBuilder GetWeightsStringRepr()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inputsCount; i++)
            {
                for (int j = 0; j < outputsCount; j++)
                {
                    sb.AppendFormat("{0} ", W[i, j]);
                }
                sb.Append("\n");
            }
            return sb;
        }

        public void Parse(string s)
        {
            var ws = s.Split(new char[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            weights = new double[inputsCount + Bias, outputsCount];
            for (int i = 0; i < inputsCount; i++)
            {
                for (int j = 0; j < outputsCount; j++)
                {
                    weights[i, j] = Double.Parse(ws[idx++]);
                }
            }
        }
    }
}
