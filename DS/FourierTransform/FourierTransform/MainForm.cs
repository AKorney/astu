using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourierTransform.SignalRepresentation;
using System.IO;
using System.Text.RegularExpressions;
using FourierTransform.Fourier;
using System.Numerics;
using System.Diagnostics;
using static System.Math;
using SignalProcessing;

namespace FourierTransform
{
    public partial class MainForm : Form
    {

        private Dictionary<string, SignalProfile> _profiles = new Dictionary<string, SignalProfile>();
        private List<SignalPoint> _transformedSource;
        private List<SignalPoint> _rawSource;
        public MainForm()
        {
            InitializeComponent();
            _profiles.Add("cardio", new SignalProfile(127, 60, 360, "сек", "мВ"));
            _profiles.Add("reo", new SignalProfile(0, 50, 360, "сек", "мОм"));
            _profiles.Add("velo", new SignalProfile(512, 120, 360, "сек", "мВ"));
            _profiles.Add("spiro", new SignalProfile(512, 100, 360, "сек", "Л"));
            _profiles.Add("default", new SignalProfile(0, 256, 360, "сек", ""));
                   
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "txt files (*.txt)|*.txt";
                ofd.InitialDirectory = @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\test";
                ofd.RestoreDirectory = true;
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    var profileKey = Regex.Replace(Path.GetFileNameWithoutExtension(ofd.FileName), "[0-9]", "");
                    if (!_profiles.ContainsKey(profileKey))
                        profileKey = "default";

                    var rawContent = File.ReadAllText(ofd.FileName);
                    var splittedText = rawContent.Split(new Char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    

                    _rawSource = splittedText.Select((item, index) => new SignalPoint { X = index, Y = Convert.ToDouble(item.Replace(".", ",")) }).ToList();

                    _transformedSource = SignalConverter.ConvertWithProfile(_rawSource, _profiles[profileKey]);

                    sourceChart.DataSource = _transformedSource;
                    sourceChart.Series["SourceSignal"].XValueMember = "X";
                    sourceChart.Series["SourceSignal"].YValueMembers = "Y";

                    sourceChart.ChartAreas["ChartArea1"].AxisX.Title = _profiles[profileKey].XLabel;
                    sourceChart.ChartAreas["ChartArea1"].AxisY.Title = _profiles[profileKey].YLabel;
                    sourceChart.DataBind();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            const int M = 8;

            int p = (int)Math.Floor(Math.Log(1.0 * _transformedSource.Count / M, 2));
            int availableCount = M * (int)Math.Pow(2, p);
            var range = _transformedSource.GetRange(0, availableCount);
            var signalCut = range.Select(pt => new Complex(pt.Y, 0)).ToArray();

            //var src = _transformedSource.Select(p => new Complex(p.Y, 0)).ToArray();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var transform = FourierTransformer.DFT(signalCut,false);
            sw.Stop();
            TimeSpan timeSpan = sw.Elapsed;

            label1.Text = string.Format("Time: {0}m {1}s {2}ms",  timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            var dft = FourierTransformer.CalculateSpecs(transform);

            sourceChart.DataSource = range;
            sourceChart.DataBind();

            ampSpec.DataSource = dft.amplitudeSpec.GetRange(0, dft.amplitudeSpec.Count/2);
            ampSpec.Series[0].XValueMember = "X";
            ampSpec.Series[0].YValueMembers = "Y";
            ampSpec.ChartAreas[0].AxisX.Title = "Гц";
            ampSpec.DataBind();


            phaseSpec.DataSource = dft.phaseSpec.GetRange(0,dft.phaseSpec.Count/2);
            phaseSpec.Series[0].XValueMember = "X";
            phaseSpec.Series[0].YValueMembers = "Y";
            phaseSpec.ChartAreas[0].AxisX.Title = "Гц";
            phaseSpec.DataBind();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int p = (int)Math.Floor(Math.Log(_transformedSource.Count, 2));
            var range = _transformedSource.GetRange(0, (int)Math.Pow(2, p));
            var signalCut = range.Select(pt => new Complex(pt.Y, 0)).ToArray();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var transform = FourierTransformer.FFT(signalCut, false);
            sw.Stop();
            TimeSpan timeSpan = sw.Elapsed;

            label1.Text = string.Format("Time: {0}m {1}s {2}ms", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

            var fft = FourierTransformer.CalculateSpecs(transform);

            sourceChart.DataSource = range;
            sourceChart.DataBind();

            ampSpec.DataSource = fft.amplitudeSpec.GetRange(0, fft.amplitudeSpec.Count / 2);
            ampSpec.Series[0].XValueMember = "X";
            ampSpec.Series[0].YValueMembers = "Y";
            ampSpec.ChartAreas[0].AxisX.Title = "Гц";
            ampSpec.DataBind();


            phaseSpec.DataSource = fft.phaseSpec.GetRange(0, fft.phaseSpec.Count / 2);
            phaseSpec.Series[0].XValueMember = "X";
            phaseSpec.Series[0].YValueMembers = "Y";
            phaseSpec.ChartAreas[0].AxisX.Title = "Гц";
            phaseSpec.DataBind();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            const int M = 8;

            int p = (int)Math.Floor(Math.Log(1.0*_transformedSource.Count/M, 2));
            int availableCount = M * (int)Math.Pow(2, p);
            var range = _transformedSource.GetRange(0, availableCount);
            var signalCut = range.Select(pt => new Complex(pt.Y, 0)).ToArray();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var transform = FourierTransformer.FFTn(signalCut, M, false);
            sw.Stop();
            TimeSpan timeSpan = sw.Elapsed;

            label1.Text = string.Format("Time: {0}m {1}s {2}ms", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

            var fft = FourierTransformer.CalculateSpecs(transform);

            sourceChart.DataSource = range;
            sourceChart.DataBind();

            ampSpec.DataSource = fft.amplitudeSpec.GetRange(0, fft.amplitudeSpec.Count / 2);
            ampSpec.Series[0].XValueMember = "X";
            ampSpec.Series[0].YValueMembers = "Y";
            ampSpec.ChartAreas[0].AxisX.Title = "Гц";
            ampSpec.DataBind();


            phaseSpec.DataSource = fft.phaseSpec.GetRange(0, fft.phaseSpec.Count / 2);
            phaseSpec.Series[0].XValueMember = "X";
            phaseSpec.Series[0].YValueMembers = "Y";
            phaseSpec.ChartAreas[0].AxisX.Title = "Гц";
            phaseSpec.DataBind();
        }

        private static int FindMaxPower(int N)=> (int)Math.Floor(Math.Log(1.0 * N, 2));
        private void button5_Click(object sender, EventArgs e)
        {
            var test = BuildWalshMatrix(8);
            int p = FindMaxPower(_transformedSource.Count);
            var range = _transformedSource.GetRange(0, (int)Math.Pow(2, p));
            var signalCut = range.Select(pt => pt.Y).ToArray();

            var direct = WalshTransform(signalCut);
            

            //var directSignal = direct.Select((d, i) => new Tuple<double,double>(i / 360.0, d));
            ShowWalshAfc(direct);

            var directClone = new double[direct.Length];

            direct.CopyTo(directClone, 0);
            if(numericUpDown1.Value>0)
            {
                int harm = (int)numericUpDown1.Value;
                for (int i  = harm*2+1; i<directClone.Length; i++)
                {
                    directClone[i] = 0;
                }
            }
            var invertLow = WalshTransform(directClone, true);
            var invertSignalLow = invertLow.Select((d, i) => new Tuple<double, double>(i / 360.0, d));
            SignalProcessing.ChartForm chartLow = new SignalProcessing.ChartForm(invertSignalLow.ToArray(),
                "sec", "", "Invert Walsh transform (low)", "Signal");
            chartLow.Show();

            direct.CopyTo(directClone, 0);
            if (numericUpDown1.Value > 0)
            {
                int harm = (int)numericUpDown1.Value;
                for (int i = 0; i < harm * 2 + 1; i++)
                {
                    directClone[i] = 0;
                    directClone[directClone.Length - i-1] = 0;
                }
            }
            var invertHigh = WalshTransform(directClone, true);
            var invertSignalHigh = invertHigh.Select((d, i) => new Tuple<double, double>(i / 360.0, d));
            SignalProcessing.ChartForm chartHigh = new SignalProcessing.ChartForm(invertSignalHigh.ToArray(),
                "sec", "", "Invert Walsh transform (high)", "Signal");
            chartHigh.Show();
        }

        private void ShowWalshAfc(double[] transform)
        {
            int N = transform.Length / 2;
            double[] afc = new double[N];
            double[] ffc = new double[N];
            //AFC
            afc[0] = transform[0];
            afc[N-1] = Abs(transform[2*N-1]);
            for (int i = 1; i < N-1; i++)
            {
                afc[i] = Sqrt(transform[2 * i - 1] * transform[2 * i - 1] + transform[2 * i] * transform[2 * i]);
            }
            var AFC = afc.Select((d, i) => new SignalPoint { X = i , Y = d }).ToList();
            ampSpec.DataSource = AFC;
            ampSpec.Series[0].XValueMember = "X";
            ampSpec.Series[0].YValueMembers = "Y";
            ampSpec.ChartAreas[0].AxisX.Title = "Гц";
            ampSpec.DataBind();

            ffc[0] = 0;
            ffc[N - 1] = PI / 2;
            for (int i = 1; i < N - 1; i++)
            {
                ffc[i] = Atan2(transform[2 * i - 1],transform[2 * i]);
            }
            var FFC = ffc.Select((d, i) => new SignalPoint { X = i , Y = d }).ToList();
            phaseSpec.DataSource = FFC;
            phaseSpec.Series[0].XValueMember = "X";
            phaseSpec.Series[0].YValueMembers = "Y";
            phaseSpec.ChartAreas[0].AxisX.Title = "Гц";
            phaseSpec.DataBind();
        }

        private static double[] WalshTransform(double[] source, bool invert = false)
        {
            int N = source.Length;
            var W = BuildWalshMatrix(N);
            double[] transform = new double[N];
            for (int k = 0; k < N; k++)
            {
                transform[k] = 0;
                for (int i = 0; i < N; i++)
                {
                    transform[k] += W[k][i] * source[i];
                }
                if (!invert) transform[k] /= N;
            }
            return transform;
        }

        private static int[][] BuildWalshMatrix(int N)
        {
            int n = FindMaxPower(N) + 1;
            int[][] result = new int[N][];
            int[] R = new int[n];
            for (int u = 0; u < N; u++)
            {
                result[u] = new int[N];
                int ut = u;
                int sr = 1;// << (n - 1);
                R[0] = (ut & sr) != 0 ? 1 : 0;
                for (int i = 1; i < n; i++)
                {
                    R[i] = (ut & sr) != 0 ? 1 : 0;
                    sr <<= 1;
                    R[i] += (ut & sr) != 0 ? 1 : 0;
                }
                for (int v = 0; v < N; v++)
                {
                    int vt = v;
                    int sum = 0;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        sum += R[i] * (vt & 1);
                        vt >>= 1;
                    }
                    result[u][v] = (sum % 2) == 0 ? 1 : -1;
                }
            }
            return result;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            var test = BuildHadamardMatrixRecursive(8);
            int p = FindMaxPower(_transformedSource.Count);
            var range = _transformedSource.GetRange(0, (int)Math.Pow(2, p));
            var signalCut = range.Select(pt => pt.Y).ToArray();

            var direct = HadamardTransform(signalCut);


            //var directSignal = direct.Select((d, i) => new Tuple<double,double>(i / 360.0, d));
            ShowWalshAfc(direct);

            var directClone = new double[direct.Length];

            direct.CopyTo(directClone, 0);
            if (numericUpDown1.Value > 0)
            {
                int harm = (int)numericUpDown1.Value;
                for (int i = harm * 2 + 1; i < directClone.Length; i++)
                {
                    directClone[i] = 0;
                }
            }
            var invertLow = HadamardTransform(directClone, true);
            var invertSignalLow = invertLow.Select((d, i) => new Tuple<double, double>(i / 360.0, d));
            SignalProcessing.ChartForm chartLow = new SignalProcessing.ChartForm(invertSignalLow.ToArray(),
                "sec", "", "Invert Hadamard transform (low)", "Signal");
            chartLow.Show();

            direct.CopyTo(directClone, 0);
            if (numericUpDown1.Value > 0)
            {
                int harm = (int)numericUpDown1.Value;
                for (int i = 0; i < harm * 2 + 1; i++)
                {
                    directClone[i] = 0;
                    directClone[directClone.Length - i - 1] = 0;
                }
            }
            var invertHigh = HadamardTransform(directClone, true);
            var invertSignalHigh = invertHigh.Select((d, i) => new Tuple<double, double>(i / 360.0, d));
            SignalProcessing.ChartForm chartHigh = new SignalProcessing.ChartForm(invertSignalHigh.ToArray(),
                "sec", "", "Invert Hadamard transform (high)", "Signal");
            chartHigh.Show();
        }

        private double[] HadamardTransform(double[] source, bool invert = false)
        {
            int N = source.Length;
            var W = BuildHadamardMatrixRecursive(N);
            double[] transform = new double[N];
            for (int k = 0; k < N; k++)
            {
                transform[k] = 0;
                for (int i = 0; i < N; i++)
                {
                    transform[k] += W[k][i] * source[i];
                }
                if (!invert) transform[k] /= N;
            }
            return transform;
        }

        private int[][] BuildHadamardMatrixRecursive(int n)
        {
            if (n == 1)
            {
                return new int[][]
                {
                        new int[]{ 1 }
                };
            }
            var submatrix = BuildHadamardMatrixRecursive(n >> 1);
            return MatrixOperationsHelper.CombineSubmatrix(submatrix, submatrix, submatrix, submatrix.NegateValues());
        }

    }
}


/*
 * 
 * 
 * List<SignalPoint> raw = new List<SignalPoint>();
            raw.Add(new SignalPoint { X = 1, Y = 1 });
            raw.Add(new SignalPoint { X = 2, Y = 1 });
            raw.Add(new SignalPoint { X = 3, Y = 1 });
            raw.Add(new SignalPoint { X = 4, Y = 1 });
            raw.Add(new SignalPoint { X = 5, Y = 1 });
            SignalProfile none = new SignalProfile(0.5, 0.5, 10, "X", "Y");
            var profiled = SignalConverter.ConvertWithProfile(raw, none);
            chart1.DataSource = profiled;
            chart1.Series.Add("Signal");
            chart1.Series["Signal"].XValueMember = "X";
            chart1.Series["Signal"].YValueMembers = "Y";
            chart1.Series["Signal"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.DataBind();
 */
