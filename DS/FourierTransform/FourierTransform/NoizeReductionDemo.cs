using FourierTransform.SignalRepresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FourierTransform
{
    public partial class NoizeReductionDemo : Form
    {
        private Dictionary<string, SignalProfile> _profiles = new Dictionary<string, SignalProfile>();
        public NoizeReductionDemo()
        {
            InitializeComponent();
            _profiles.Add("cardio", new SignalProfile(127, 60, 360, "сек", "мВ"));
            _profiles.Add("reo", new SignalProfile(0, 50, 360, "сек", "мОм"));
            _profiles.Add("velo", new SignalProfile(512, 120, 360, "сек", "мВ"));
            
            MakeVelo();
        }
        private void MakeCardio()
        {
            List<SignalPoint> sourceSignal = new List<SignalPoint>();


            string path = @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\test\car\cardio05.txt";
            sourceSignal = ReadData(path, "cardio");

            var ab = Fourier.FourierTransformer.CalculateAB(sourceSignal);
            double hc = _profiles["cardio"].Frequency / sourceSignal.Count;
            int targetIndex = (int)Math.Floor(50.0 / hc + 0.5);
            ab.a[targetIndex - 1] = ab.a[targetIndex] = ab.a[targetIndex + 1] = 0;
            ab.b[targetIndex - 1] = ab.b[targetIndex] = ab.b[targetIndex + 1] = 0;
            var result = Fourier.FourierTransformer.InverseTransform(ab);

            ShowCharts(sourceSignal, result, "cardio");
        }

        private void MakeReo()
        {
            List<SignalPoint> sourceSignal = new List<SignalPoint>();


            string path = @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\test\reo\reo01.txt";
            sourceSignal = ReadData(path, "reo");

            var ab = Fourier.FourierTransformer.CalculateAB(sourceSignal);
            double hc = _profiles["reo"].Frequency / sourceSignal.Count;
            int targetIndex = (int)Math.Floor(60.0 / hc + 0.5);
            for (int i = targetIndex; i < sourceSignal.Count - targetIndex ; i++)
            {
                ab.a[i] = ab.b[i] = 0;
            }
            var result = Fourier.FourierTransformer.InverseTransform(ab);

            ShowCharts(sourceSignal, result, "reo");
        }

        private void MakeVelo()
        {
            List<SignalPoint> sourceSignal = new List<SignalPoint>();


            string path = @"D:\stud_repo\astu\DS\FourierTransform\FourierTransform\test\velo\velo04.txt";
            sourceSignal = ReadData(path, "velo");

            var ab = Fourier.FourierTransformer.CalculateAB(sourceSignal);
            double hc = _profiles["velo"].Frequency / sourceSignal.Count;
            int targetIndex = (int)Math.Floor(3.0 / hc + 0.5);
            for (int i = 0; i < targetIndex; i++)
            {
                ab.a[i] = ab.a[ab.a.Length - i - 1] = 0;
                ab.b[i] = ab.b[ab.b.Length - i - 1] = 0;
            }
            var result = Fourier.FourierTransformer.InverseTransform(ab);

            ShowCharts(sourceSignal, result, "reo");
        }

        private List<SignalPoint> ReadData(string path, string profile)
        {
            List<SignalPoint> sourceSignal;
            var rawContent = File.ReadAllText(path);
            var splittedText = rawContent.Split(new Char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //List<SignalPoint> rawSignal = new List<SignalPoint>();
            //for (int i = 0; i < splittedText.Length; i++)
            //{
            //    rawSignal.Add(new SignalPoint { X = i, Y = Convert.ToDouble(splittedText[i].Replace(".", ",")) });
            //}
            List<SignalPoint> rawSignal = splittedText.Select((item, index) => new SignalPoint { X = index, Y = Convert.ToDouble(item) }).ToList();
            sourceSignal = SignalConverter.ConvertWithProfile(rawSignal, _profiles[profile]);
            return sourceSignal;
        }

        private void ShowCharts(List<SignalPoint> sourceSignal, List<SignalPoint> result, string profile)
        {
            chart1.DataSource = sourceSignal;
            chart1.Series[0].XValueMember = "X";
            chart1.Series[0].YValueMembers = "Y";

            chart1.ChartAreas[0].AxisX.Title = _profiles[profile].XLabel;
            chart1.ChartAreas[0].AxisY.Title = _profiles[profile].YLabel;
            chart1.DataBind();

            chart2.DataSource = result;
            chart2.Series[0].XValueMember = "X";
            chart2.Series[0].YValueMembers = "Y";

            chart2.ChartAreas[0].AxisX.Title = _profiles[profile].XLabel;
            chart2.ChartAreas[0].AxisY.Title = _profiles[profile].YLabel;
            chart2.DataBind();
        }
    }
}
