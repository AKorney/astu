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
